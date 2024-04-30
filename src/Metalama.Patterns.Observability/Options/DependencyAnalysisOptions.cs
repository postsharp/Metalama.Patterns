﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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
    public bool? IgnoreUnsupportedDependencies { get; init; }

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
    {
        var other = (DependencyAnalysisOptions) changes;

        return new DependencyAnalysisOptions { IgnoreUnsupportedDependencies = other.IgnoreUnsupportedDependencies ?? this.IgnoreUnsupportedDependencies };
    }

    IHierarchicalOptions IHierarchicalOptions.GetDefaultOptions( OptionsInitializationContext context )
        => new DependencyAnalysisOptions() { IgnoreUnsupportedDependencies = false };
}