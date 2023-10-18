// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using System.Windows;

namespace Metalama.Patterns.Xaml.Options;

[PublicAPI]
[CompileTime]
public sealed class DependencyPropertyOptionsBuilder
{
    /// <summary>
    /// Gets or sets a value indicating whether the property should be registered as a read-only property.
    /// </summary>
    public bool? IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the property initializer (if present) should be used to set the initial value of the <see cref="DependencyProperty"/>
    /// in the instance constructor of the declaring class of the target property. The default is <see langword="false"/>.
    /// </summary>
    public bool? InitializerProvidesInitialValue { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the property initializer (if present) should be used to for <see cref="PropertyMetadata.DefaultValue"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool? InitializerProvidesDefaultValue { get; set; }

    // TODO: Document the valid signatures of PropertyChangedMethod, PropertyChangingMethod and ValidateMethod, see project README.md.

    /// <summary>
    /// Gets or sets the name of the method that will be called when the the property value has changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The method must be declared in the same class as the target property.
    /// </para>
    /// <para>
    /// If this property is not set then the default <c>OnFooChanged</c> value is used, where <c>Foo</c> is the name of the target property.
    /// </para>
    /// </remarks>
    public string? PropertyChangedMethod { get; set; }

    /// <summary>
    /// Gets or sets the name of the method that will be called when the property value is about to change.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The method must be declared in the same class as the target property.
    /// </para>
    /// <para>
    /// If this property is not set then the default <c>OnFooChanging</c> value is used, where <c>Foo</c> is the name of the target property.
    /// </para>
    /// </remarks>
    public string? PropertyChangingMethod { get; set; }

    /// <summary>
    /// Gets or sets the name of the method that validates the value of the property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The method must be declared in the same class as the target property.
    /// </para>
    /// <para>
    /// If this property is not set then the default <c>ValidateFoo</c> value is used, where <c>Foo</c> is the name of the target property.
    /// </para>
    /// </remarks>
    public string? ValidateMethod { get; set; }

    /// <summary>
    /// Gets or sets the name of the static readonly field that will be generated to expose the instance of the registered <see cref="DependencyProperty"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this property is not set then the default <c>FooProperty</c> value is used, where <c>Foo</c> is the name of the target property.
    /// </para>
    /// </remarks>
    public string? RegistrationField { get; set; }

    internal DependencyPropertyOptions Build()
        => new()
        {
            IsReadOnly = this.IsReadOnly,
            InitializerProvidesInitialValue = this.InitializerProvidesInitialValue,
            InitializerProvidesDefaultValue = this.InitializerProvidesDefaultValue,
            PropertyChangedMethod = this.PropertyChangedMethod,
            PropertyChangingMethod = this.PropertyChangingMethod,
            ValidateMethod = this.ValidateMethod,
            RegistrationField = this.RegistrationField
        };
}