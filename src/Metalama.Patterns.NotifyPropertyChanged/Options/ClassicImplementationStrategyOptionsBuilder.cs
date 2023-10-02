// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Options;

[PublicAPI]
[CompileTime]
public sealed class ClassicImplementationStrategyOptionsBuilder
{
    private ClassicImplementationStrategyOptions _options = new();

    /// <summary>
    /// Sets a value indicating whether the <c>OnUnmonitoredObservablePropertyChanged</c> method should be introduced.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="ClassicImplementationStrategyOptions.EnableOnUnmonitoredObservablePropertyChangedMethod"/>
    /// </remarks>
    public bool EnableOnUnmonitoredObservablePropertyChangedMethod
    {
        // ReSharper disable once WithExpressionModifiesAllMembers
        set => this._options = this._options with { EnableOnUnmonitoredObservablePropertyChangedMethod = value };
    }

    public ClassicImplementationStrategyOptions Build() => this._options;
}