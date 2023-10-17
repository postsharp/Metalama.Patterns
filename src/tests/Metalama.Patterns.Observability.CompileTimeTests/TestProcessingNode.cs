// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Observability.Implementation;
using Metalama.Patterns.Observability.Implementation.Graph;

namespace Metalama.Patterns.Observability.CompileTimeTests;

/// <summary>
/// An unspecialized <see cref="ProcessingNode{TDerived, TReadOnlyDerivedInterface}"/> used for unit tests.
/// </summary>
[CompileTime]
internal sealed class TestProcessingNode : ProcessingNode<TestProcessingNode, IReadOnlyTestProcessingNode>, IReadOnlyTestProcessingNode
{
    IReadOnlyCollection<IReadOnlyTestProcessingNode> IHasChildren<IReadOnlyTestProcessingNode>.Children => this.Children;

    IReadOnlyTestProcessingNode IHasParent<IReadOnlyTestProcessingNode>.Parent => this.Parent;

    IReadOnlyCollection<IReadOnlyTestProcessingNode> IHasReferencedBy<IReadOnlyTestProcessingNode>.ReferencedBy => this.ReferencedBy;
}