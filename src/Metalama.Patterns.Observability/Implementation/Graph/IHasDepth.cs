// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Implementation.Graph;

[CompileTime]
internal interface IHasDepth
{
    /// <summary>
    /// Gets the depth of a tree node. The root node has depth zero, the children of the root node have depth 1, and so on.
    /// </summary>
    int Depth { get; }
}