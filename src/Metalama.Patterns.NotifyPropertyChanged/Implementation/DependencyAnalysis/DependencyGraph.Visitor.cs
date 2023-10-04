// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// To avoid confusion due to some identical type names, this file is concerned only with Roslyn types.
// Do not add usings for the namespaces Metalama.Framework.Code, Metalama.Framework.Engine.CodeModel or
// related namespaces.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;
using SpecialType = Microsoft.CodeAnalysis.SpecialType;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;

internal static partial class DependencyGraph
{
    private static void AddReferencedProperties(
        ICompilation compilation,
        Node tree,
        IPropertySymbol property,
        ReportDiagnostic reportDiagnostic,
        Action<string>? trace = null,
        CancellationToken cancellationToken = default )
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

        // ReSharper disable once InvokeAsExtensionMethod
        var semanticModel = SymbolExtensions.GetSemanticModel( compilation, body.SyntaxTree );

        var visitor = new Visitor( tree, property, semanticModel, reportDiagnostic, trace, cancellationToken );

        visitor.Visit( body );
    }

    [CompileTime]
    private sealed class Visitor : CSharpSyntaxWalker, IGatherIdentifiersContextManagerClient
    {
        private readonly IGraphBuildingNode _tree;
        private readonly INamedTypeSymbol _declaringType;
        private readonly ISymbol _originSymbol;
        private readonly SemanticModel _semanticModel;
        private readonly ReportDiagnostic _reportDiagnostic;
        private readonly CancellationToken _cancellationToken;
        private readonly Action<string>? _trace;

        private readonly GatherIdentifiersContextManager _gatherManager;

        private int _depth = 1;

        public Visitor(
            IGraphBuildingNode tree,
            ISymbol originSymbol,
            SemanticModel semanticModel,
            ReportDiagnostic reportDiagnostic,
            Action<string>? trace,
            CancellationToken cancellationToken )
        {
            this._trace = trace;
            this._tree = tree;
            this._declaringType = originSymbol.ContainingType;
            this._originSymbol = originSymbol;
            this._semanticModel = semanticModel;
            this._reportDiagnostic = reportDiagnostic;
            this._cancellationToken = cancellationToken;
            this._gatherManager = new GatherIdentifiersContextManager( this );
        }

        void IGatherIdentifiersContextManagerClient.OnRootContextPopped( GatherIdentifiersContext context )
        {
            this.ProcessAndResetIfApplicable( context );
        }

        private void ReportWarningNotImplementedForDependencyAnalysis(
            Location location,
            [CallerMemberName] string? name = null,
            [CallerLineNumber] int line = 0 )
        {
            this._reportDiagnostic(
                DiagnosticDescriptors.WarningNotImplementedForDependencyAnalysis.WithArguments( $"{name}/{line}" ),
                location );
        }

        private static bool IsOrInheritsFrom( INamedTypeSymbol type, ITypeSymbol? candidateBaseType )
        {
            if ( type == null )
            {
                throw new ArgumentNullException( nameof(type) );
            }

            if ( candidateBaseType == null )
            {
                return false;
            }

            var baseType = type;

            while ( baseType != null )
            {
                if ( candidateBaseType.Equals( baseType ) )
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        private bool IsLocalInstanceMember( ISymbol symbol ) => !symbol.IsStatic && IsOrInheritsFrom( this._declaringType, symbol.ContainingType );

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
            // ReSharper disable once MissingIndent
            return type is
                {
                SpecialType: SpecialType.System_Boolean or
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
                SpecialType.System_UInt64
            };
        }

        private void ValidateMethodArgumentType( ExpressionSyntax expression )
        {
            var ti = this._semanticModel.GetTypeInfo( expression );

            switch ( ti.Type )
            {
                case IErrorTypeSymbol:
                    // Don't report, assume that the compiler will report.
                    break;

                case null:
                    // TODO: Decide how to handle when we have a use case.
                    this.ReportWarningNotImplementedForDependencyAnalysis( expression.GetLocation() );

                    break;

                default:
                    {
                        var elementaryType = GetElementaryType( ti.Type );

                        if ( !IsPrimitiveType( elementaryType ) )
                        {
                            this._reportDiagnostic(
                                DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis
                                    .WithArguments( "Method arguments (including 'this') must be of primitive types." ),
                                expression.GetLocation() );
                        }

                        break;
                    }
            }
        }

        private void ProcessAndResetIfApplicable( GatherIdentifiersContext context )
        {
            this._trace?.Invoke( ">> ProcessAndResetIfApplicable" );

            if ( context.StartDepth == this._depth )
            {
                foreach ( var symbols in context.SymbolsForAllForks() )
                {
                    this._trace?.Invoke( $"Processing symbol chain {string.Join( ".", symbols.Select( s => s.Symbol.Name ) )}" );

                    // Only consider chains that start with a local member. And for now, only properties.
                    // TODO: Expand logic here to support fields and methods.

                    var firstSymbol = symbols[0].Symbol;

                    if ( this.IsLocalInstanceMember( firstSymbol ) )
                    {
                        var stemCount = symbols.Count( s => s.Symbol.Kind == SymbolKind.Property );

                        if ( stemCount > 0 )
                        {
                            var final = symbols[stemCount - 1];

                            if ( final.Node.GetAccessKind() is AccessKind.Read or AccessKind.ReadWrite )
                            {
                                var treeNode = this._tree;

                                for ( var i = 0; i < stemCount; ++i )
                                {
                                    var s = symbols[i];

                                    treeNode = treeNode.GetOrAddChild( s.Symbol );
                                }

                                treeNode.AddReferencedBy( this._tree.GetOrAddChild( this._originSymbol ) );
                            }
                        }
                    }
                }

                context.Reset();
            }

            this._trace?.Invoke( "<< ProcessAndResetIfApplicable" );
        }

        public override void Visit( SyntaxNode? node )
        {
            this._cancellationToken.ThrowIfCancellationRequested();

            ++this._depth;

            base.Visit( node );

            this.ProcessAndResetIfApplicable( this._gatherManager.Current );

            --this._depth;
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
                if ( !invocationSymbol.IsStatic )
                {
                    // TODO: Remove this warning when we support calling local instance methods.
                    this._reportDiagnostic(
                        DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Calls to local instance methods are not supported." ),
                        node.GetLocation() );
                }
            }

            // NB: Calls to external methods are safe so long as all args are primitive (including this)

            if ( node.Expression.IsKind( SyntaxKind.IdentifierName ) )
            {
                // Like "Fn(...)", no 'this.'. 'this' must be the declaring type, or it's a static method.
            }
            else if ( node.Expression is MemberAccessExpressionSyntax memberAccess )
            {
                if ( invocationSymbol?.IsStatic == false )
                {
                    this.ValidateMethodArgumentType( memberAccess.Expression );
                }
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

        public override void VisitArgument( ArgumentSyntax node )
        {
            using ( this._gatherManager.UseNewRootContext() )
            {
                base.VisitArgument( node );
            }
        }

        public override void VisitVariableDeclaration( VariableDeclarationSyntax node )
        {
            // TODO: At first glance, variables don't seem to be problematic. Why should they not be supported?
#if false
            // TODO: Use more correct diagnostic.
            this._reportDiagnostic(
                DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Variable declarations are not supported." ),
                node.GetLocation() );

            // Best effort
#endif
            base.VisitVariableDeclaration( node );
        }

        public override void VisitLocalFunctionStatement( LocalFunctionStatementSyntax node )
        {
            // TODO: For now we don't try to examine the body of local functions - much as we don't examine the body of regular methods.
            // Invocations of the local function will warn if they are not supported (primitive arg types only etc), but as with regular
            // methods, don't warn on the declaration.
        }

        public override void VisitConditionalExpression( ConditionalExpressionSyntax node )
        {
            // `Condition` is an isolated expression, use a new root context while visiting it.
            using ( this._gatherManager.UseNewRootContext() )
            {
                base.Visit( node.Condition );
            }

            var current = this._gatherManager.Current;
            current.EnsureStarted( this._depth );

            var forkWhenFalse = current.PrepareFork( this._depth );

            // Visit `WhenTrue` using the current context.
            this.Visit( node.WhenTrue );

            // Visit `WhenFalse` using the prepared fork.
            using ( forkWhenFalse.Use() )
            {
                this.Visit( node.WhenFalse );
            }
        }

        public override void VisitBinaryExpression( BinaryExpressionSyntax node )
        {
            if ( node.IsKind( SyntaxKind.CoalesceExpression ) )
            {
                var current = this._gatherManager.Current;
                current.EnsureStarted( this._depth );

                var forkRight = current.PrepareFork( this._depth );

                // Visit `Left` using the current context.
                this.Visit( node.Left );

                // Visit `Right` using the prepared fork.
                using ( forkRight.Use() )
                {
                    this.Visit( node.Right );
                }
            }
            else
            {
                base.VisitBinaryExpression( node );
            }
        }

        public override void VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            this._gatherManager.Current.EnsureStarted( this._depth );

            base.VisitMemberAccessExpression( node );
        }

        public override void VisitConditionalAccessExpression( ConditionalAccessExpressionSyntax node )
        {
            this._gatherManager.Current.EnsureStarted( this._depth );

            base.VisitConditionalAccessExpression( node );
        }

        public override void VisitIdentifierName( IdentifierNameSyntax node )
        {
            var ctx = this._gatherManager.Current;

            var symbol = this._semanticModel.GetSymbolInfo( node ).Symbol;

            if ( symbol != null )
            {
                if ( ctx.StartDepth == 0 )
                {
                    // This happens when an identifier is used without qualification,
                    // eg. X in `int Y => X`
                    if ( node.GetAccessKind() is AccessKind.Read or AccessKind.ReadWrite )
                    {
                        ctx.EnsureStarted( this._depth );
                    }
                }

                if ( ctx.StartDepth > 0 )
                {
                    ctx.AddSymbol( symbol, node, this._depth );
                }
            }
            else
            {
                // TODO: When can this happen?
                this.ReportWarningNotImplementedForDependencyAnalysis( node.GetLocation() );

                // Best effort
                this.ProcessAndResetIfApplicable( ctx );
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