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

        var introduceDependencyPropertyFieldResult = this._builder.Advice.IntroduceField(
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

        var methodsByName = this._declaringType.Methods.ToLookup( m => m.Name );

        var onChangingHandlerName = this._options.PropertyChangingMethod ?? $"On{this._propertyName}Changing";
        var onChangedHandlerName = this._options.PropertyChangedMethod ?? $"On{this._propertyName}Changed";

        var (onChangingHandlerMethod, onChangingHandlerParametersKind) = this.GetHandlerMethod( methodsByName, onChangingHandlerName, allowOldValue: false, nameof( this._options.PropertyChangingMethod ) );
        var (onChangedHandlerMethod, onChangedHandlerParametersKind) = this.GetHandlerMethod( methodsByName, onChangedHandlerName, allowOldValue: true, nameof( this._options.PropertyChangedMethod ) );

        if ( this._options.PropertyChangingMethod != null && onChangingHandlerParametersKind == ChangeHandlerSignatureKind.NotFound )
        {
            this._builder.Diagnostics.Report( 
                Diagnostics.WarningConfiguredHandlerMethodNotFound.WithArguments( (this._declaringType, this._options.PropertyChangingMethod, nameof( this._options.PropertyChangingMethod )) ) );
        }

        if ( this._options.PropertyChangedMethod != null && onChangedHandlerParametersKind == ChangeHandlerSignatureKind.NotFound )
        {
            this._builder.Diagnostics.Report(
                Diagnostics.WarningConfiguredHandlerMethodNotFound.WithArguments( (this._declaringType, this._options.PropertyChangedMethod, nameof( this._options.PropertyChangedMethod )) ) );
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

        if ( introduceDependencyPropertyFieldResult.Outcome != AdviceOutcome.Default )
        {
            // We cannot proceed with other transformations if we could not introduce the DependencyProperty field.

            return;
        }

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
                onChangingHandlerMethod,
                onChangingHandlerParametersKind,
                onChangedHandlerMethod,
                onChangedHandlerParametersKind
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

        // NB: In a previous implementation, we would generate a static field to store the result of the initializer expression
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

    private (IMethod? ChangeHanlderMethod, ChangeHandlerSignatureKind ParametersKind) GetHandlerMethod(
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

        var parametersKind = ChangeHandlerSignatureKind.Invalid;

        if ( method != null )
        {
            parametersKind = GetChangeHandlerSignature( method, this._declaringType, this._propertyType, allowOldValue, this._assets );

            if ( parametersKind == ChangeHandlerSignatureKind.Invalid )
            {
                this._builder.Diagnostics.Report( Diagnostics.ErrorHandlerMethodIsInvalid.WithArguments( (optionName, this._builder.Target) ), method );
            }
        }

        return (method, parametersKind);
    }

    private static ChangeHandlerSignatureKind GetChangeHandlerSignature(
        IMethod method,
        INamedType declaringType,
        IType propertyType,
        bool allowOldValue,
        Assets assets )
    {
        var p = method.Parameters;

        switch ( p.Count )
        {
            case 0:
                return method.IsStatic ? ChangeHandlerSignatureKind.StaticNoParameters : ChangeHandlerSignatureKind.InstanceNoParameters;

            case 1:

                if ( p[0].RefKind is RefKind.None or RefKind.In )
                {
                    if ( p[0].Type.Equals( assets.DependencyProperty ) )
                    {
                        return method.IsStatic ? ChangeHandlerSignatureKind.StaticDependencyProperty : ChangeHandlerSignatureKind.InstanceDependencyProperty;
                    }
                    else if ( method.IsStatic
                             && (p[0].Type.SpecialType == SpecialType.Object
                                  || p[0].Type.Equals( declaringType )
                                  || p[0].Type.Equals( assets.DependencyObject )) )
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
                }
                break;

            case 2:

                if ( p[0].RefKind is RefKind.None or RefKind.In && p[1].RefKind is RefKind.None or RefKind.In )
                {
                    if ( method.IsStatic
                         && p[0].Type.Equals( assets.DependencyProperty )
                         && (p[1].Type.SpecialType == SpecialType.Object
                              || p[1].Type.Equals( declaringType )
                              || p[1].Type.Equals( assets.DependencyObject )) )
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
                }
                break;
        }

        return ChangeHandlerSignatureKind.Invalid;
    }
}