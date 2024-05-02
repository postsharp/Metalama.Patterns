// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Options;
using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability;

/// <summary>
/// Adds an observability contract that guarantees that the member (1) does not depend on any non-constant
/// inputs or factors (i.e. will always return the same output given the same input) and (2) does not return non-constant outputs
/// (i.e. even the properties of the outputs are constant). When applied to a type, the guarantee must hold for all methods and properties.
/// </summary>
[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Struct | AttributeTargets.Class )]
public sealed class ConstantAttribute : Attribute, IHierarchicalOptionsProvider
{
    IEnumerable<IHierarchicalOptions> IHierarchicalOptionsProvider.GetOptions( in OptionsProviderContext context )
        => new[] { new DependencyAnalysisOptions() { ObservabilityContract = ObservabilityContract.Constant } };
}