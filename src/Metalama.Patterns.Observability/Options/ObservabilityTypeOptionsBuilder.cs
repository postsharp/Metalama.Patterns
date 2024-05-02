// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Implementation;

namespace Metalama.Patterns.Observability.Options;

/// <summary>
/// Builds dependency options. Used at the level of <see cref="ICompilation"/>, <see cref="INamespace"/> and <see cref="INamedType"/>.
/// </summary>
[PublicAPI]
[CompileTime]
public sealed class ObservabilityTypeOptionsBuilder
{
    internal ObservabilityOptions? ObservabilityOptions { get; private set; }

    internal ClassicObservabilityStrategyOptions? ClassicStrategyOptions { get; private set; }

    internal DependencyAnalysisOptions? DependencyAnalysisOptions { get; private set; }

    /// <summary>
    /// Gets or sets the <see cref="IObservabilityStrategy"/> used to implement the <see cref="ObservableAttribute"/> aspect.
    /// </summary>
    public IObservabilityStrategy? ObservabilityStrategy
    {
        get => this.ObservabilityOptions?.ImplementationStrategy;
        set => this.ObservabilityOptions = (this.ObservabilityOptions ?? new ObservabilityOptions()) with { ImplementationStrategy = value };
    }

    /// <summary>
    /// Gets or sets a value whether observability warnings in the target members must be suppressed.
    /// </summary>
    public bool? SuppressWarnings
    {
        get => this.DependencyAnalysisOptions?.SuppressWarnings;
        set
            => this.DependencyAnalysisOptions =
                new DependencyAnalysisOptions { SuppressWarnings = value };
    }

    /// <summary>
    /// Gets or sets an <see cref="ObservabilityContract"/> for the target member, guaranteeing its behavior
    /// with respect to the <see cref="ObservableAttribute"/> aspect.
    /// </summary>
    public ObservabilityContract? ObservabilityContract
    {
        get => this.DependencyAnalysisOptions?.ObservabilityContract;
        set
            => this.DependencyAnalysisOptions =
                new DependencyAnalysisOptions { ObservabilityContract = value };
    }

#if DEBUG
    /// <summary>
    /// Sets a value indicating the verbosity of diagnostic comments inserted into generated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int? DiagnosticCommentVerbosity
    {
        set => this.ObservabilityOptions = (this.ObservabilityOptions ?? new ObservabilityOptions()) with { DiagnosticCommentVerbosity = value };
    }
#endif

    /// <summary>
    /// Configures <c>Metalama.Patterns.Observability.Implementation.ClassicStrategy</c> for the current project.
    /// </summary>
    /// <param name="configure">A delegate that configures the strategy.</param>
    public void ConfigureClassicStrategy( Action<ClassicObservabilityStrategyOptionsBuilder> configure )
    {
        var builder = new ClassicObservabilityStrategyOptionsBuilder( this.ClassicStrategyOptions );
        configure( builder );

        this.ClassicStrategyOptions = builder.Options;
    }
}