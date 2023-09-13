// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal static class DependencyGraph
{
    [CompileTime]
    public interface INode
    {
        INode GetOrAddChild( ISymbol childSymbol );
        
        void AddReferencedBy( INode node );
    }

    [CompileTime]
    public class Node<T> : INode
    {
        private readonly ISymbol? _symbol;
        private Dictionary<ISymbol, Node<T>>? _children;
        private HashSet<Node<T>>? _referencedBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class which represents the root node of a tree.
        /// </summary>
        public Node()
        {
        }

        private Node( Node<T> parent, ISymbol symbol ) 
        {
            this.Parent = parent;
            this._symbol = symbol;
            this.Depth = parent.Depth + 1;
        }

        /// <summary>
        /// Extensibility point for the consumer.
        /// </summary>
#pragma warning disable SA1401 // Fields should be private
        public T? Data;
#pragma warning restore SA1401 // Fields should be private

        public bool IsRoot => this.Parent == null;

        public Node<T>? Parent { get; }

        /// <summary>
        /// Gets the depth of the current node. The unparented root node has depth zero.
        /// </summary>
        public int Depth { get; }

        /// <summary>
        /// Gets the symbol of the node.
        /// </summary>
        /// <exception cref="NotSupportedException"><see cref="IsRoot"/> is <see langword="true"/>.</exception>
        public ISymbol Symbol => this._symbol ?? throw new NotSupportedException( "The operation is not supported on root nodes." );

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
        /// Gets the members that reference, directly or indirectly, the current node. By default, the search follows only <see cref="DirectReferences"/>;
        /// if <paramref name="followDescendants"/> is <see langword="true"/>, the search follows the direct references of the current node and
        /// the direct references of the current node's decendants.
        /// </summary>
        /// <param name="followDescendants"></param>
        /// <returns></returns>
        public IReadOnlyCollection<Node<T>> GetAllReferences( bool followDescendants = false )
        {
            // TODO: This algorithm is naive, and will cause repeated work if GetAllReferences() is called on one of the nodes already visited.
            // However, it's not recusive so there's no risk of stack overflow. So safe, but slow.

            if ( this._referencedBy == null && !followDescendants )
            {
                return Array.Empty<Node<T>>();
            }

            var refsToFollow = new Stack<Node<T>>(
                followDescendants
                    ? this.DecendantsDepthFirst().SelectMany( n => n.DirectReferences ).Concat( this.DirectReferences )
                    : this.DirectReferences );
            
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

        public Node<T> GetOrAddChild( ISymbol childSymbol )
        {
            Node<T> result;

            if ( this._children == null )
            {
                this._children = new();
                result = new Node<T>( this, childSymbol );
                this._children.Add( childSymbol, result );             
            }
            else
            {
                if ( !this._children.TryGetValue( childSymbol, out result ) )
                {
                    result = new Node<T>( this, childSymbol );
                    this._children.Add( childSymbol, result );
                }
            }

            return result;
        }

        INode INode.GetOrAddChild( ISymbol childSymbol )
            => this.GetOrAddChild( childSymbol );

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

#if DEBUG || LAMADEBUG
        public string GetPath()
            => string.Join( ".", this.AncestorsAndSelf().Reverse().Select( n => n.Name ) );
#endif

        public override string ToString()
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0 );
            return sb.ToString();
        }

        public string ToString( Node<T>? highlight )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, highlight == null ? null : n => n == highlight );
            return sb.ToString();
        }

        public string ToString( Func<Node<T>,bool>? shouldHighlight )
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0, shouldHighlight );
            return sb.ToString();
        }

        private void ToString( StringBuilder appendTo, int indent, Func<Node<T>,bool>? shouldHighlight = null )
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

    public static Node<T> GetDependencyGraph<T>( INamedType type )
    {
        var tree = new Node<T>();

        foreach ( var p in type.Properties )
        {
            AddReferencedProperties( tree, p );
        }

        return tree;
    }

    private static void AddReferencedProperties(
        INode tree,
        IProperty property )
    {
        var compilation = property.Compilation.GetRoslynCompilation();

        var propertySymbol = property.GetSymbol();

        if ( propertySymbol == null )
        {
            return;
        }
        var body = propertySymbol
            .DeclaringSyntaxReferences
            .Select( r => r.GetSyntax() )
            .Cast<PropertyDeclarationSyntax>()
            .Select( GetGetterBody )
            .SingleOrDefault();

        if ( body == null )
        {
            return;
        }

        var semanticModel = property.Compilation.GetSemanticModel( body.SyntaxTree );

        var visitor = new Visitor( tree, propertySymbol, semanticModel );
        visitor.Visit( body );
    }

    private class Visitor : CSharpSyntaxWalker
    {
        private readonly INode _tree;
        private readonly ISymbol _origin;
        private readonly SemanticModel _semanticModel;
        private readonly List<IPropertySymbol> _properties = new();
        private SyntaxNode? _lastVisitedPropertyNode;
        private int _depth = 1;
        private int _accessorStartDepth;

        public Visitor(
            INode tree,
            ISymbol origin,
            SemanticModel semanticModel )
        {
            this._tree = tree;
            this._origin = origin;
            this._semanticModel = semanticModel;
        }

        public override void Visit( SyntaxNode? node )
        {
            ++this._depth;

            base.Visit( node );

            if ( this._accessorStartDepth == this._depth && this._properties.Count > 0 )
            {
                if ( this._lastVisitedPropertyNode.GetAccessKind() is AccessKind.Read or AccessKind.ReadWrite )
                {
                    var treeNode = this._tree;

                    for ( var i = 0; i < this._properties.Count; i++ )
                    {
                        treeNode = treeNode.GetOrAddChild( this._properties[i] );
                    }

                    treeNode.AddReferencedBy( this._tree.GetOrAddChild( this._origin ) );
                }

                this.ClearCurrentAccessor();
            }

            --this._depth;
        }

        private void ClearCurrentAccessor()
        {
            this._accessorStartDepth = 0;
            this._lastVisitedPropertyNode = null;
            this._properties.Clear();
        }

        public override void VisitBinaryExpression( BinaryExpressionSyntax node )
        {
            if ( node.IsKind( SyntaxKind.CoalesceExpression ) )
            {
                // This creates multiple potential access expressions.
                // TODO: Proper error reporting
                throw new NotSupportedException( "Coalesce expressions are not supported." );
            }

            base.VisitBinaryExpression( node );
        }

        public override void VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            if ( this._accessorStartDepth == 0 )
            {
                this._accessorStartDepth = this._depth;
            }

            base.VisitMemberAccessExpression( node );
        }

        public override void VisitConditionalAccessExpression( ConditionalAccessExpressionSyntax node )
        {
            if ( this._accessorStartDepth == 0 )
            {
                this._accessorStartDepth = this._depth;
            }

            base.VisitConditionalAccessExpression( node );
        }

        public override void VisitIdentifierName( IdentifierNameSyntax node )
        {
            if ( this._accessorStartDepth == 0 )
            {
                // This happens when an identifier is used without qualification,
                // eg. X in `int Y => X`
                if ( node.GetAccessKind() is AccessKind.Read or AccessKind.ReadWrite )
                {
                    this._accessorStartDepth = this._depth;
                }
            }

            if ( this._accessorStartDepth > 0 )
            {
                var symbol = this._semanticModel.GetSymbolInfo( node ).Symbol;
                if ( symbol is IPropertySymbol property )
                {
                    this._properties.Add( property );
                    this._lastVisitedPropertyNode = node;
                }
                else
                {
                    // Not a property (eg, it's a method or field).
                    // TODO: Proper error handling when not supported/allowed.
                    throw new NotSupportedException( $"Encountered unsupported identifier '{node.Identifier.Text} of kind {(symbol == null ? "<unresolved>" : symbol.Kind)}." );
                    this.ClearCurrentAccessor();
                }
            }
        }
    }

    /// <summary>
    /// Gets the body of the property getter, if any.
    /// </summary>
    private static SyntaxNode? GetGetterBody( PropertyDeclarationSyntax property )
    {
        if ( property.ExpressionBody != null )
        {
            return property.ExpressionBody;
        }

        if ( property.AccessorList == null )
        {
            return null;
        }

        // We are not using LINQ to work around a bug (#33676) with lambda expressions in compile-time code.
        foreach ( var accessor in property.AccessorList.Accessors )
        {
            if ( accessor.Keyword.IsKind( SyntaxKind.GetKeyword ) )
            {
                return (SyntaxNode?) accessor.ExpressionBody ?? accessor.Body;
            }
        }

        return null;
    }
}