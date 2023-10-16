// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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
            [CompileTime] IMethod? onChangingMethod,
            [CompileTime] ChangeHandlerSignatureKind onChangingSignatureKind,
            [CompileTime] IMethod? onChangedMethod,
            [CompileTime] ChangeHandlerSignatureKind onChangedSignatureKind,
            [CompileTime] IMethod? validateMethod,
            [CompileTime] ValidationHandlerSignatureKind validateSignatureKind )
        {
            /* The PostSharp implementation:
             * 
             * - Uses the coercion callback for validation and notifying "changing"
             * - Appears to always return true for the validation callback
             * - Uses PropertyChanged callback to notify "changed"
             * 
             */

            if ( options.InitializerProvidesDefaultValue != true )
            {
                defaultValueExpr = null;
            }

            if ( onChangingMethod != null || onChangedMethod != null || validateMethod != null || defaultValueExpr != null )
            {
                IExpression? coerceValueCallbackExpr = null;

                if ( onChangingMethod != null || validateMethod != null )
                {
                    // As per PostSharp implementation, this callback is used for validation and notifying "changing".

                    // TODO: Integration with Contracts
                    // TODO: Integration with INotifyPropertyChanging

                    object CoerceValue( DependencyObject d, object value )
                    {
                        if ( validateMethod != null )
                        {
                            InvokeValidateMethod(
                                dependencyPropertyField,
                                validateMethod,
                                validateSignatureKind,
                                propertyType,
                                declaringType,
                                ExpressionFactory.Capture( d ),
                                ExpressionFactory.Capture( value ) );
                        }

                        if ( onChangingMethod != null )
                        {
                            InvokeChangeMethod(
                                dependencyPropertyField,
                                onChangingMethod,
                                onChangingSignatureKind,
                                propertyType,
                                declaringType,
                                ExpressionFactory.Capture( d ),
                                null,
                                ExpressionFactory.Capture( value ) );
                        }

                        return value;
                    }

                    coerceValueCallbackExpr = ExpressionFactory.Capture( (CoerceValueCallback) CoerceValue );
                }

                IExpression? propertyChangedCallbackExpr = null;

                if ( onChangedMethod != null )
                {
                    // TODO: Integration with NotifyPropertyChanged

                    void PropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
                    {
                        InvokeChangeMethod(
                            dependencyPropertyField,
                            onChangedMethod,
                            onChangedSignatureKind,
                            propertyType,
                            declaringType,
                            ExpressionFactory.Capture( d ),
                            ExpressionFactory.Capture( e.OldValue ),
                            ExpressionFactory.Capture( e.NewValue ) );
                    }

                    propertyChangedCallbackExpr = ExpressionFactory.Capture( (PropertyChangedCallback) PropertyChanged );
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
        private static void InvokeValidateMethod(
            [CompileTime] IField dependencyPropertyField,
            [CompileTime] IMethod method,
            [CompileTime] ValidationHandlerSignatureKind signatureKind,
            [CompileTime] INamedType propertyType,
            [CompileTime] INamedType declaringType,
            [CompileTime] IExpression instanceExpr,
            [CompileTime] IExpression valueExpr )
        {
            if ( method.TypeParameters.Count == 1 )
            {
                method = method.WithTypeArguments( propertyType );
            }

            switch ( signatureKind )
            {
                case ValidationHandlerSignatureKind.InstanceValue:
                    if ( !method!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                            .Invoke( method.Parameters[0].Type.SpecialType == SpecialType.Object ? valueExpr.Value : meta.Cast( propertyType, valueExpr.Value ) ) )
                    {
                        throw new ArgumentException( "Invalid property value.", "value" );
                    }

                    break;

                case ValidationHandlerSignatureKind.InstanceDependencyPropertyAndValue:
                    if ( !method!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                            .Invoke(
                                dependencyPropertyField.Value,
                                method.Parameters[1].Type.SpecialType == SpecialType.Object ? valueExpr.Value : meta.Cast( propertyType, valueExpr.Value ) ) )
                    {
                        throw new ArgumentException( "Invalid property value.", "value" );
                    }

                    break;

                case ValidationHandlerSignatureKind.StaticValue:
                    if ( !method!.Invoke( method.Parameters[0].Type.SpecialType == SpecialType.Object ? valueExpr.Value : meta.Cast( propertyType, valueExpr.Value ) ) )
                    {
                        throw new ArgumentException( "Invalid property value.", "value" );
                    }

                    break;

                case ValidationHandlerSignatureKind.StaticDependencyPropertyAndValue:
                    if ( !method!.Invoke(
                            dependencyPropertyField.Value,
                            method.Parameters[1].Type.SpecialType == SpecialType.Object ? valueExpr.Value : meta.Cast( propertyType, valueExpr.Value ) ) )
                    {
                        throw new ArgumentException( "Invalid property value.", "value" );
                    }

                    break;

                case ValidationHandlerSignatureKind.StaticDependencyPropertyAndInstanceAndValue:
                    if ( !method!.Invoke(
                            dependencyPropertyField.Value,
                            instanceExpr.Type.Is( method.Parameters[1].Type, ConversionKind.Reference ) ? instanceExpr.Value : meta.Cast( declaringType, instanceExpr.Value ),
                            method.Parameters[2].Type.SpecialType == SpecialType.Object ? valueExpr.Value : meta.Cast( propertyType, valueExpr.Value ) ) )
                    {
                        throw new ArgumentException( "Invalid property value.", "value" );
                    }

                    break;

                case ValidationHandlerSignatureKind.StaticInstanceAndValue:
                    if ( !method!.Invoke(
                            instanceExpr.Type.Is( method.Parameters[0].Type, ConversionKind.Reference ) ? instanceExpr.Value : meta.Cast( declaringType, instanceExpr.Value ),
                            method.Parameters[1].Type.SpecialType == SpecialType.Object ? valueExpr.Value : meta.Cast( propertyType, valueExpr.Value ) ) )
                    {
                        throw new ArgumentException( "Invalid property value.", "value" );
                    }

                    break;
            }
        }

        [Template]
        private static void InvokeChangeMethod(
            [CompileTime] IField dependencyPropertyField,
            [CompileTime] IMethod method,
            [CompileTime] ChangeHandlerSignatureKind signatureKind,
            [CompileTime] INamedType propertyType,
            [CompileTime] INamedType declaringType,
            [CompileTime] IExpression instanceExpr,
            [CompileTime] IExpression? oldValueExpr,
            [CompileTime] IExpression newValueExpr )
        {
            if ( method.TypeParameters.Count == 1 )
            {
                method = method.WithTypeArguments( propertyType );
            }

            switch ( signatureKind )
            {
                case ChangeHandlerSignatureKind.InstanceNoParameters:
                    method!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) ).Invoke();

                    break;

                case ChangeHandlerSignatureKind.StaticNoParameters:
                    method!.Invoke();

                    break;

                case ChangeHandlerSignatureKind.InstanceValue:
                    method!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                        .Invoke( method.Parameters[0].Type.SpecialType == SpecialType.Object ? newValueExpr.Value : meta.Cast( propertyType, newValueExpr.Value ) );

                    break;

                case ChangeHandlerSignatureKind.InstanceOldValueAndNewValue:
                    method!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) )
                        .Invoke(
                            method.Parameters[0].Type.SpecialType == SpecialType.Object ? oldValueExpr!.Value : meta.Cast( propertyType, oldValueExpr!.Value ),
                            method.Parameters[1].Type.SpecialType == SpecialType.Object ? newValueExpr.Value : meta.Cast( propertyType, newValueExpr.Value ) );
                    break;

                case ChangeHandlerSignatureKind.InstanceDependencyProperty:
                    method!.With( (IExpression?) meta.Cast( declaringType, instanceExpr.Value ) ).Invoke( dependencyPropertyField.Value );

                    break;

                case ChangeHandlerSignatureKind.StaticDependencyProperty:
                    method!.Invoke( dependencyPropertyField.Value );

                    break;

                case ChangeHandlerSignatureKind.StaticDependencyPropertyAndInstance:
                    method!.Invoke( 
                        dependencyPropertyField.Value,
                        instanceExpr.Type.Is( method.Parameters[1].Type, ConversionKind.Reference ) ? instanceExpr.Value : meta.Cast( declaringType, instanceExpr.Value ) );

                    break;

                case ChangeHandlerSignatureKind.StaticInstance:
                    method!.Invoke(
                        instanceExpr.Type.Is( method.Parameters[0].Type, ConversionKind.Reference ) ? instanceExpr.Value : meta.Cast( declaringType, instanceExpr.Value ) );

                    break;
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