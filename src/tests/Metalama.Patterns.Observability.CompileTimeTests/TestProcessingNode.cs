// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Observability.Implementation;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

namespace Metalama.Patterns.Observability.CompileTimeTests;

/// <summary>
/// An unspecialized <see cref="ProcessingNode{TDerived, TReadOnlyDerivedInterface}"/> used for unit tests.
/// </summary>
[UsedImplicitly] // TODO: Check.
[CompileTime]
internal sealed class TestProcessingNode : ProcessingNode<TestProcessingNode, IReadOnlyTestProcessingNode>, IReadOnlyTestProcessingNode
{
    IReadOnlyCollection<IReadOnlyTestProcessingNode> IDependencyNode<IReadOnlyTestProcessingNode>.Children => this.Children;

    IReadOnlyTestProcessingNode IDependencyNode<IReadOnlyTestProcessingNode>.Parent => this.Parent;

    IReadOnlyCollection<IReadOnlyTestProcessingNode> IDependencyNode<IReadOnlyTestProcessingNode>.ReferencedBy => this.ReferencedBy;
}