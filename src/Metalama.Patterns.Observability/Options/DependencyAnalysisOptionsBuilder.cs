// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Options;

[PublicAPI]
[CompileTime]
public sealed class DependencyAnalysisOptionsBuilder
{
    private DependencyAnalysisOptions _options = new();

    public bool IsSafeToCall
    {
        // ReSharper disable once WithExpressionModifiesAllMembers
        set => this._options = this._options with { IsSafe = value };
    }

    public DependencyAnalysisOptions Build() => this._options;
}