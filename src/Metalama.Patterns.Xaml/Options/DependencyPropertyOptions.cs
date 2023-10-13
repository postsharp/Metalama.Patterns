// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Code;
using Metalama.Framework.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml.Options;

internal sealed record DependencyPropertyOptions : IHierarchicalOptions<ICompilation>, IHierarchicalOptions<INamespace>, IHierarchicalOptions<INamedType>, IHierarchicalOptions<IProperty>
{
    /// <summary>
    /// Gets a value indicating whether the property should be registered as a read-only property.
    /// </summary>
    public bool? IsReadOnly { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property initializer (if present) should be used to set the initial value of the <see cref="DependencyProperty"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool? InitializerProvidesInitialValue { get; init; }

    /// <summary>
    /// Gets a value indicating whether the property initializer (if present) should be used to for <see cref="PropertyMetadata.DefaultValue"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool? InitializerProvidesDefaultValue { get; init; }

    // TODO: Document the valid signatures of PropertyChangedMethod and PropertyChangingMethod, see project README.md.

    /// <summary>
    /// Gets the name of the method that will be called when the the property value has changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>OnPropertyChanged</c> method must be declared in the same class as the target property.
    /// </para>
    /// <para>
    /// If this property is not set then the default <c>OnFooChanged</c> value is used, where <c>Foo</c> is the name of the target property.
    /// </para>
    /// </remarks>
    public string? PropertyChangedMethod { get; init; }

    /// <summary>
    /// Gets the name of the method that reacts to the changes of the property value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>OnPropertyChanged</c> method must be declared in the same class as the target property.
    /// </para>
    /// <para>
    /// If this property is not set then the default <c>OnFooChanging</c> value is used, where <c>Foo</c> is the name of the target property.
    /// </para>
    /// </remarks>
    public string? PropertyChangingMethod { get; init; }

    /// <summary>
    /// Gets the name of the static readonly field that will be generated to expose the instance of the registered <see cref="DependencyProperty"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this property is not set then the default <c>FooProperty</c> value is used, where <c>Foo</c> is the name of the target property.
    /// </para>
    /// </remarks>
    public string? RegistrationField { get; init; }

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( OptionsInitializationContext context )
    {
        return new DependencyPropertyOptions()
        {
            IsReadOnly = false,
            InitializerProvidesInitialValue = true,
            InitializerProvidesDefaultValue = true
        };
    }

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (DependencyPropertyOptions) changes;

        return new DependencyPropertyOptions
        {
            IsReadOnly = other.IsReadOnly ?? this.IsReadOnly,
            InitializerProvidesInitialValue = other.InitializerProvidesInitialValue ?? this.InitializerProvidesInitialValue,
            InitializerProvidesDefaultValue = other.InitializerProvidesDefaultValue ?? this.InitializerProvidesDefaultValue,
            PropertyChangedMethod = other.PropertyChangedMethod ?? this.PropertyChangedMethod,
            PropertyChangingMethod = other.PropertyChangingMethod ?? this.PropertyChangingMethod,
            RegistrationField = other.RegistrationField ?? this.RegistrationField,
        };
    }
}