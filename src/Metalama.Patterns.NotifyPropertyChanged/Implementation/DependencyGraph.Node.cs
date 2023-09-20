// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

internal static partial class DependencyGraph
{
    [CompileTime]
    public class Node<T> : INode
    {
        private readonly ISymbol? _symbol;
        private readonly IFieldOrProperty? _fieldOrProperty;
        private string? _dottedPropertyPath;
        private string? _contiguousPropertyPath;
        private Dictionary<ISymbol, Node<T>>? _children;
        private HashSet<Node<T>>? _referencedBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class which represents the root node of a tree.
        /// </summary>
        public Node()
        {
        }

        private Node( Node<T> parent, ISymbol symbol, ICompilation compilation ) 
        {
            this.Parent = parent ?? throw new ArgumentNullException( nameof( parent ) );
            this._symbol = symbol ?? throw new ArgumentNullException( nameof( symbol ) );
            this._fieldOrProperty = (IFieldOrProperty) compilation.GetDeclaration( symbol );
            this.Depth = parent.Depth + 1;
        }

        private static Exception NewNotSupportedOnRootNodeException()
            => new InvalidOperationException( "The operation is not supported on a root node." );

        public bool IsRoot => this.Parent == null;

        public Node<T>? Parent { get; }

        /// <summary>
        /// Gets the depth of the current node. The unparented root node has depth zero.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Gets the Roslyn symbol of the node. Use <see cref="FieldOrProperty"/> for the Metalama equivalent.
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="IsRoot"/> is <see langword="true"/>.</exception>
        public ISymbol Symbol => this._symbol ?? throw NewNotSupportedOnRootNodeException();

        /// <summary>
        /// Gets the Metalama <see cref="IFieldOrProperty"/> for the node. Use <see cref="Symbol"/> for the Rosyln equivalent.
        /// </summary>
        public IFieldOrProperty FieldOrProperty => this._fieldOrProperty ?? throw NewNotSupportedOnRootNodeException();

        /// <summary>
        /// Gets a property path like "A1" or "A1.B1".
        /// </summary>
        public string DottedPropertyPath 
            => this._dottedPropertyPath ??= 
                this.IsRoot
                ? throw NewNotSupportedOnRootNodeException()
                : this.Parent!.IsRoot ? this.Name : $"{this.Parent.DottedPropertyPath}.{this.Name}";

        /// <summary>
        /// Gets a property path like "A1" or "A1B1".
        /// </summary>
        public string ContiguousPropertyPath
            => this._contiguousPropertyPath ??=
                this.IsRoot
                ? throw NewNotSupportedOnRootNodeException()
                : this.Parent!.IsRoot ? this.Name : $"{this.Parent.ContiguousPropertyPath}.{this.Name}";

        /// <summary>
        /// Extensibility point for the consumer.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        public T? Data;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Gets the name of the node. This is a synonym for <c>Symbol.Name</c>.
        /// </summary>
        public string Name => this.Symbol.Name;

        public IReadOnlyCollection<Node<T>> Children => ((IReadOnlyCollection<Node<T>>?) this._children?.Values) ?? Array.Empty<Node<T>>();

        public bool HasChildren => this._children != null;

        /// <summary>
        /// Gets the members that reference the current node.
        /// </summary>
        public IReadOnlyCollection<Node<T>> DirectReferences => ((IReadOnlyCollection<Node<T>>?) this._referencedBy) ?? Array.Empty<Node<T>>();

        /// <summary>
        /// Gets the distinct set of members that reference, directly or indirectly, the current node. By default, the search follows only <see cref="DirectReferences"/>;
        /// if <paramref name="includeImmediateChild"/> is specified, the search follows the direct references of the current node and
        /// the direct references of the current node's children matching the given predicate.
        /// </summary>
        /// <param name="includeImmediateChild"></param>
        /// <returns></returns>
        public IReadOnlyCollection<Node<T>> GetAllReferences( Func<Node<T>,bool>? includeImmediateChild = null )
        {
            // TODO: This algorithm is naive, and will cause repeated work if GetAllReferences() is called on one of the nodes already visited.
            // However, it's not recusive so there's no risk of stack overflow. So safe, but slow.

            if ( this._referencedBy == null && includeImmediateChild == null )
            {
                return Array.Empty<Node<T>>();
            }

            var refsToFollow = new Stack<Node<T>>(
                includeImmediateChild == null
                    ? this.DirectReferences
                    : this.Children.Where( n => includeImmediateChild( n ) ).SelectMany( n => n.DirectReferences ).Concat( this.DirectReferences ) );
            
            var refsFollowed = new HashSet<Node<T>>();

            while ( refsToFollow.Count > 0 )
            {
                var r = refsToFollow.Pop();

                if ( refsFollowed.Add( r ) )
                {
                    if ( r._referencedBy != null )
                    {
                        foreach ( var indirectRef in r._referencedBy )
                        {
                            refsToFollow.Push( indirectRef );
                        }
                    }
                }
            }

            return refsFollowed;
        }

        public IEnumerable<Node<T>> DecendantsDepthFirst()
        {
            // NB: No loop detection.

            var stack = new Stack<Node<T>>( this.Children );

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

        /// <summary>
        /// Gets the ancestors of the current node in leaf-to-root order.
        /// </summary>
        /// <param name="includeRoot"></param>
        /// <returns></returns>
        public IEnumerable<Node<T>> Ancestors( bool includeRoot = false )
            => this.AncestorsCore( includeRoot, false );

        /// <summary>
        /// Gets the current node and its ancestors in leaf-to-root order.
        /// </summary>
        /// <param name="includeRoot"></param>
        /// <returns></returns>
        public IEnumerable<Node<T>> AncestorsAndSelf( bool includeRoot = false )
            => this.AncestorsCore( includeRoot, true );

        private IEnumerable<Node<T>> AncestorsCore( bool includeRoot, bool includeSelf )
        {
            var node = includeSelf ? this : this.Parent;

            while ( node != null )
            {
                if ( !includeRoot && node.IsRoot )
                {
                    break;
                }
                yield return node;
                node = node.Parent;
            }
        }

        public Node<T> GetAncestorOrSelfAtDepth( int depth )
        {
            if ( depth > this.Depth || depth < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( depth ), "Must be greater than zero and less than or equal to the depth of the current node." );
            }

            var n = this;

            while ( n!.Depth != depth )
            {
                n = n.Parent;
            }

            return n;
        }

        public Node<T> GetOrAddChild( ISymbol childSymbol, ICompilation compilation )
        {
            Node<T>? result;

            if ( this._children == null )
            {
                this._children = new();
                result = new Node<T>( this, childSymbol, compilation );
                this._children.Add( childSymbol, result );             
            }
            else
            {
                if ( !this._children.TryGetValue( childSymbol, out result ) )
                {
                    result = new Node<T>( this, childSymbol, compilation );
                    this._children.Add( childSymbol, result );
                }
            }

            return result;
        }

        INode INode.GetOrAddChild( ISymbol childSymbol, ICompilation compilation )
            => this.GetOrAddChild( childSymbol, compilation );

        public Node<T>? GetChild( ISymbol? childSymbol ) 
            => childSymbol == null || this._children == null || !this._children.TryGetValue( childSymbol, out var result )
                ? null : result;

        public void AddReferencedBy( Node<T> node )
        {
            this._referencedBy ??= new();
            this._referencedBy.Add( node );
        }

        void INode.AddReferencedBy( INode node )
            => this.AddReferencedBy( (Node<T>) node );

        public override string ToString()
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0 );
            return sb.ToString();
        }

        public string ToString( GetDisplayStringForData? displayData )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, displayData: displayData );
            return sb.ToString();
        }

        public string ToString( Node<T>? highlight, GetDisplayStringForData? displayData = null )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, highlight == null ? null : n => n == highlight, displayData );
            return sb.ToString();
        }

        public string ToString( Func<Node<T>,bool>? shouldHighlight, GetDisplayStringForData? displayData = null )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, shouldHighlight, displayData );
            return sb.ToString();
        }

        public delegate string GetDisplayStringForData( ref T? data );

        private void ToString( StringBuilder appendTo, int indent, Func<Node<T>,bool>? shouldHighlight = null, GetDisplayStringForData? displayData = null )
        {
            if ( shouldHighlight != null && shouldHighlight( this ) )
            {
                appendTo.Append( ' ', indent - 2 ).Append( "* " );
            }
            else
            {
                appendTo.Append( ' ', indent );
            }
            
            appendTo.Append( this._symbol?.Name ?? "<root>" );

            var allRefs = this.GetAllReferences();

            if ( allRefs.Count > 0 )
            {
                appendTo.Append( " [ " ).Append( string.Join( ", ", allRefs.Select( n => n.Name ).OrderBy( n => n ) ) ).Append( " ]" );
            }

            if ( displayData != null )
            {
                var dd = displayData( ref this.Data );
                if ( !string.IsNullOrEmpty( dd ) )
                {
                    appendTo.Append( ' ' ).Append( dd );
                }
            }

            appendTo.AppendLine();

            if ( this._children != null )
            {
                indent += 2;
                foreach ( var child in this._children.Values.OrderBy( c => c.Name ) )
                {
                    child.ToString( appendTo, indent, shouldHighlight, displayData );
                }
            }
        }
    }
}