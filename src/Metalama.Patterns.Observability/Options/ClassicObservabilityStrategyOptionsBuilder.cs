// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Options;

[PublicAPI]
[CompileTime]
public sealed class ClassicObservabilityStrategyOptionsBuilder
{
    internal ClassicObservabilityStrategyOptions? Options { get; private set; }

    internal ClassicObservabilityStrategyOptionsBuilder( ClassicObservabilityStrategyOptions? options )
    {
        this.Options = options;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>OnObservablePropertyChanged</c> method should be introduced.
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="ClassicObservabilityStrategyOptions.EnableOnObservablePropertyChangedMethod"/>
    /// </remarks>
    public bool? EnableOnObservablePropertyChangedMethod
    {
        get => this.Options?.EnableOnObservablePropertyChangedMethod;
        
        // ReSharper disable once WithExpressionModifiesAllMembers
        set => this.Options = (this.Options ?? new ClassicObservabilityStrategyOptions()) with { EnableOnObservablePropertyChangedMethod = value };
    }
}