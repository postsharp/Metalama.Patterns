// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Clean up unused references in project file.

// TODO: Investigating compile time exception inside Metalma. Remove conditions when resolved.
// #define ENABLE_DP_CALLBACKS

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Project;
using Metalama.Patterns.Xaml.Implementation;
using System.Windows;

namespace Metalama.Patterns.Xaml;

// The required conventions are described here:
// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/properties/custom-dependency-properties?view=netdesktop-7.0

[AttributeUsage( AttributeTargets.Property )]
public sealed partial class DependencyPropertyAttribute : Attribute, IAspect<IProperty>
{
    /// <summary>
    /// Gets or sets a value indicating whether the property should be registered as a read-only property.
    /// </summary>
    public bool IsReadOnly { get; set; }

    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        builder.MustNotBeStatic();
        builder.MustHaveAccessibility( Framework.Code.Accessibility.Public );
        builder.MustBeReadable();
        builder.MustSatisfy( p => p.IsAutoPropertyOrField == true, p => $"{p} must be an auto-property." );
        builder.DeclaringType().MustBe( typeof( DependencyObject ), ConversionKind.Reference );
    }

    void IAspect<IProperty>.BuildAspect( IAspectBuilder<IProperty> builder )
    {
        var assets = builder.Target.Compilation.Cache.GetOrAdd( _ => new Assets() );

        var propertyType = builder.Target.Type;
        var declaringType = builder.Target.DeclaringType;
        var propertyName = builder.Target.Name;

        // NB: WPF convention requires a specific field name.

        var introduceFieldResult = builder.WithTarget( declaringType ).Advice.IntroduceField(
            declaringType,
            $"{propertyName}Property",
            typeof( DependencyProperty ),
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Accessibility = Framework.Code.Accessibility.Public;
                b.Writeability = Writeability.ConstructorOnly;
            } );

        if ( introduceFieldResult.Outcome != AdviceOutcome.Default || !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            return;
        }

        var methodsByName = declaringType.Methods.ToLookup( m => m.Name );

        var onChangingHandlerName = $"On{propertyName}Changing";
        var onChangedHandlerName = $"On{propertyName}Changed";

        var (onChangingHandlerMethod, onChangingHandlerParametersKind) = 
            GetHandlerMethod( methodsByName, onChangingHandlerName, declaringType, propertyType, allowOldValue: false, assets );

        var (onChangedHandlerMethod, onChangedHandlerParametersKind) = 
            GetHandlerMethod( methodsByName, onChangedHandlerName, declaringType, propertyType, allowOldValue: true, assets );

        builder.WithTarget( declaringType ).Advice.AddInitializer(
            declaringType,
            nameof( InitializeProperty ),
            InitializerKind.BeforeTypeConstructor,
            args: new
            {
                dependencyPropertyField = introduceFieldResult.Declaration,
                registerAsReadOnly = this.IsReadOnly,
                propertyName,
                propertyType,
                declaringType,
                defaultValueExpr = (IExpression?) null,
                onChangingHandlerMethod,
                onChangingHandlerParametersKind,
                onChangedHandlerMethod,
                onChangedHandlerParametersKind
            } );

        builder.Advice.OverrideAccessors(
            builder.Target,
            new GetterTemplateSelector( nameof( OverrideGetter ) ),
            args: new
            {
                propertyType,
                dependencyPropertyField = introduceFieldResult.Declaration
            } );

        if ( builder.Target.Writeability != Writeability.None )
        {
            builder.Advice.OverrideAccessors(
                builder.Target,
                setTemplate: nameof( OverrideSetter ),
                args: new
                {
                    dependencyPropertyField = introduceFieldResult.Declaration
                } );
        }
    }

    private static (IMethod? ChangeHanlderMethod, ChangeHandlerParametersKind ParametersKind) GetHandlerMethod(
        ILookup<string, IMethod> methodsByName,
        string methodName,
        INamedType declaringType,
        IType propertyType,
        bool allowOldValue,
        Assets assets )
    {
        IMethod? method = null;
        var parametersKind = ChangeHandlerParametersKind.Invalid;

        var onChangingGroup = methodsByName[methodName];

        switch ( onChangingGroup.Count() )
        {
            case 0:
                break;

            case 1:
                method = onChangingGroup.First();
                break;

            default:
                // TODO: Ambiguous method diagnostic
                throw new NotSupportedException( $"Ambiguous handler method for {methodName}" );
        }

        if ( method != null )
        {
            parametersKind = GetChangeHandlerParametersKind( method, declaringType, propertyType, allowOldValue, assets );

            if ( parametersKind == ChangeHandlerParametersKind.Invalid )
            {
                // TODO: Invalid method signature diagnostic
                throw new NotSupportedException( $"Invalid handler method signature for {methodName}" );
            }
        }

        return (method, parametersKind);
    }

    private static ChangeHandlerParametersKind GetChangeHandlerParametersKind(
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
                return method.IsStatic ? ChangeHandlerParametersKind.StaticNone : ChangeHandlerParametersKind.None;

            case 1:

                if ( p[0].RefKind is RefKind.None or RefKind.In )
                {
                    if ( p[0].Type == assets.DependencyProperty )
                    {
                        return method.IsStatic ? ChangeHandlerParametersKind.StaticDependencyProperty : ChangeHandlerParametersKind.DependencyProperty;
                    }
                    else if ( method.IsStatic
                             && (p[0].Type.SpecialType == SpecialType.Object
                                  || p[0].Type == declaringType
                                  || p[0].Type == assets.DependencyObject) )
                    {
                        return ChangeHandlerParametersKind.StaticInstance;
                    }
                    else if ( !method.IsStatic
                              && (p[0].Type.SpecialType == SpecialType.Object
                                   || propertyType.Is( p[0].Type )
                                   || p[0].Type.TypeKind == TypeKind.TypeParameter) )
                    {
                        return ChangeHandlerParametersKind.Value;
                    }
                }
                break;

            case 2:

                if ( p[0].RefKind is RefKind.None or RefKind.In && p[1].RefKind is RefKind.None or RefKind.In )
                {
                    if ( method.IsStatic
                         && p[0].Type == assets.DependencyProperty
                         && (p[1].Type.SpecialType == SpecialType.Object
                              || p[1].Type == declaringType
                              || p[1].Type == assets.DependencyObject) )
                    {
                        return ChangeHandlerParametersKind.StaticDependencyPropertyAndInstance;
                    }
                    else if ( allowOldValue 
                              && !method.IsStatic
                              && (p[0].Type.SpecialType == SpecialType.Object
                                   || propertyType.Is( p[0].Type )
                                   || p[0].Type.TypeKind == TypeKind.TypeParameter)
                              && (p[1].Type.SpecialType == SpecialType.Object
                                   || propertyType.Is( p[1].Type )
                                   || p[1].Type.TypeKind == TypeKind.TypeParameter) )
                    {
                        return ChangeHandlerParametersKind.OldValueAndNewValue;
                    }
                }
                break;
        }

        return ChangeHandlerParametersKind.Invalid;
    }

    [Template]
    private static void InvokeChangeHandler(
        [CompileTime] IField dependencyPropertyField,
        [CompileTime] IMethod handlerMethod, 
        [CompileTime] ChangeHandlerParametersKind parametersKind,
        [CompileTime] INamedType propertyType,
        [CompileTime] INamedType declaringType,
        [CompileTime] IExpression instanceExpr,
        [CompileTime] IExpression? oldValueExpr,
        [CompileTime] IExpression newValueExpr )
    {
        switch ( parametersKind )
        {
            // TODO: Some casts may not be required depending on the type of the method parameter.

            case ChangeHandlerParametersKind.None:
                handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) ).Invoke();
                break;

            case ChangeHandlerParametersKind.StaticNone:
                handlerMethod!.Invoke();
                break;

            case ChangeHandlerParametersKind.Value:
                handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                    .Invoke( handlerMethod.Parameters[0].Type.SpecialType == SpecialType.Object ? newValueExpr.Value : meta.Cast( propertyType, newValueExpr.Value ) );
                break;

            case ChangeHandlerParametersKind.OldValueAndNewValue:
                handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                    .Invoke( 
                        handlerMethod.Parameters[0].Type.SpecialType == SpecialType.Object ? oldValueExpr!.Value : meta.Cast( propertyType, oldValueExpr!.Value ),
                        handlerMethod.Parameters[1].Type.SpecialType == SpecialType.Object ? newValueExpr.Value : meta.Cast( propertyType, newValueExpr.Value ) );
                break;

            case ChangeHandlerParametersKind.DependencyProperty:
                handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) ).Invoke( dependencyPropertyField.Value );
                break;

            case ChangeHandlerParametersKind.StaticDependencyProperty:
                handlerMethod!.Invoke( dependencyPropertyField.Value );
                break;

            case ChangeHandlerParametersKind.StaticDependencyPropertyAndInstance:
                handlerMethod!.Invoke( dependencyPropertyField.Value, meta.Cast( declaringType, instanceExpr.Value ) );
                break;

            case ChangeHandlerParametersKind.StaticInstance:
                handlerMethod!.Invoke( meta.Cast( declaringType, instanceExpr.Value ) );
                break;
        }
    }

    [Template]
    private static dynamic? OverrideGetter(
        [CompileTime] IType propertyType,
        [CompileTime] IField dependencyPropertyField )
    {
        return meta.Cast( propertyType, meta.This.GetValue( dependencyPropertyField.Value ) );
    }

    [Template]
    private static void OverrideSetter(
        dynamic? value,
        [CompileTime] IField dependencyPropertyField )
    {
        meta.This.SetValue( dependencyPropertyField.Value, value );
    }

    [Template]
    private static void InitializeProperty(
        [CompileTime] IField dependencyPropertyField,
        [CompileTime] bool registerAsReadOnly,
        [CompileTime] string propertyName, 
        [CompileTime] INamedType propertyType,
        [CompileTime] INamedType declaringType,
        [CompileTime] IExpression? defaultValueExpr,
        [CompileTime] IMethod? onChangingHandlerMethod,
        [CompileTime] ChangeHandlerParametersKind onChangingHandlerParametersKind,
        [CompileTime] IMethod? onChangedHandlerMethod,
        [CompileTime] ChangeHandlerParametersKind onChangedHandlerParametersKind )
    {
        /* The PostSharp implementation:
         * 
         * - Uses the coercion callback for validation and notifying "changing"
         * - Always returns true for the validation callback
         * - Uses ValueChange callback to notify "changed"
         * 
         */

        if ( onChangingHandlerMethod != null || onChangedHandlerMethod != null || defaultValueExpr != null )
        {
            IExpression? coerceValueCallbackExpr = null;

            if ( onChangingHandlerMethod != null )
            {
#if ENABLE_DP_CALLBACKS
                var coerceValueCallback = (CoerceValueCallback) CoerceValue;

                coerceValueCallbackExpr = ExpressionFactory.Capture( coerceValueCallback );

                object CoerceValue( DependencyObject d, object value )
                {
                    // As per PostSharp implementation, this callback is used for validation and notifying "changing", throwing ArgumentException if invalid.
                    // NB: Validation (eg, a configured explicit validation method and/or integration with Contracts) is not yet implemented.

                    // TODO: Validation
                    // TODO: Integration with INotifyPropertyChanging

                    InvokeChangeHandler(
                        dependencyPropertyField,
                        onChangingHandlerMethod,
                        onChangingHandlerParametersKind,
                        propertyType,
                        declaringType,
                        ExpressionFactory.Capture( d ),
                        null,
                        ExpressionFactory.Capture( value ) );

                    return value;
                }
#endif
            }

            IExpression? propertyChangedCallbackExpr = null;

            if ( onChangedHandlerMethod != null )
            {
#if ENABLE_DP_CALLBACKS
                // TODO: Integration with INotifyPropertyChanged

                var propertyChangedCallback = (PropertyChangedCallback) PropertyChanged;

                propertyChangedCallbackExpr = ExpressionFactory.Capture( propertyChangedCallback );

                void PropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                    InvokeChangeHandler(
                        dependencyPropertyField,
                        onChangedHandlerMethod,
                        onChangedHandlerParametersKind,
                        propertyType,
                        declaringType,
                        ExpressionFactory.Capture( d ),
                        ExpressionFactory.Capture( e.OldValue ),
                        ExpressionFactory.Capture( e.NewValue ) );
                }
#endif
            }

            IExpression? metadataExpr = null;

            if ( defaultValueExpr != null && propertyChangedCallbackExpr != null && coerceValueCallbackExpr != null )
            {
                metadataExpr = ExpressionFactory.Capture( new PropertyMetadata( defaultValueExpr.Value, propertyChangedCallbackExpr.Value, coerceValueCallbackExpr.Value ) );
            }
            else if ( defaultValueExpr != null && propertyChangedCallbackExpr != null && coerceValueCallbackExpr == null )
            {
                metadataExpr = ExpressionFactory.Capture( new PropertyMetadata( defaultValueExpr.Value, propertyChangedCallbackExpr.Value ) );
            }
            else if ( defaultValueExpr != null && propertyChangedCallbackExpr == null && coerceValueCallbackExpr == null )
            {
                metadataExpr = ExpressionFactory.Capture( new PropertyMetadata( defaultValueExpr.Value ) );
            }
            else if ( defaultValueExpr == null && propertyChangedCallbackExpr != null && coerceValueCallbackExpr == null )
            {
                metadataExpr = ExpressionFactory.Capture( new PropertyMetadata( propertyChangedCallbackExpr.Value ) );
            }
            else
            {
                var metadata = new PropertyMetadata();

                if ( defaultValueExpr != null )
                {
                    metadata.DefaultValue = defaultValueExpr.Value;
                }

                if ( propertyChangedCallbackExpr != null )
                {
                    metadata.PropertyChangedCallback = propertyChangedCallbackExpr.Value;
                }

                if ( coerceValueCallbackExpr != null )
                {
                    metadata.CoerceValueCallback = coerceValueCallbackExpr.Value;
                }

                metadataExpr = ExpressionFactory.Capture( metadata );
            }

            if ( registerAsReadOnly )
            {
                dependencyPropertyField.Value = DependencyProperty.RegisterReadOnly(
                    propertyName,
                    propertyType.ToTypeOfExpression().Value,
                    declaringType.ToTypeOfExpression().Value,
                    metadataExpr.Value )
                    .DependencyProperty;
            }
            else
            {
                dependencyPropertyField.Value = DependencyProperty.Register(
                    propertyName,
                    propertyType.ToTypeOfExpression().Value,
                    declaringType.ToTypeOfExpression().Value,
                    metadataExpr.Value );
            }
        }
        else
        {
            if ( registerAsReadOnly )
            {
                dependencyPropertyField.Value = DependencyProperty.RegisterReadOnly(
                    propertyName,
                    propertyType.ToTypeOfExpression().Value,
                    declaringType.ToTypeOfExpression().Value,
                    null )
                    .DependencyProperty;
            }
            else
            {
                dependencyPropertyField.Value = DependencyProperty.Register(
                    propertyName,
                    propertyType.ToTypeOfExpression().Value,
                    declaringType.ToTypeOfExpression().Value );
            }
        }
    }
}