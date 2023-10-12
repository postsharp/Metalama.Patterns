// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using System.Windows;

namespace Metalama.Patterns.Xaml.Options;

[PublicAPI]
[CompileTime]
public sealed class DependencyPropertyOptionsBuilder
{
    private DependencyPropertyOptions _options = new();

    /// <summary>
    /// Sets a value indicating whether the property should be registered as a read-only property.
    /// </summary>
    public bool? IsReadOnly
    {
        set => this._options = this._options with { IsReadOnly = value };
    }

    /// <summary>
    /// Sets a value indicating whether the property initializer (if present) should be set as the initial value of the <see cref="DependencyProperty"/>.
    /// The default is <see langword="true"/>.
    /// </summary>
    public bool? SetInitialValueFromInitializer
    {
        set => this._options = this._options with { SetInitialValueFromInitializer = value };
    }

    internal DependencyPropertyOptions Build() => this._options;
}