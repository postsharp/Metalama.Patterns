// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Investigating compile time exception inside Metalma. Remove conditions when resolved.
#define ENABLE_LOCALFUNCTIONS_WORKAROUND_WA1_WA2

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Patterns.Xaml.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml.Implementation;

internal sealed partial class DependencyPropertyAspectBuilder
{
    private sealed class Templates : ITemplateProvider
    {
        private Templates() { }

        public static TemplateProvider Provider { get; } = TemplateProvider.FromInstance( new Templates() );

        [Template]
        private static void InvokeChangeHandler(
            [CompileTime] IField dependencyPropertyField,
            [CompileTime] IMethod handlerMethod,
            [CompileTime] ChangeHandlerSignatureKind parametersKind,
            [CompileTime] INamedType propertyType,
            [CompileTime] INamedType declaringType,
            [CompileTime] IExpression instanceExpr,
            [CompileTime] IExpression? oldValueExpr,
            [CompileTime] IExpression newValueExpr )
        {
            if ( handlerMethod.TypeParameters.Count == 1 )
            {
                handlerMethod = handlerMethod.WithTypeArguments( propertyType );
            }

            switch ( parametersKind )
            {
                // TODO: Some casts may not be required depending on the type of the method parameter.

                case ChangeHandlerSignatureKind.InstanceNoParameters:
                    handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) ).Invoke();

                    break;

                case ChangeHandlerSignatureKind.StaticNoParameters:
                    handlerMethod!.Invoke();

                    break;

                case ChangeHandlerSignatureKind.InstanceValue:
                    handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                        .Invoke( handlerMethod.Parameters[0].Type.SpecialType == SpecialType.Object ? newValueExpr.Value : meta.Cast( propertyType, newValueExpr.Value ) );

                    break;

                case ChangeHandlerSignatureKind.InstanceOldValueAndNewValue:
                    handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                        .Invoke(
                            handlerMethod.Parameters[0].Type.SpecialType == SpecialType.Object ? oldValueExpr!.Value : meta.Cast( propertyType, oldValueExpr!.Value ),
                            handlerMethod.Parameters[1].Type.SpecialType == SpecialType.Object ? newValueExpr.Value : meta.Cast( propertyType, newValueExpr.Value ) );
                    break;

                case ChangeHandlerSignatureKind.InstanceDependencyProperty:
                    handlerMethod!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) ).Invoke( dependencyPropertyField.Value );

                    break;

                case ChangeHandlerSignatureKind.StaticDependencyProperty:
                    handlerMethod!.Invoke( dependencyPropertyField.Value );

                    break;

                case ChangeHandlerSignatureKind.StaticDependencyPropertyAndInstance:
                    handlerMethod!.Invoke( dependencyPropertyField.Value, meta.Cast( declaringType, instanceExpr.Value ) );

                    break;

                case ChangeHandlerSignatureKind.StaticInstance:
                    handlerMethod!.Invoke( meta.Cast( declaringType, instanceExpr.Value ) );

                    break;
            }
        }

        [Template]
        internal static dynamic? OverrideGetter(
            [CompileTime] IType propertyType,
            [CompileTime] IField dependencyPropertyField )
        {
            return meta.Cast( propertyType, meta.This.GetValue( dependencyPropertyField.Value ) );
        }

        [Template]
        internal static void OverrideSetter(
            dynamic? value,
            [CompileTime] IField dependencyPropertyField )
        {
            meta.This.SetValue( dependencyPropertyField.Value, value );
        }

        [Template]
        internal static void InitializeDependencyProperty(
            [CompileTime] IField dependencyPropertyField,
            [CompileTime] DependencyPropertyOptions options,
            [CompileTime] string propertyName,
            [CompileTime] INamedType propertyType,
            [CompileTime] INamedType declaringType,
            [CompileTime] IExpression? defaultValueExpr,
            [CompileTime] IMethod? onChangingHandlerMethod,
            [CompileTime] ChangeHandlerSignatureKind onChangingHandlerParametersKind,
            [CompileTime] IMethod? onChangedHandlerMethod,
            [CompileTime] ChangeHandlerSignatureKind onChangedHandlerParametersKind )
        {
            /* The PostSharp implementation:
             * 
             * - Uses the coercion callback for validation and notifying "changing"
             * - Always returns true for the validation callback
             * - Uses PropertyChanged callback to notify "changed"
             * 
             */

            if ( options.InitializerProvidesDefaultValue != true )
            {
                defaultValueExpr = null;
            }

            if ( onChangingHandlerMethod != null || onChangedHandlerMethod != null || defaultValueExpr != null )
            {
                IExpression? coerceValueCallbackExpr = null;

#if ENABLE_LOCALFUNCTIONS_WORKAROUND_WA1_WA2
                // TODO: Remove workaround [WA1]
                // The method below should be inside the 'if' block below (see [WA1]), and the outer 'if' in the method should be removed.
                object CoerceValue( DependencyObject d, object value )
                {
                    // As per PostSharp implementation, this callback is used for validation and notifying "changing", throwing ArgumentException if invalid.
                    // NB: Validation (eg, a configured explicit validation method and/or integration with Contracts) is not yet implemented.

                    // TODO: Validation
                    // TODO: Integration with INotifyPropertyChanging

                    if ( onChangingHandlerMethod != null )
                    {
                        InvokeChangeHandler(
                            dependencyPropertyField,
                            onChangingHandlerMethod,
                            onChangingHandlerParametersKind,
                            propertyType,
                            declaringType,
                            ExpressionFactory.Capture( d ),
                            null,
                            ExpressionFactory.Capture( value ) );
                    }

                    return value;
                }
#endif

                if ( onChangingHandlerMethod != null )
                {
                    coerceValueCallbackExpr = ExpressionFactory.Capture( (CoerceValueCallback) CoerceValue );

                    // TODO: Remove workaround [WA1] (see above)
#if !ENABLE_LOCALFUNCTIONS_WORKAROUND_WA1_WA2
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

#if ENABLE_LOCALFUNCTIONS_WORKAROUND_WA1_WA2
                // TODO: Remove workaround [WA2]
                // The method below should be inside the 'if' block below (see [WA2]), and the outer 'if' in the method should be removed.
                void PropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
                {
                    if ( onChangedHandlerMethod != null )
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
                }
#endif

                if ( onChangedHandlerMethod != null )
                {
                    // TODO: Integration with INotifyPropertyChanged

                    propertyChangedCallbackExpr = ExpressionFactory.Capture( (PropertyChangedCallback) PropertyChanged );

                    // TODO: Remove workaround [WA2] (see above)
#if !ENABLE_LOCALFUNCTIONS_WORKAROUND_WA1_WA2
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

                if ( defaultValueExpr != null && defaultValueExpr.Type.SpecialType != SpecialType.Object )
                {
                    // Cast to ensure that the correct overload of the PropertyMetadata ctor is used.

                    defaultValueExpr = (IExpression?) meta.Cast( TypeFactory.GetType( SpecialType.Object ), defaultValueExpr.Value );
                }

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

                if ( options.IsReadOnly == true )
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
                if ( options.IsReadOnly == true )
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

        [Template]
        internal static void Assign(
            [CompileTime] IExpression left,
            [CompileTime] IExpression right )
        {
            left.Value = right.Value;
        }
    }
}