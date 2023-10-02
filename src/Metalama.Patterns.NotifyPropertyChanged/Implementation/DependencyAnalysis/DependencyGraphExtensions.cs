// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.Graph;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;

[CompileTime]
internal static class DependencyGraphExtensions
{
    /// <summary>
    /// Constructs a duplicate of the current graph using the specified node type.
    /// </summary>
    /// <remarks>
    /// This method will set the <see cref="DependencyGraph.Node.Tag"/> property of all nodes in the graph. By design, the tags
    /// can only be set once.
    /// </remarks>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TContext">The type of a context object that will be passed to <see cref="IInitializableNode{TNode, TContext}.Initialize"/>.</typeparam>
    /// <param name="node">The root node of a graph.</param>
    /// <param name="context">A context object that will be passed to <see cref="IInitializableNode{TNode, TContext}.Initialize"/>.</param>
    /// <returns></returns>    
    public static TNode DuplicateUsing<[CompileTime] TNode, TContext>( this DependencyGraph.Node node, TContext? context = default )
        where TNode : IInitializableNode<TNode, TContext>, new()
    {
        if ( node == null )
        {
            throw new ArgumentNullException( nameof(node) );
        }

        if ( !node.IsRoot )
        {
            throw new ArgumentException( "Must be a root node.", nameof(node) );
        }

        foreach ( var n in node.SelfAndDescendantsDepthFirst() )
        {
            n.Tag = new TNode();
        }

        foreach ( var n in node.SelfAndDescendantsDepthFirst() )
        {
            TNode[]? children = null;

            if ( n.HasChildren )
            {
                var nc = n.Children;
                children = new TNode[nc.Count];
                var i = 0;

                foreach ( var c in nc )
                {
                    children[i++] = (TNode) c.Tag!;
                }
            }

            TNode[]? referencedBy = null;

            if ( n.HasReferencedBy )
            {
                var nr = n.ReferencedBy;
                referencedBy = new TNode[nr.Count];
                var i = 0;

                foreach ( var r in nr )
                {
                    referencedBy[i++] = (TNode) r.Tag!;
                }
            }

            ((TNode) n.Tag!).Initialize(
                context,
                n.Depth,
                n.Symbol,
                (TNode?) (n.IsRoot ? null : n.Parent.Tag),
                children,
                referencedBy );
        }

        return (TNode) node.Tag!;
    }
}