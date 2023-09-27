// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

internal static partial class DependencyGraph
{
    [CompileTime]
    private interface IConstructionNode
    {
        Node GetOrAddChild( ISymbol childSymbol );

        void AddReferencedBy( Node node );
    }

    /// <summary>
    /// Simple nodes concerned only with the structure of the dependency graph.
    /// </summary>
    [CompileTime]
    public sealed class Node : IConstructionNode, IParentedTreeNode<Node>
    {
        private Dictionary<ISymbol, Node>? _children;
        private HashSet<Node>? _referencedBy;

        internal Node()
        {
        }

        internal Node( Node parent, ISymbol symbol )
        {
            this.Parent = parent;
            this.Symbol = symbol;
        }

        public ISymbol? Symbol { get; }

        public Node? Parent { get; }

        public bool IsRoot => this.Parent == null;

        public bool HasChildren => this._children != null;

        public IEnumerable<Node> Children => (IEnumerable<Node>?) this._children?.Values ?? Array.Empty<Node>();

        public bool HasReferencedBy => this._referencedBy != null;

        public IEnumerable<Node> ReferencedBy => (IEnumerable<Node>?) this._referencedBy ?? Array.Empty<Node>();

        private object? _tag;

        /// <summary>
        /// Gets or sets an arbitary object associated with the current node.
        /// </summary>
        /// <remarks>
        /// Once set, the value cannot be changed. This enforces a compromise between correctness and performance.
        /// </remarks>
        public object? Tag 
        {
            get => this._tag;
            set
            {
                if ( this._tag == null )
                { 
                    this._tag = value;
                }
                else
                {
                    throw new InvalidOperationException( "The tag has already been set and cannot be changed." );
                }
            }
        }

        Node IConstructionNode.GetOrAddChild( ISymbol childSymbol )
        {
            Node? result;

            if ( this._children == null )
            {
                this._children = new Dictionary<ISymbol, Node>();
                result = new Node( this, childSymbol );
                this._children.Add( childSymbol, result );
            }
            else
            {
                if ( !this._children.TryGetValue( childSymbol, out result ) )
                {
                    result = new Node( this, childSymbol );
                    this._children.Add( childSymbol, result );
                }
            }

            return result;
        }

        void IConstructionNode.AddReferencedBy( Node node )
        {
            this._referencedBy ??= new HashSet<Node>();
            this._referencedBy.Add( node );
        }
    }
}