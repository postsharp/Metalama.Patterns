// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal static class DependencyGraphExtensions
{
    public static IEnumerable<TNode> DescendantsDepthFirst<TNode>( this TNode node )
        where TNode : IParentedTreeNode<TNode>
        => DescendantsDepthFirst( node, false );

    public static IEnumerable<TNode> SelfAndDescendantsDepthFirst<TNode>( this TNode node )
        where TNode : IParentedTreeNode<TNode>
        => DescendantsDepthFirst( node, true );

    private static IEnumerable<TNode> DescendantsDepthFirst<TNode>( this TNode node, bool includeSelf )
        where TNode : IParentedTreeNode<TNode>
    {
        // NB: No loop detection.

        if ( includeSelf )
        {
            yield return node;
        }

        var stack = new Stack<TNode>( node.Children );

        while ( stack.Count > 0 )
        {
            var current = stack.Pop();

            yield return current;

            foreach ( var child in current.Children )
            {
                stack.Push( child );
            }
        }
    }

    public delegate void InitializeNode<TNode>( TNode node, ISymbol? symbol, TNode? parent, IEnumerable<TNode>? children, IEnumerable<TNode>? referencedBy );

    /// <summary>
    /// Constructs a duplicate of the current graph using the specified node type.
    /// </summary>
    /// <remarks>
    /// This method will set the <see cref="DependencyGraph.Node.Tag"/> property of all nodes in the graph. The tags
    /// must not have been set previously.
    /// </remarks>
    /// <typeparam name="TNode">The root node of a graph.</typeparam>
    /// <param name="initialize"></param>
    /// <returns></returns>
    public static TNode DuplicateUsing<TNode>( this DependencyGraph.Node node, InitializeNode<TNode> initialize )
        where TNode : new()
    {
        if ( node == null )
        {
            throw new ArgumentNullException( nameof( node ) );
        }

        if ( !node.IsRoot )
        {
            throw new ArgumentException( "Must be a root node.", nameof( node ) );
        }

        foreach ( var n in node.SelfAndDescendantsDepthFirst() )
        {
            n.Tag = new TNode();
        }

        foreach ( var n in node.SelfAndDescendantsDepthFirst() )
        {
            initialize(
                (TNode) n.Tag!,
                n.Symbol,
                (TNode?) n.Parent?.Tag,
                n.HasChildren ? n.Children.Select( c => (TNode) c.Tag! ) : null,
                n.HasReferencedBy ? n.ReferencedBy.Select( r => (TNode) r.Tag! ) : null );
        }

        return (TNode) node.Tag!;
    }
}