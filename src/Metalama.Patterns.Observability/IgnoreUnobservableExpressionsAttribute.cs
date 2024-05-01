// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Options;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability;

/// <summary>
/// Custom attribute that indicates that the target element (or its contained members, when applicable) is safe to access and for which no dependency analysis is required.
/// </summary>
[PublicAPI]
[AttributeUsage( AttributeTargets.Property )]
public sealed class IgnoreUnobservableExpressionsAttribute : Attribute, IHierarchicalOptionsProvider
{
    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
    {
        return new[] { new DependencyAnalysisOptions() { IgnoreUnobservableExpressions = true } };
    }
}