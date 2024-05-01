// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Code;
using Metalama.Framework.Options;

namespace Metalama.Patterns.Observability.Options;

[PublicAPI]
public sealed record DependencyAnalysisOptions :
    IHierarchicalOptions<ICompilation>,
    IHierarchicalOptions<INamespace>,
    IHierarchicalOptions<INamedType>,
    IHierarchicalOptions<IMember>
{
    public bool? IgnoreUnobservableExpressions { get; init; }

    public ObservabilityContract? ObservabilityContract { get; init; }

    public bool? IsDeeplyImmutableType { get; init; }

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (DependencyAnalysisOptions) changes;

        return new DependencyAnalysisOptions
        {
            IgnoreUnobservableExpressions = other.IgnoreUnobservableExpressions ?? this.IgnoreUnobservableExpressions,
            ObservabilityContract = other.ObservabilityContract ?? this.ObservabilityContract,
            IsDeeplyImmutableType = other.IsDeeplyImmutableType ?? this.IsDeeplyImmutableType
        };
    }

    internal static DependencyAnalysisOptions Default { get; } = new()
    {
        IgnoreUnobservableExpressions = false

        // Other members intentionally null by default because we could have some rules given the default.
    };

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( OptionsInitializationContext context ) => Default;
}