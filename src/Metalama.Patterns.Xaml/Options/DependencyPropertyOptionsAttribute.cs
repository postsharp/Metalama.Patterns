// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml.Options;

[RunTimeOrCompileTime]
[AttributeUsage( AttributeTargets.Assembly | AttributeTargets.Class )]
public class DependencyPropertyOptionsAttribute : Attribute, IHierarchicalOptionsProvider
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

    private bool? _setInitialValueFromInitializer;

    /// <summary>
    /// Gets or sets a value indicating whether the object provided by the property initializer (if present) should be set as the initial value of the <see cref="DependencyProperty"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool SetInitialValueFromInitializer
    {
        get => this._setInitialValueFromInitializer ?? true;
        set => this._setInitialValueFromInitializer = value;
    }

    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
    {
        return new[]
        {
            new DependencyPropertyOptions() { IsReadOnly = this._isReadOnly, SetInitialValueFromInitializer = this._setInitialValueFromInitializer }
        };
    }
}