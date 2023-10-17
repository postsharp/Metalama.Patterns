// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Observability.Implementation.Graph;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

internal static partial class DependencyGraph
{
    /// <summary>
    /// Represents node concerned with the structure of the dependency graph.
    /// </summary>
    /// <remarks>
    /// Graphs comprised of these structural nodes are typically used as the template for graphs comprised of concern-specific processing nodes
    /// using the <see cref="DependencyGraphExtensions.DuplicateUsing{TNode, TContext}(Node, TContext)"/> extension method.
    /// Support for the graph building phase is exposed only to the <see cref="DependencyGraph"/> class via the private
    /// <see cref="IGraphBuildingNode"/> interface.
    /// </remarks>
    [CompileTime]
    public sealed class Node : IGraphBuildingNode, INode<Node>
    {
        private readonly Node? _parent;
        private Dictionary<ISymbol, Node>? _children;
        private HashSet<Node>? _referencedBy;

        internal Node() { }

        private Node( Node parent, ISymbol symbol )
        {
            this._parent = parent;
            this.Symbol = symbol;
            this.Depth = parent.Depth + 1;
        }

        public ISymbol? Symbol { get; }

        public int Depth { get; }

        public bool IsRoot => this._parent == null;

        public Node Parent => this._parent ?? throw new NotSupportedException( "Parent is not defined for a root node." );

        public bool HasChildren => this._children != null;

        public IReadOnlyCollection<Node> Children => (IReadOnlyCollection<Node>?) this._children?.Values ?? Array.Empty<Node>();

        public bool HasReferencedBy => this._referencedBy != null;

        public IReadOnlyCollection<Node> ReferencedBy => (IReadOnlyCollection<Node>?) this._referencedBy ?? Array.Empty<Node>();

        private object? _tag;

        /// <summary>
        /// Gets or sets an arbitrary object associated with the current node.
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

        Node IGraphBuildingNode.GetOrAddChild( ISymbol childSymbol )
        {
            if ( childSymbol == null )
            {
                throw new ArgumentNullException( nameof(childSymbol) );
            }

            if ( childSymbol.Equals( this.Symbol ) )
            {
                throw new InvalidOperationException( "Cannot add a child with the same symbol as the current node." );
            }

            Node? result;

            if ( this._children == null )
            {
                this._children = new Dictionary<ISymbol, Node>( SymbolEqualityComparer.Default );
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

        void IGraphBuildingNode.AddReferencedBy( Node node )
        {
            if ( node == null )
            {
                throw new ArgumentNullException( nameof(node) );
            }

            if ( node == this )
            {
                // Ignore reference to self.
                return;
            }

            this._referencedBy ??= new HashSet<Node>();
            this._referencedBy.Add( node );
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0 );

            return sb.ToString();
        }

        // ReSharper disable once UnusedMember.Global
        public string ToString( Node? highlight )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, highlight == null ? null : n => n == highlight );

            return sb.ToString();
        }

        private void ToString( StringBuilder appendTo, int indent, Func<Node, bool>? shouldHighlight = null )
        {
            if ( shouldHighlight != null && shouldHighlight( this ) )
            {
                appendTo.Append( ' ', indent - 2 ).Append( "* " );
            }
            else
            {
                appendTo.Append( ' ', indent );
            }

            appendTo.Append( this.Symbol?.Name ?? "<root>" );

            var allRefs = this.AllReferencedBy();

            if ( allRefs.Count > 0 )
            {
                appendTo.Append( " [ " ).Append( string.Join( ", ", allRefs.Select( n => n.Symbol?.Name ?? "<noname>" ).OrderBy( n => n ) ) ).Append( " ]" );
            }

            appendTo.AppendLine();

            if ( this._children != null )
            {
                indent += 2;

                foreach ( var child in this._children.Values.OrderBy( c => c.Symbol?.Name ?? "<noname>" ) )
                {
                    child.ToString( appendTo, indent, shouldHighlight );
                }
            }
        }
    }
}