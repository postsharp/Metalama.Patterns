// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Observability.Implementation;

namespace Metalama.Patterns.Observability.Options;

[PublicAPI]
[CompileTime]
public sealed class ObservabilityOptionsBuilder
{
    private ObservabilityOptions _options = new();

    /// <summary>
    /// Sets the <see cref="IImplementationStrategyFactory"/> used to provide <see cref="IImplementationStrategyBuilder"/> instances.
    /// </summary>
    internal IImplementationStrategyFactory ImplementationStrategyFactory
    {
        set => this._options = this._options with { ImplementationStrategyFactory = value };
    }

    #if DEBUG
    /// <summary>
    /// Sets a value indicating the verbosity of diagnostic comments inserted into generated code. Must be a value
    /// between 0 and 3 (inclusive). 0 (default) inserts no comments, 3 is the most verbose.
    /// </summary>
    public int? DiagnosticCommentVerbosity
    {
        set => this._options = this._options with { DiagnosticCommentVerbosity = value };
    }
    #endif

    internal ObservabilityOptions Build() => this._options;
}