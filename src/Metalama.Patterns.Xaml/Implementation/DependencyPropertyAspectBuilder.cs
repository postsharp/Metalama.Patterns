// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;
using Metalama.Patterns.Xaml.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal sealed partial class DependencyPropertyAspectBuilder
{
    private readonly IAspectBuilder<IProperty> _builder;
    private readonly Assets _assets;
    private readonly IType _propertyType;
    private readonly INamedType _declaringType;
    private readonly string _propertyName;
    private readonly DependencyPropertyOptions _options;

    public DependencyPropertyAspectBuilder( IAspectBuilder<IProperty> builder )
    {
        this._builder = builder;
        this._assets = builder.Target.Compilation.Cache.GetOrAdd( _ => new Assets() );
        this._propertyType = builder.Target.Type;
        this._declaringType = builder.Target.DeclaringType;
        this._propertyName = builder.Target.Name;
        this._options = builder.Target.Enhancements().GetOptions<DependencyPropertyOptions>();
    }

    public void Build()
    {
        // NB: WPF convention requires a specific field name, so we don't try to find an unused name.
        var dependencyPropertyFieldName = this._options.RegistrationField ?? $"{this._propertyName}Property";

        IIntroductionAdviceResult<IField>? introduceDependencyPropertyFieldResult = null;

        // Check for a conflicting member name explicitly because introduction won't fail unless the conflict comes from another field.

        var conflictingMember = (IMemberOrNamedType) this._declaringType.AllMembers().FirstOrDefault( m => m.Name == dependencyPropertyFieldName ) ?? this._declaringType.NestedTypes.FirstOrDefault( t => t.Name == dependencyPropertyFieldName );
        
        if ( conflictingMember != null )
        {
            this._builder.Diagnostics.Report( Diagnostics.ErrorRequiredDependencyPropertyFieldNameIsAlreadyUsed.WithArguments( (conflictingMember, this._declaringType, dependencyPropertyFieldName) ) );
        }
        else
        {
            introduceDependencyPropertyFieldResult = this._builder.Advice.IntroduceField(
                this._declaringType,
                this._options.RegistrationField ?? $"{this._propertyName}Property",
                typeof( DependencyProperty ),
                IntroductionScope.Static,
                OverrideStrategy.Fail,
                b =>
                {
                    b.Accessibility = Framework.Code.Accessibility.Public;
                    b.Writeability = Writeability.ConstructorOnly;
                } );
        }

        var methodsByName = this._declaringType.Methods.ToLookup( m => m.Name );

        var onChangingMethodName = this._options.PropertyChangingMethod ?? $"On{this._propertyName}Changing";
        var onChangedMethodName = this._options.PropertyChangedMethod ?? $"On{this._propertyName}Changed";
        var validateMethodName = this._options.ValidateMethod ?? $"Validate{this._propertyName}";

        var (onChangingMethod, onChangingSignatureKind) = this.GetChangeHandlerMethod( methodsByName, onChangingMethodName, allowOldValue: false, nameof( this._options.PropertyChangingMethod ) );
        var (onChangedMethod, onChangedSignatureKind) = this.GetChangeHandlerMethod( methodsByName, onChangedMethodName, allowOldValue: true, nameof( this._options.PropertyChangedMethod ) );
        var (validateMethod, validateSignatureKind) = this.GetValidationHandlerMethod( methodsByName, validateMethodName, nameof( this._options.ValidateMethod ) );

        if ( this._options.PropertyChangingMethod != null && onChangingSignatureKind == ChangeHandlerSignatureKind.NotFound )
        {
            this._builder.Diagnostics.Report( 
                Diagnostics.WarningConfiguredHandlerMethodNotFound.WithArguments( (this._declaringType, this._options.PropertyChangingMethod, nameof( this._options.PropertyChangingMethod )) ) );
        }

        if ( this._options.PropertyChangedMethod != null && onChangedSignatureKind == ChangeHandlerSignatureKind.NotFound )
        {
            this._builder.Diagnostics.Report(
                Diagnostics.WarningConfiguredHandlerMethodNotFound.WithArguments( (this._declaringType, this._options.PropertyChangedMethod, nameof( this._options.PropertyChangedMethod )) ) );
        }

        if ( this._options.ValidateMethod != null && onChangedSignatureKind == ChangeHandlerSignatureKind.NotFound )
        {
            this._builder.Diagnostics.Report(
                Diagnostics.WarningConfiguredHandlerMethodNotFound.WithArguments( (this._declaringType, this._options.ValidateMethod, nameof( this._options.PropertyChangedMethod )) ) );
        }

        if ( this._builder.Target.InitializerExpression != null && this._options.InitializerProvidesInitialValue != true && this._options.InitializerProvidesDefaultValue != true )
        {
            this._builder.Diagnostics.Report( Diagnostics.WarningDependencyPropertyInitializerWillNotBeUsed.WithArguments( this._builder.Target ) );
        }

        if ( !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            // TODO: CapturesNonObservableTransformations handling: stopping here may lead to warnings in user code that handler methods are not used, because we don't generate calls to them.

            return;
        }

        if ( introduceDependencyPropertyFieldResult is not { Outcome: AdviceOutcome.Default } )
        {
            // We cannot proceed with other transformations if we could not introduce the DependencyProperty field.

            return;
        }

        /* Regarding setting the initial value, the PostSharp implementation takes care to:
         * 
         * - Only set the initial value when definitely supplied via an initializer and not when supplied via the DefaultValue property of the aspect.
         * - Set the initial value using DependencyObject.SetCurrentValue ("otherwise we override value coming from templates").
         * - Suspend enforcement of contracts and explicit validation around that call.
         * 
         * See also https://tp.postsharp.net/entity/26136-dependencyproperty-values-are-not-set-when
         * 
         * TG: My understanding from a Metalama perspective is that we have easy access and control of the initializer expression, and it can be used
         * to provide the PropertyMetadata.DefaultValue. PostSharp could not do this, so it provided the DefaultValue attribute property as a
         * compromised alternative (only constant default values can be provided that way). So in ML, we should not need the Start/StopIgnoringContracts
         * concept. By default (according to DependencyPropertyOptions), we will use the initializer to set PropertyMetadata.DefaultValue
         * (controlled by DependencyPropertyOptions.IntiializerProvidesDefaultValue), and we will *not* call DependencyObject.SetValue (controlled by
         * DependencyPropertyOptions.IntiializerProvidesInitialValue).
         */

        this._builder.Advice.WithTemplateProvider( Templates.Provider ).AddInitializer(
            this._declaringType,
            nameof( Templates.InitializeDependencyProperty ),
            InitializerKind.BeforeTypeConstructor,
            args: new
            {
                dependencyPropertyField = introduceDependencyPropertyFieldResult.Declaration,
                options = this._options,
                propertyName = this._propertyName,
                propertyType = this._propertyType,
                declaringType = this._declaringType,
                defaultValueExpr = this._builder.Target.InitializerExpression,
                onChangingMethod,
                onChangingSignatureKind,
                onChangedMethod,
                onChangedSignatureKind,
                validateMethod,
                validateSignatureKind
            } );

        this._builder.Advice.WithTemplateProvider( Templates.Provider ).OverrideAccessors(
            this._builder.Target,
            new GetterTemplateSelector( nameof( Templates.OverrideGetter ) ),
            args: new
            {
                propertyType = this._propertyType,
                dependencyPropertyField = introduceDependencyPropertyFieldResult.Declaration
            } );

        if ( this._builder.Target.Writeability != Writeability.None )
        {
            this._builder.Advice.WithTemplateProvider( Templates.Provider ).OverrideAccessors(
                this._builder.Target,
                setTemplate: nameof( Templates.OverrideSetter ),
                args: new
                {
                    dependencyPropertyField = introduceDependencyPropertyFieldResult.Declaration
                } );
        }

        // Here we avoid the temptation to generate a static field to store the result of the initializer expression
        // and use the same result for the default value and as the initial value of all instances of the declaring type. This
        // pattern does not have the same semantics as a regular property initializer, which would be invoked for each instance
        // of the declaring type. So we now emulate normal semantics to avoid surprise. If required, the user can themself implement
        // singleton semantics as they would for any regular property initializer.

        if ( this._builder.Target.InitializerExpression != null && this._options.InitializerProvidesInitialValue == true )
        {
            this._builder.Advice.WithTemplateProvider( Templates.Provider ).AddInitializer(
                    this._declaringType,
                    nameof( Templates.Assign ),
                    InitializerKind.BeforeInstanceConstructor,
                    args: new
                    {
                        left = (IExpression) this._builder.Target.Value!,
                        right = this._builder.Target.InitializerExpression
                    } );
        }
    }

    private (IMethod? ChangeHanlderMethod, ChangeHandlerSignatureKind SignatureKind) GetChangeHandlerMethod(
        ILookup<string, IMethod> methodsByName,
        string methodName,
        bool allowOldValue,
        string optionName )
    {
        IMethod? method;

        var onChangingGroup = methodsByName[methodName];

        switch ( onChangingGroup.Count() )
        {
            case 0:
                return (null, ChangeHandlerSignatureKind.NotFound);

            case 1:
                method = onChangingGroup.First();
                break;

            default:

                this._builder.Diagnostics.Report( Diagnostics.ErrorHandlerMethodIsAmbiguous.WithArguments( (this._declaringType, methodName, optionName) ) );
                
                return (null, ChangeHandlerSignatureKind.Ambiguous);
        }

        var signatureKind = ChangeHandlerSignatureKind.Invalid;

        if ( method != null )
        {
            signatureKind = this.GetChangeHandlerSignature( method, this._declaringType, this._propertyType, allowOldValue );

            if ( signatureKind == ChangeHandlerSignatureKind.Invalid )
            {
                this._builder.Diagnostics.Report( Diagnostics.ErrorHandlerMethodIsInvalid.WithArguments( (optionName, this._builder.Target) ), method );
            }
        }

        return (method, signatureKind);
    }

    private ChangeHandlerSignatureKind GetChangeHandlerSignature(
        IMethod method,
        INamedType declaringType,
        IType propertyType,
        bool allowOldValue )
    {
        var p = method.Parameters;

        if ( method.ReturnType.SpecialType != SpecialType.Void || p.Count > 2 || p.Any( p => p.RefKind is not RefKind.None or RefKind.In ) )
        {
            return ChangeHandlerSignatureKind.Invalid;
        }

        switch ( p.Count )
        {
            case 0:
                return method.IsStatic ? ChangeHandlerSignatureKind.StaticNoParameters : ChangeHandlerSignatureKind.InstanceNoParameters;

            case 1:

                if ( p[0].Type.Equals( this._assets.DependencyProperty ) )
                {
                    return method.IsStatic ? ChangeHandlerSignatureKind.StaticDependencyProperty : ChangeHandlerSignatureKind.InstanceDependencyProperty;
                }
                else if ( method.IsStatic
                            && (p[0].Type.SpecialType == SpecialType.Object
                                || p[0].Type.Equals( declaringType )
                                || p[0].Type.Equals( this._assets.DependencyObject )) )
                {
                    return ChangeHandlerSignatureKind.StaticInstance;
                }
                else if ( !method.IsStatic
                            && (p[0].Type.SpecialType == SpecialType.Object
                                || propertyType.Is( p[0].Type )
                                || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ChangeHandlerSignatureKind.InstanceValue;
                }

                break;

            case 2:

                if ( method.IsStatic
                        && p[0].Type.Equals( this._assets.DependencyProperty )
                        && (p[1].Type.SpecialType == SpecialType.Object
                            || p[1].Type.Equals( declaringType )
                            || p[1].Type.Equals( this._assets.DependencyObject )) )
                {
                    return ChangeHandlerSignatureKind.StaticDependencyPropertyAndInstance;
                }
                else if ( allowOldValue
                            && !method.IsStatic
                            && (p[0].Type.SpecialType == SpecialType.Object
                                || propertyType.Is( p[0].Type )
                                || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1))
                            && (p[1].Type.SpecialType == SpecialType.Object
                                || propertyType.Is( p[1].Type )
                                || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ChangeHandlerSignatureKind.InstanceOldValueAndNewValue;
                }

                break;
        }

        return ChangeHandlerSignatureKind.Invalid;
    }

    private (IMethod? ChangeHanlderMethod, ValidationHandlerSignatureKind SignatureKind) GetValidationHandlerMethod(
        ILookup<string, IMethod> methodsByName,
        string methodName,
        string optionName )
    {
        IMethod? method;

        var onChangingGroup = methodsByName[methodName];

        switch ( onChangingGroup.Count() )
        {
            case 0:
                return (null, ValidationHandlerSignatureKind.NotFound);

            case 1:
                method = onChangingGroup.First();
                break;

            default:

                this._builder.Diagnostics.Report( Diagnostics.ErrorHandlerMethodIsAmbiguous.WithArguments( (this._declaringType, methodName, optionName) ) );

                return (null, ValidationHandlerSignatureKind.Ambiguous);
        }

        var signatureKind = ValidationHandlerSignatureKind.Invalid;

        if ( method != null )
        {
            signatureKind = this.GetValidationHandlerSignature( method, this._declaringType, this._propertyType );
            if ( signatureKind == ValidationHandlerSignatureKind.Invalid )
            {
                this._builder.Diagnostics.Report( Diagnostics.ErrorHandlerMethodIsInvalid.WithArguments( (optionName, this._builder.Target) ), method );
            }
        }

        return (method, signatureKind);
    }

    private ValidationHandlerSignatureKind GetValidationHandlerSignature(
        IMethod method,
        INamedType declaringType,
        IType propertyType )
    {
        var p = method.Parameters;

        if ( method.ReturnType.SpecialType != SpecialType.Boolean
            || method.ReturnParameter.RefKind != RefKind.None
            || p.Count > 3
            || p.Any( p => p.RefKind is not RefKind.None or RefKind.In ) )
        {
            return ValidationHandlerSignatureKind.Invalid;
        }

        switch ( p.Count )
        {
            case 0:
                return ValidationHandlerSignatureKind.Invalid;

            case 1:

                if ( p[0].Type.SpecialType == SpecialType.Object
                     || propertyType.Is( p[0].Type )
                     || (p[0].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1) )
                {
                    return method.IsStatic ? ValidationHandlerSignatureKind.StaticValue : ValidationHandlerSignatureKind.InstanceValue;
                }

                break;

            case 2:

                if ( p[0].Type.Equals( this._assets.DependencyProperty )
                     && (p[1].Type.SpecialType == SpecialType.Object
                        || propertyType.Is( p[1].Type )
                        || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1) ) )
                {
                    return method.IsStatic ? ValidationHandlerSignatureKind.StaticDependencyPropertyAndValue : ValidationHandlerSignatureKind.InstanceDependencyPropertyAndValue;
                }
                else if ( method.IsStatic
                          && (p[0].Type.SpecialType == SpecialType.Object
                              || p[0].Type.Equals( declaringType )
                              || p[0].Type.Equals( this._assets.DependencyObject ))
                          && (p[1].Type.SpecialType == SpecialType.Object
                              || propertyType.Is( p[1].Type )
                              || (p[1].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ValidationHandlerSignatureKind.StaticInstanceAndValue;
                }

                break;

            case 3:

                if ( method.IsStatic
                        && p[0].Type.Equals( this._assets.DependencyProperty )
                        && (p[1].Type.SpecialType == SpecialType.Object
                            || p[1].Type.Equals( declaringType )
                            || p[1].Type.Equals( this._assets.DependencyObject ))
                        && (p[2].Type.SpecialType == SpecialType.Object
                            || propertyType.Is( p[2].Type )
                            || (p[2].Type.TypeKind == TypeKind.TypeParameter && method.TypeParameters.Count == 1)) )
                {
                    return ValidationHandlerSignatureKind.StaticDependencyPropertyAndInstanceAndValue;
                }

                break;
        }

        return ValidationHandlerSignatureKind.Invalid;
    }
}