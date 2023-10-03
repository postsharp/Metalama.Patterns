// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// To avoid confusion due to some identical type names, this file is concerned only with Roslyn types.
// Do not add usings for the namespaces Metalama.Framework.Code, Metalama.Framework.Engine.CodeModel or
// related namespaces.

using Metalama.Framework.Aspects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;

internal static partial class DependencyGraph
{
    private static void AddReferencedProperties(
        Framework.Code.ICompilation compilation,
        Node tree,
        IPropertySymbol property,
        ReportDiagnostic reportDiagnostic,
        CancellationToken cancellationToken )
    {
        var body = property
            .DeclaringSyntaxReferences
            .Select( r => r.GetSyntax( cancellationToken ) )
            .Cast<PropertyDeclarationSyntax>()
            .Select( GetGetterBody )
            .SingleOrDefault();

        if ( body == null )
        {
            return;
        }

        var semanticModel = Framework.Engine.CodeModel.SymbolExtensions.GetSemanticModel( compilation, body.SyntaxTree );

        var visitor = new Visitor( tree, property, semanticModel, reportDiagnostic, cancellationToken );
        
        visitor.Visit( body );
    }

    [CompileTime]
    private sealed class Visitor : CSharpSyntaxWalker
    {
        private readonly IGraphBuildingNode _tree;
        private readonly INamedTypeSymbol _declaringType;
        private readonly ISymbol _originSymbol;
        private readonly SemanticModel _semanticModel;
        private readonly List<(ISymbol Symbol, SyntaxNode Node)> _accessSymbols = new();
        private readonly ReportDiagnostic _reportDiagnostic;
        private readonly CancellationToken _cancellationToken;

        private int _depth = 1;
        private int _accessorStartDepth;

        public Visitor(
            IGraphBuildingNode tree,
            ISymbol originSymbol,
            SemanticModel semanticModel,
            ReportDiagnostic reportDiagnostic,
            CancellationToken cancellationToken )
        {
            this._tree = tree;
            this._declaringType = originSymbol.ContainingType;
            this._originSymbol = originSymbol;
            this._semanticModel = semanticModel;
            this._reportDiagnostic = reportDiagnostic;
            this._cancellationToken = cancellationToken;
        }

        private void ReportWarningNotImplementedForDependencyAnalysis( Location location, [CallerMemberName] string? name = null, [CallerLineNumber] int line = 0)
        {
            this._reportDiagnostic(
                DiagnosticDescriptors.WarningNotImplementedForDependencyAnalysis.WithArguments( $"{name}/{line}" ),
                location );
        }

        private static bool IsOrInheritsFrom( INamedTypeSymbol type, ITypeSymbol candidateBaseType )
        {
            var baseType = type;

            while ( baseType != null)
            {
                if (candidateBaseType.Equals( baseType ) )
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        private bool IsLocalInstanceMember( ISymbol symbol )
            => !symbol.IsStatic && IsOrInheritsFrom( symbol.ContainingType, this._declaringType );

        private bool ValidateLocalInstanceMember( (ISymbol Symbol, SyntaxNode Node) symbolAndNode )
        {
            // For now, we only allow refs to properties and private fields. Methods are not supported.

            var isValid = symbolAndNode.Symbol is IPropertySymbol || symbolAndNode.Symbol is IFieldSymbol { DeclaredAccessibility: Microsoft.CodeAnalysis.Accessibility.Private };

            if ( !isValid )
            {
                this._reportDiagnostic(
                    DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis
                        .WithArguments( "Only properties and private fields of the current type can be referenced." ),
                    symbolAndNode.Node.GetLocation() );
            }

            return isValid;
        }

        private static ITypeSymbol GetElementaryType( ITypeSymbol type )
        {
            while ( true )
            {
                var elementType = Get( type );

                if ( elementType == type )
                {
                    return elementType;
                }

                type = elementType;
            }

            static ITypeSymbol Get( ITypeSymbol t )
            {
                if ( t.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T )
                {
                    return ((INamedTypeSymbol) t).TypeArguments[0];
                }

                return t switch
                {
                    IArrayTypeSymbol array => array.ElementType,
                    IPointerTypeSymbol pointer => pointer.PointedAtType,
                    _ => t                    
                };
            }
        }

        private static bool IsPrimitiveType( ITypeSymbol? type )
        {
            return type != null && type.SpecialType is
                SpecialType.System_Boolean or
                SpecialType.System_Byte or
                SpecialType.System_Char or
                SpecialType.System_DateTime or
                SpecialType.System_Decimal or
                SpecialType.System_Double or
                SpecialType.System_Int16 or
                SpecialType.System_Int32 or
                SpecialType.System_Int64 or
                SpecialType.System_SByte or
                SpecialType.System_Single or
                SpecialType.System_String or
                SpecialType.System_UInt16 or
                SpecialType.System_UInt32 or
                SpecialType.System_UInt64;
        }

        private void ValidateMethodArgumentType( ExpressionSyntax expression )
        {
            var ti = this._semanticModel.GetTypeInfo( expression );

            if ( ti.Type is IErrorTypeSymbol )
            {
                // Don't report, assume that the compiler will report.
            }
            else if ( ti.Type == null )
            {
                // TODO: Decide how to handle when we have a use case.
                this.ReportWarningNotImplementedForDependencyAnalysis( expression.GetLocation() );
            }
            else
            {
                var elementaryType = GetElementaryType( ti.Type );

                if ( !IsPrimitiveType( elementaryType ) )
                {
                    this._reportDiagnostic(
                        DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis
                            .WithArguments( "Method arguments (including 'this') must be of primitive types." ),
                        expression.GetLocation() );
                }
            }
        }

#if false
        private void X( IReadOnlyCollection<(ISymbol Symbol, SyntaxNode Node)> accessSymbols )
        {
            // Granular validation is performed by the various Visit... overloads. The only validation
            // here is regarding the validity of the accessor sequence. For example, we don't support
            // 

            /* accessSymbols will be a chain of symbols which represent expressions like these (not all below are supported):
             * 
             * X
             * X.Y
             * X.Y.Z
             * X.M(...).Y
             * _x
             * _x.Y
             * 
             */

            // For now we only consider the stem comprising local fields and properties of
            var isValid = true;

            var localInstanceMemberStemCount = accessSymbols.TakeWhile( v => this.IsLocalInstanceMember( v.Symbol ) ).Count();
            
            isValid &= accessSymbols.Take( localInstanceMemberStemCount ).All( this.ValidateLocalInstanceMember );

            // Examine any remaining non-local symbols for validity
            foreach ( var nls in accessSymbols.Skip( localInstanceMemberStemCount ) )
            {
                var nlsIsValid = true;

                // TODO: Report relvant diagnostics for invalid conditions.

                if ( this.IsLocalInstanceMember( nls.Symbol ) )
                {
                    // We've hit a local instance member after hitting non-local-instance-member symbols. This is not supported.
                    nlsIsValid = false;
                }
                else if ( nls.Symbol is ILocalSymbol { IsConst: false } )
                {
                    // Non-const local variable
                    nlsIsValid = false;
                }                
                else if ( nls.Symbol is IMethodSymbol method )
                {
                    // Instance methods of the root type are already covered by IsLocalInstanceMember above. Here
                    // we only expect to hit static and external methods.

                    if ( !method.IsStatic )
                    {
                        // Validate 'this'
                    }

                    var invocationExpression = nls.Node.FirstAncestorOrSelf<SyntaxNode>( n => n.Kind() == SyntaxKind.InvocationExpression ) as InvocationExpressionSyntax;

                    if ( invocationExpression != null )
                    {
                        invocationExpression.ArgumentList.Arguments[0].Expression
                    }
                    else
                    {
                        // TODO: When would this happen?
                        throw new NotSupportedException( "Couldn't find ancestor invocation expression" );
                    }
                }
            }
        }
#endif

        public override void Visit( SyntaxNode? node )
        {
            this._cancellationToken.ThrowIfCancellationRequested();

            ++this._depth;

            base.Visit( node );

            if ( this._accessorStartDepth == this._depth && this._accessSymbols.Count > 0 )
            {
                // Only consider chains that start with a local member. And for now, only properties.
                // TODO: Expand logic here to support fields and methods.

                var firstSymbol = this._accessSymbols[ 0 ].Symbol;
                if ( this.IsLocalInstanceMember( firstSymbol ) )
                {
                    var stemCount = this._accessSymbols.Count( s => s.Symbol.Kind == SymbolKind.Property );

                    if ( stemCount > 0 )
                    {
                        var final = this._accessSymbols[stemCount - 1];

                        if ( final.Node.GetAccessKind() is AccessKind.Read or AccessKind.ReadWrite )
                        {
                            var treeNode = this._tree;

                            for ( var i = 0; i < stemCount; ++i)
                            {
                                var s = this._accessSymbols[i];

                                treeNode = treeNode.GetOrAddChild( s.Symbol );
                            }

                            treeNode.AddReferencedBy( this._tree.GetOrAddChild( this._originSymbol ) );
                        }
                    }
                }

                this.ClearCurrentAccessor();
            }

            --this._depth;
        }

        private void ClearCurrentAccessor()
        {
            this._accessorStartDepth = 0;
            this._accessSymbols.Clear();
        }

        public override void VisitInvocationExpression( InvocationExpressionSyntax node )
        {
            var invocationSymbol = this._semanticModel.GetSymbolInfo( node ).Symbol;

            if ( invocationSymbol == null )
            {
                this.ReportWarningNotImplementedForDependencyAnalysis( node.GetLocation() );
            }
            else if ( this._declaringType.Equals( invocationSymbol.ContainingType ) )
            {
                // TODO: Remove this warning when we support calling local instance methods.
                this._reportDiagnostic(
                    DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Calls to local methods are not supported." ),
                    node.GetLocation() );
            }
            else if ( !IsPrimitiveType( invocationSymbol.ContainingType ) )
            {
                this._reportDiagnostic(
                    DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Calls to external methods (except methods of primitive types) are not supported." ),
                    node.GetLocation() );
            }
            
            if ( node.Expression.IsKind( SyntaxKind.IdentifierName ) )
            {
                // Like "Fn(...)", no 'this.'. 'this' must be the declaring type, or it's a static method.
            }
            else if ( node.Expression is MemberAccessExpressionSyntax memberAccess )
            {
                this.ValidateMethodArgumentType( memberAccess.Expression );
            }
            else
            {
                this.ReportWarningNotImplementedForDependencyAnalysis( node.GetLocation() );
            }

            foreach ( var arg in node.ArgumentList.Arguments )
            {
                this.ValidateMethodArgumentType( arg.Expression );
            }

            base.VisitInvocationExpression( node );
        }

        public override void VisitVariableDeclaration( VariableDeclarationSyntax node )
        {
            // TODO: Use more correct diagnostic.
            this._reportDiagnostic(
                DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Variable declarations are not supported." ),
                node.GetLocation() );
        }

        public override void VisitLocalFunctionStatement( LocalFunctionStatementSyntax node )
        {
            // TODO: Use more correct diagnostic.
            this._reportDiagnostic(
                DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Local function declarations are not supported." ),
                node.GetLocation() );
        }

        public override void VisitBinaryExpression( BinaryExpressionSyntax node )
        {
            if ( node.IsKind( SyntaxKind.CoalesceExpression ) )
            {
                // This creates multiple potential access expressions.

                this._reportDiagnostic(
                    DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Coalesce expressions are not supported." ),
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

                if ( symbol != null )
                {
                    this._accessSymbols.Add( ( symbol, node ) );
                }
                else
                {
                    // TODO: When can this happen?
                    this.ReportWarningNotImplementedForDependencyAnalysis( node.GetLocation() );
                    this.ClearCurrentAccessor();
                }
                /*
                if ( symbol is IPropertySymbol property )
                {
                    this._accessSymbols.Add( property );
                    this._lastVisitedNode = node;
                }                
                else
                {
                    // Not a property (eg, it's a method or field).
                    this._reportDiagnostic(
                        DiagnosticDescriptors.ErrorMiscUnsupportedIdentifier
                            .WithArguments( (node.Identifier.Text, symbol == null ? "<unresolved>" : symbol.Kind.ToString()) ),
                        node.GetLocation() );

                    this.ClearCurrentAccessor();
                }
                */
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