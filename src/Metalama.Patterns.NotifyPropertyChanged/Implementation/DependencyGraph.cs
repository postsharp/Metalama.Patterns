// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Engine.CodeModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal static partial class DependencyGraph
{
    public delegate void ReportDiagnostic( IDiagnostic diagnostic, Location? location = null );

    public static TNode GetDependencyGraph<TNode>( INamedType type, ReportDiagnostic reportDiagnostic )
        where TNode : Node<TNode>, new()
    {
        var tree = new TNode();

        foreach ( var p in type.Properties )
        {
            AddReferencedProperties( tree, p, reportDiagnostic );
        }

        return tree;
    }

    private static void AddReferencedProperties<TNode>(
        TNode tree,
        IProperty property,
        ReportDiagnostic reportDiagnostic )
        where TNode : Node<TNode>, new()
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

        var visitor = new Visitor<TNode>( tree, property, propertySymbol, property.Compilation, semanticModel, reportDiagnostic );
        visitor.Visit( body );
    }

    private class Visitor<TNode> : CSharpSyntaxWalker
        where TNode : Node<TNode>, new()
    {
        private readonly TNode _tree;
        private readonly IFieldOrProperty _originFieldOrProperty;
        private readonly ISymbol _originSymbol;
        private readonly ICompilation _compilation;
        private readonly SemanticModel _semanticModel;
        private readonly List<IPropertySymbol> _properties = new();
        private readonly ReportDiagnostic _reportDiagnostic;
        private SyntaxNode? _lastVisitedPropertyNode;
        private int _depth = 1;
        private int _accessorStartDepth;

        public Visitor(
            TNode tree,
            IFieldOrProperty originFieldOrProperty,
            ISymbol originSymbol,
            ICompilation compilation,
            SemanticModel semanticModel,            
            ReportDiagnostic reportDiagnostic )
        {
            this._tree = tree;
            this._originSymbol = originSymbol;
            this._originFieldOrProperty = originFieldOrProperty;
            this._compilation = compilation;
            this._semanticModel = semanticModel;
            this._reportDiagnostic = reportDiagnostic;
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

                    foreach ( var p in this._properties )
                    {
                        treeNode = treeNode.GetOrAddChild( p, (IFieldOrProperty) this._compilation.GetDeclaration( p ) );
                    }

                    treeNode.AddReferencedBy( this._tree.GetOrAddChild( this._originSymbol, this._originFieldOrProperty ) );
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

                this._reportDiagnostic(
                    DiagnosticDescriptors.DependencyAnalysis.ErrorMiscUnsupportedExpression.WithArguments( "Coalesce" ),
                    node.GetLocation() );

                this.ClearCurrentAccessor();
            }
            else
            {
                base.VisitBinaryExpression( node );
            }
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
                    this._reportDiagnostic(
                        DiagnosticDescriptors.DependencyAnalysis.ErrorMiscUnsupportedIdentifier
                        .WithArguments( (node.Identifier.Text, symbol == null ? "<unresolved>" : symbol.Kind.ToString()) ),
                        node.GetLocation() );
                    
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