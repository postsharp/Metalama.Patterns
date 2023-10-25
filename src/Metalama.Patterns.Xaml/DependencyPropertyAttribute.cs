// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Options;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Property )]
public sealed class DependencyPropertyAttribute : Attribute, IAspect<IProperty>, IHierarchicalOptionsProvider
{
    private bool? _isReadOnly;

    /// <summary>
    /// Gets or sets a value indicating whether the property should be registered as a read-only property.
    /// </summary>
    public bool IsReadOnly
    {
        get => this._isReadOnly ?? false;
        set => this._isReadOnly = value;
    }

    private bool? _initializerProvidesInitialValue;

    /// <summary>
    /// Gets or sets a value indicating whether the property initializer (if present) should be used to set the initial value of the <see cref="DependencyProperty"/>
    /// in the instance constructor of the declaring class of the target property. The default is <see langword="false"/>.
    /// </summary>
    public bool InitializerProvidesInitialValue
    {
        get => this._initializerProvidesInitialValue ?? false;
        set => this._initializerProvidesInitialValue = value;
    }

    private bool? _initializerProvidesDefaultValue;

    /// <summary>
    /// Gets or sets a value indicating whether the property initializer (if present) should be used to for <see cref="PropertyMetadata.DefaultValue"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool InitializerProvidesDefaultValue
    {
        get => this._initializerProvidesDefaultValue ?? true;
        set => this._initializerProvidesDefaultValue = value;
    }

    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
    {
        return new[]
        {
            new DependencyPropertyOptions()
            {
                IsReadOnly = this._isReadOnly,
                InitializerProvidesInitialValue = this._initializerProvidesInitialValue,
                InitializerProvidesDefaultValue = this._initializerProvidesDefaultValue
            }
        };
    }

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

    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        // TODO: Aspect tests for eligibility.

        builder.MustNotBeStatic();
        
        // ReSharper disable once RedundantNameQualifier
        builder.MustHaveAccessibility( Framework.Code.Accessibility.Public );
        builder.MustBeReadable();
        builder.MustSatisfy( p => p.IsAutoPropertyOrField == true, p => $"{p} must be an auto-property." );
        builder.DeclaringType().MustBe( typeof(DependencyObject), ConversionKind.Reference );
    }

    void IAspect<IProperty>.BuildAspect( IAspectBuilder<IProperty> builder )
    {
        var aspectBuilder = new DependencyPropertyAspectBuilder( builder, this );
        aspectBuilder.Build();
    }
}