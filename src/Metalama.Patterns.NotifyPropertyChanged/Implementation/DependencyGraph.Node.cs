// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

internal partial class DependencyGraph
{
    /// <summary>
    /// Unspecialized <see cref="Node{TDerived}"/>.
    /// </summary>
    public sealed class Node : Node<Node> { }

    [CompileTime]
    public class Node<TDerived>
        where TDerived : Node<TDerived>, new()
    {
        private ISymbol? _symbol;
        private IFieldOrProperty? _fieldOrProperty;
        private string? _dottedPropertyPath;
        private string? _contiguousPropertyPath;
        private Dictionary<ISymbol, TDerived>? _children;
        private HashSet<TDerived>? _referencedBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class which represents the root node of a tree.
        /// </summary>
        protected Node() { }

        private void InitializeBase( TDerived parent, ISymbol symbol, IFieldOrProperty fieldOrProperty )
        {
            this.Parent = parent ?? throw new ArgumentNullException( nameof(parent) );
            this._symbol = symbol ?? throw new ArgumentNullException( nameof(symbol) );
            this._fieldOrProperty = fieldOrProperty ?? throw new ArgumentNullException( nameof(fieldOrProperty) );
            this.Depth = parent.Depth + 1;
            this.Initialize();
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual void Initialize() { }

        // ReSharper disable once MemberCanBePrivate.Global
        protected static Exception NewNotSupportedOnRootNodeException() => new InvalidOperationException( "The operation is not supported on a root node." );

        public bool IsRoot => this.Parent == null;

        public TDerived? Parent { get; private set; }

        /// <summary>
        /// Gets the depth of the current node. The unparented root node has depth zero.
        /// </summary>
        public int Depth { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets the Roslyn symbol of the node. Use <see cref="FieldOrProperty"/> for the Metalama equivalent.
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="IsRoot"/> is <see langword="true"/>.</exception>
        public ISymbol Symbol => this._symbol ?? throw NewNotSupportedOnRootNodeException();

        /// <summary>
        /// Gets the Metalama <see cref="IFieldOrProperty"/> for the node. Use <see cref="Symbol"/> for the Roslyn equivalent.
        /// </summary>
        public IFieldOrProperty FieldOrProperty => this._fieldOrProperty ?? throw NewNotSupportedOnRootNodeException();

        /// <summary>
        /// Gets a property path like "A1" or "A1.B1".
        /// </summary>
        public string DottedPropertyPath
            => this._dottedPropertyPath ??=
                this.IsRoot
                    ? throw NewNotSupportedOnRootNodeException()
                    : this.Parent!.IsRoot
                        ? this.Name
                        : $"{this.Parent.DottedPropertyPath}.{this.Name}";

        /// <summary>
        /// Gets a property path like "A1" or "A1B1".
        /// </summary>
        public string ContiguousPropertyPath
            => this._contiguousPropertyPath ??=
                this.IsRoot
                    ? throw NewNotSupportedOnRootNodeException()
                    : this.Parent!.IsRoot
                        ? this.Name
                        : $"{this.Parent.ContiguousPropertyPath}.{this.Name}";

        public string ContiguousPropertyPathWithoutDot => this.ContiguousPropertyPath.Replace( ".", "" );

        /// <summary>
        /// Gets the name of the node. This is a synonym for <c>Symbol.Name</c>.
        /// </summary>
        public string Name => this.Symbol.Name;

        public IReadOnlyCollection<TDerived> Children => (IReadOnlyCollection<TDerived>?) this._children?.Values ?? Array.Empty<TDerived>();

        public bool HasChildren => this._children != null;

        /// <summary>
        /// Gets the members that reference the current node.
        /// </summary>
        public IReadOnlyCollection<TDerived> DirectReferences => (IReadOnlyCollection<TDerived>?) this._referencedBy ?? Array.Empty<TDerived>();

        /// <summary>
        /// Gets the distinct set of members that reference, directly or indirectly, the current node. By default, the search follows only <see cref="DirectReferences"/>;
        /// if <paramref name="includeImmediateChild"/> is specified, the search follows the direct references of the current node and
        /// the direct references of the current node's children matching the given predicate.
        /// </summary>
        /// <param name="includeImmediateChild"></param>
        /// <returns></returns>
        public IReadOnlyCollection<TDerived> GetAllReferences( Func<TDerived, bool>? includeImmediateChild = null )
        {
            // TODO: This algorithm is naive, and will cause repeated work if GetAllReferences() is called on one of the nodes already visited.
            // However, it's not recursive so there's no risk of stack overflow. So safe, but slow.

            if ( this._referencedBy == null && includeImmediateChild == null )
            {
                return Array.Empty<TDerived>();
            }

            var refsToFollow = new Stack<TDerived>(
                includeImmediateChild == null
                    ? this.DirectReferences
                    : this.Children.Where( includeImmediateChild ).SelectMany( n => n.DirectReferences ).Concat( this.DirectReferences ) );

            var refsFollowed = new HashSet<TDerived>();

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

        public IEnumerable<TDerived> DescendantsDepthFirst()
        {
            // NB: No loop detection.

            var stack = new Stack<TDerived>( this.Children );

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
        public IEnumerable<TDerived> Ancestors( bool includeRoot = false ) => this.AncestorsCore( includeRoot, false );

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Gets the current node and its ancestors in leaf-to-root order.
        /// </summary>
        /// <param name="includeRoot"></param>
        /// <returns></returns>
        public IEnumerable<TDerived> AncestorsAndSelf( bool includeRoot = false ) => this.AncestorsCore( includeRoot, true );

        private IEnumerable<TDerived> AncestorsCore( bool includeRoot, bool includeSelf )
        {
            var node = includeSelf ? (TDerived) this : this.Parent;

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

        public TDerived GetAncestorOrSelfAtDepth( int depth )
        {
            if ( depth > this.Depth || depth < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof(depth), "Must be greater than zero and less than or equal to the depth of the current node." );
            }

            var n = (TDerived) this;

            while ( n!.Depth != depth )
            {
                n = n.Parent;
            }

            return n;
        }

        public TDerived GetOrAddChild( ISymbol childSymbol, IFieldOrProperty fieldOrProperty )
        {
            TDerived? result;

            if ( this._children == null )
            {
                this._children = new Dictionary<ISymbol, TDerived>();
                result = new TDerived();
                result.InitializeBase( (TDerived) this, childSymbol, fieldOrProperty );
                this._children.Add( childSymbol, result );
            }
            else
            {
                if ( !this._children.TryGetValue( childSymbol, out result ) )
                {
                    result = new TDerived();
                    result.InitializeBase( (TDerived) this, childSymbol, fieldOrProperty );
                    this._children.Add( childSymbol, result );
                }
            }

            return result;
        }

        public TDerived? GetChild( ISymbol? childSymbol )
            => childSymbol == null || this._children == null || !this._children.TryGetValue( childSymbol, out var result )
                ? null
                : result;

        public void AddReferencedBy( TDerived node )
        {
            this._referencedBy ??= new HashSet<TDerived>();
            this._referencedBy.Add( node );
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0 );

            return sb.ToString();
        }

        public string ToString( string? format )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, format: format );

            return sb.ToString();
        }

        public string ToString( TDerived? highlight, string? format = null )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, highlight == null ? null : n => n == highlight, format );

            return sb.ToString();
        }

        // ReSharper disable once UnusedMember.Global
        public string ToString( Func<TDerived, bool>? shouldHighlight, string? format = null )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, shouldHighlight, format );

            return sb.ToString();
        }

        protected virtual void ToStringAppendToLine( StringBuilder appendTo, string? format ) { }

        private void ToString( StringBuilder appendTo, int indent, Func<TDerived, bool>? shouldHighlight = null, string? format = null )
        {
            if ( shouldHighlight != null && shouldHighlight( (TDerived) this ) )
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

            this.ToStringAppendToLine( appendTo, format );

            appendTo.AppendLine();

            if ( this._children != null )
            {
                indent += 2;

                foreach ( var child in this._children.Values.OrderBy( c => c.Name ) )
                {
                    child.ToString( appendTo, indent, shouldHighlight );
                }
            }
        }
    }
}