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
internal static class DependencyHelper
{
    [CompileTime]
    public sealed class TreeNode
    {
        private readonly ISymbol? _symbol;
        private Dictionary<ISymbol, TreeNode>? _children;
        private HashSet<ISymbol>? _referencedBy;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class which represents the root node of a tree.
        /// </summary>
        public TreeNode()
        {
        }

        private TreeNode( TreeNode parent, ISymbol symbol ) 
        {
            this.Parent = parent;
            this._symbol = symbol;
        }

        public bool IsRoot => this.Parent == null;

        public TreeNode? Parent { get; }

        /// <summary>
        /// Gets the symbol of the node.
        /// </summary>
        public ISymbol Symbol => this._symbol ?? throw new NotSupportedException( "The root node does not have a symbol." );

        public IReadOnlyCollection<TreeNode> Children => ((IReadOnlyCollection<TreeNode>?) this._children?.Values) ?? Array.Empty<TreeNode>();

        /// <summary>
        /// Gets the members that reference the current node as the final member of an access expression.
        /// </summary>
        public IReadOnlyCollection<ISymbol> ReferencedBy => ((IReadOnlyCollection<ISymbol>?) this._children) ?? Array.Empty<ISymbol>();

        public TreeNode GetOrAddChild( ISymbol childSymbol )
        {
            TreeNode result;

            if ( this._children == null )
            {
                this._children = new();
                result = new TreeNode( this, childSymbol );
                this._children.Add( childSymbol, result );             
            }
            else
            {
                if ( !this._children.TryGetValue( childSymbol, out result ) )
                {
                    result = new TreeNode( this, childSymbol );
                    this._children.Add( childSymbol, result );
                }
            }

            return result;
        }

        public TreeNode? GetChild( ISymbol childSymbol ) 
            => this._children == null || !this._children.TryGetValue( childSymbol, out var result )
                ? null : result;

        public void AddReferencedBy( ISymbol symbol )
        {
            this._referencedBy ??= new();
            this._referencedBy.Add( symbol );
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            this.ToString( sb, 0 );
            return sb.ToString();
        }

        private void ToString( StringBuilder appendTo, int indent )
        {
            appendTo.Append( ' ', indent ).Append( this._symbol?.Name ?? "<root>" );

            if ( this._referencedBy != null )
            {
                appendTo.Append( " [ " ).Append( string.Join( ", ", this._referencedBy.Select( s => s.Name ).OrderBy( n => n ) ) ).Append( " ]" );
            }

            appendTo.AppendLine();

            if ( this._children != null )
            {
                indent += 2;
                foreach ( var child in this._children.Values.OrderBy( c => c.Symbol.Name ) )
                {
                    child.ToString( appendTo, indent );
                }
            }
        }
    }

    public static TreeNode GetDependencyGraph( INamedType type )
    {
        var tree = new TreeNode();

        foreach ( var p in type.Properties )
        {
            AddReferencedProperties( tree, p );
        }

        return tree;
    }

    private static void AddReferencedProperties(
        TreeNode tree,
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
        private readonly TreeNode _tree;
        private readonly ISymbol _origin;
        private readonly SemanticModel _semanticModel;
        private readonly List<IPropertySymbol> _properties = new();
        private int _depth = 1;

        public Visitor(
            TreeNode tree,
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
                var treeNode = this._tree;

                for ( var i = 0; i < this._properties.Count; i++ )
                {
                    treeNode = treeNode.GetOrAddChild( this._properties[i] );
                }

                treeNode.AddReferencedBy( this._origin );

                this._accessorStartDepth = 0;
                this._properties.Clear();
            }

            --this._depth;
        }

        private int _accessorStartDepth;

        public override void VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            if ( this._accessorStartDepth == 0 )
            {
                this._accessorStartDepth = this._depth;
            }

            base.VisitMemberAccessExpression( node );
        }

        public override void VisitIdentifierName( IdentifierNameSyntax node )
        {
            if ( this._accessorStartDepth > 0 )
            {
                var symbol = this._semanticModel.GetSymbolInfo( node ).Symbol;
                if ( symbol is IPropertySymbol property )
                {
                    this._properties.Add( property );
                }
                else
                {
                    // Not a property (eg, a method or field).
                    // TODO: Emit error when not supported/allowed.

                    this._accessorStartDepth = 0;
                    this._properties.Clear();
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