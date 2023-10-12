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
    /// Gets a value indicating whether the property initializer (if present) should be set as the initial value of the <see cref="DependencyProperty"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool? SetInitialValueFromInitializer { get; init; }

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (DependencyPropertyOptions) changes;

        return new DependencyPropertyOptions
        {
            IsReadOnly = other.IsReadOnly ?? this.IsReadOnly,
            SetInitialValueFromInitializer = other.SetInitialValueFromInitializer ?? this.SetInitialValueFromInitializer
        };
    }

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( OptionsInitializationContext context )
    {
        return new DependencyPropertyOptions()
        {
            IsReadOnly = false,
            SetInitialValueFromInitializer = true
        };
    }
}