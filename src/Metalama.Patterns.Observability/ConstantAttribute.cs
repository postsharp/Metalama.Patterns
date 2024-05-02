// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Options;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability;

/// <summary>
/// Adds an observability contract that guarantees that the outputs of the member (1) do depend on any non-constant
/// input (i.e. will always be identical given identical inputs) and (2) are themselves constant.
/// When applied to a type, the guarantee must hold for all methods and properties.
/// </summary>
[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class )]
public sealed class ConstantAttribute : Attribute, IHierarchicalOptionsProvider
{
    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
        => new[] { new DependencyAnalysisOptions() { ObservabilityContract = ObservabilityContract.Constant } };
}