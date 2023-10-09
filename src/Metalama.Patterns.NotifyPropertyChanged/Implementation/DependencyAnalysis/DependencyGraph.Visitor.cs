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

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;

internal static partial class DependencyGraph
{
    private static void AddReferencedProperties(
        ICompilation compilation,
        Node tree,
        IPropertySymbol property,
        IGraphBuildingContext context,
        Action<string>? trace = null,
        CancellationToken cancellationToken = default )
    {
        var body = property
            .DeclaringSyntaxReferences
            .Select( r => r.GetSyntax( cancellationToken ) )
            .Cast<PropertyDeclarationSyntax>()
            .Select( RoslynExtensions.GetGetterBody )
            .SingleOrDefault();

        if ( body == null )
        {
            return;
        }

        // ReSharper disable once InvokeAsExtensionMethod
        var semanticModel = SymbolExtensions.GetSemanticModel( compilation, body.SyntaxTree );

        var visitor = new Visitor( tree, property, semanticModel, context, trace, cancellationToken );

        visitor.Visit( body );
    }

    [CompileTime]
    private sealed class Visitor : CSharpSyntaxWalker, IGatherIdentifiersContextManagerClient
    {
        private readonly IGraphBuildingNode _tree;
        private readonly INamedTypeSymbol _declaringType;
        private readonly ISymbol _originSymbol;
        private readonly SemanticModel _semanticModel;
        private readonly CancellationToken _cancellationToken;
        private readonly Action<string>? _trace;
        private readonly IGraphBuildingContext _context;

        private readonly GatherIdentifiersContextManager _gatherManager;

        private int _depth = 1;

        public Visitor(
            IGraphBuildingNode tree,
            ISymbol originSymbol,
            SemanticModel semanticModel,
            IGraphBuildingContext context,
            Action<string>? trace,
            CancellationToken cancellationToken )
        {
            this._trace = trace;
            this._tree = tree;
            this._declaringType = originSymbol.ContainingType;
            this._originSymbol = originSymbol;
            this._semanticModel = semanticModel;
            this._cancellationToken = cancellationToken;
            this._context = context;
            this._gatherManager = new GatherIdentifiersContextManager( this );
        }

        void IGatherIdentifiersContextManagerClient.OnRootContextPopped( GatherIdentifiersContext context )
        {
            this.ProcessAndResetIfApplicable( context );
        }

        /// <summary>
        /// Called when an unexpected scenario is encountered during processing. Indicates that the program logic of <see cref="DependencyGraph"/> is not yet implemented
        /// to handle that scenario.
        /// </summary>
        private void ReportWarningNotImplementedForDependencyAnalysis(
            Location location,
            [CallerMemberName] string? name = null,
            [CallerLineNumber] int line = 0 )
        {
            this._context.ReportDiagnostic(
                DiagnosticDescriptors.WarningNotImplementedForDependencyAnalysis.WithArguments( $"{name}/{line}" ),
                location );
        }

        private bool IsLocalInstanceMember( ISymbol symbol ) => !symbol.IsStatic && this._declaringType.IsOrInheritsFrom( symbol.ContainingType );

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
                        var elementaryType = ti.Type.GetElementaryType();

                        if ( !elementaryType.IsPrimitiveType() )
                        {
                            this._context.ReportDiagnostic(
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
                    this._trace?.Invoke( $"Processing symbol chain {string.Join( ".", symbols.Select( sr => sr.Symbol.Name ) )}" );

                    // Only consider chains that start with a local member. And for now, only properties.
                    // TODO: Expand logic here to support fields and methods.

                    var firstSymbol = symbols[0].Symbol;

                    if ( this.IsLocalInstanceMember( firstSymbol ) )
                    {
                        var stemCount = symbols.Count( sr => sr.Symbol.Kind == SymbolKind.Property );

                        if ( stemCount > 0 )
                        {
                            var final = symbols[stemCount - 1];

                            if ( final.Node.GetAccessKind() is AccessKind.Read or AccessKind.ReadWrite )
                            {
                                var treeNode = this._tree;

                                for ( var i = 0; i < stemCount; ++i )
                                {
                                    var sr = symbols[i];

                                    treeNode = treeNode.GetOrAddChild( sr.Symbol );

                                    var propertyType = ((IPropertySymbol) sr.Symbol).Type.GetElementaryType();

                                    // Warn if this is a non-leaf reference to a non-primitive or non-INPC type.
                                    if ( i < stemCount - 1 && !propertyType.IsPrimitiveType() && !this._context.TreatAsImplementingInpc( propertyType ) )
                                    {
                                        this._context.ReportDiagnostic(
                                            DiagnosticDescriptors.WarningChildrenOfNonInpcFieldsOrPropertiesAreNotObservable.WithArguments( propertyType ),
                                            sr.Node.GetLocation() );
                                    }
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
            bool? isConfiguredAsSafeToCall = null;

            if ( this._semanticModel.GetSymbolInfo( node ).Symbol is not IMethodSymbol methodSymbol )
            {
                // Undefined name, will be a compiler error (eg, "The name 'X' does not exist in the current context").
                this._gatherManager.Current.Reset();

                return;
            }

            if ( this._declaringType.IsOrInheritsFrom( methodSymbol.ContainingType ) )
            {
                if ( !methodSymbol.IsStatic )
                {
                    isConfiguredAsSafeToCall ??= this._context.IsConfiguredAsSafeToCall( methodSymbol );

                    if ( isConfiguredAsSafeToCall != true )
                    {
                        this._context.ReportDiagnostic(
                            DiagnosticDescriptors.WarningMethodIsNotSupportedForDependencyAnalysis.WithArguments( methodSymbol ),
                            node.GetLocation() );
                    }
                }
            }
            else if ( !methodSymbol.ContainingType.IsPrimitiveType() )
            {
                // Only methods of primitive types are implicitly safe to call.

                isConfiguredAsSafeToCall ??= this._context.IsConfiguredAsSafeToCall( methodSymbol );

                if ( isConfiguredAsSafeToCall != true )
                {
                    this._context.ReportDiagnostic(
                        DiagnosticDescriptors.WarningMethodIsNotSupportedForDependencyAnalysis.WithArguments( methodSymbol ),
                        node.GetLocation() );
                }
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
            // TODO: Allow variables of primitive types only, as this is safe and does not require more complex handling elsewhere. TP-33948
#if false
            // TODO: Use more correct diagnostic.
            this._context.ReportDiagnostic(
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
                // Undefined name, will be a compiler error (eg, "The name 'X' does not exist in the current context").
                ctx.Reset();
            }
        }
    }
}