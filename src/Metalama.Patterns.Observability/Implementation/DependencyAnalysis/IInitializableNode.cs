// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal interface IInitializableNode<in TNode, in TContext>
{
    /// <summary>
    /// Sets the data for a node. 
    /// </summary>
    /// <remarks>
    /// Nodes are initialized in <see cref="DependencyNodeExtensions.DescendantsDepthFirst{T}(T)"/> order.
    /// </remarks>
    /// <param name="context">A context object, for example provided by the call to <see cref="DependencyGraphExtensions.DuplicateUsing{TNode, TContext}(DependencyNode, TContext)"/>.</param>
    /// <param name="depth">The depth of the node.</param>
    /// <param name="symbol">The symbol of the current node, or <see langword="null"/> if the current node is the root node of a graph.</param>
    /// <param name="parent">The parent of the current node, or <see langword="null"/> if the current node is the root node of a graph.</param>
    /// <param name="children">The children of the current node, or <see langword="null"/> if there are none.</param>
    /// <param name="referencedBy">The nodes referencing the current node, or <see langword="null"/> if there are none.</param>
    void Initialize( TContext? context, int depth, ISymbol? symbol, TNode? parent, TNode[]? children, TNode[]? referencedBy );
}