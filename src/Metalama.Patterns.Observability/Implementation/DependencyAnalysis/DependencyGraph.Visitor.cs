﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// To avoid confusion due to some identical type names, this file is concerned only with Roslyn types.
// Do not add usings for the namespaces Metalama.Framework.Code, Metalama.Framework.Engine.CodeModel or
// related namespaces.

using Metalama.Framework.Aspects;
using Metalama.Framework.Engine.CodeModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

internal static partial class DependencyGraph
{
    private static void AddReferencedProperties(
        Framework.Code.ICompilation compilation,
        Node tree,
        IPropertySymbol property,
        IGraphBuildingContext context,
        RoslynAssets assets,
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

        var visitor = new Visitor( tree, property, semanticModel, context, assets, trace, cancellationToken );

        visitor.Visit( body );
    }

    [CompileTime]
    private sealed partial class Visitor : CSharpSyntaxWalker, IGatherIdentifiersContextManagerClient
    {
        private readonly IGraphBuildingNode _tree;
        private readonly INamedTypeSymbol _declaringType;
        private readonly ISymbol _originSymbol;
        private readonly SemanticModel _semanticModel;
        private readonly CancellationToken _cancellationToken;
        private readonly Action<string>? _trace;
        private readonly IGraphBuildingContext _context;
        private readonly RoslynAssets _assets;

        private readonly GatherIdentifiersContextManager _gatherManager;

        private int _depth = 1;

        public Visitor(
            IGraphBuildingNode tree,
            ISymbol originSymbol,
            SemanticModel semanticModel,
            IGraphBuildingContext context,
            RoslynAssets assets,
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
            this._assets = assets;
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

                        if ( !elementaryType.IsPrimitiveType( this._assets ) )
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

        private void ValidateChainSymbol( IReadOnlyList<GatherIdentifiersContext.SymbolRecord> symbols, int index, ChainSection chainSection )
        {
            // Here we report particularly those diagnostics which should be located on a syntax node inside a body. The final dependency
            // graph does not keep track of all the syntax nodes which referenced each dependency node (this would be expensive), so logging
            // diagnostics located on syntax nodes inside bodies is not possible later.

            var sr = symbols[index];

            var fieldOrPropertyType = sr.Symbol switch
            {
                IPropertySymbol property => property.Type.GetElementaryType(),
                IFieldSymbol field => field.Type.GetElementaryType(),
                _ => chainSection == ChainSection.Unsupported ? null : throw new NotSupportedException()
            };

            if ( chainSection == ChainSection.Stem )
            {
                // Warn if this is a non-leaf reference to a non-primitive or non-INPC type.

                if ( !fieldOrPropertyType.IsPrimitiveType( this._assets ) && !this._context.TreatAsImplementingInpc( fieldOrPropertyType! ) )
                {
                    this._context.ReportDiagnostic(
                        DiagnosticDescriptors.WarningChildrenOfNonInpcFieldsOrPropertiesAreNotObservable.WithArguments( fieldOrPropertyType! ),
                        sr.Node.GetLocation() );
                }

                // Warn if this is a non-leaf reference to an INPC non-auto property of the target type because we can't
                // (yet) track changes to children of indirectly-referenced INPC properties.

                if ( !this._context.IsAutoPropertyOrField( sr.Symbol )
                     && this._context.TreatAsImplementingInpc( fieldOrPropertyType! )
                     && sr.Symbol.ContainingType.Equals( this._declaringType ) )
                {
                    this._context.ReportDiagnostic(
                        DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis
                            .WithArguments(
                                "Changes to children of non-auto properties declared on the current type, where the property type implements INotifyPropertyChanged, cannot be observed." ),
                        symbols[index + 1].Node.GetLocation() );
                }
            }

            if ( sr.Symbol is IFieldSymbol fieldSymbol )
            {
                if ( !(!fieldSymbol.IsStatic && fieldSymbol.EffectiveAccessibility() == Accessibility.Private)
                     && !fieldSymbol.ContainingType.IsPrimitiveType( this._assets )
                     && !(fieldSymbol.IsReadOnly && fieldSymbol.Type.IsPrimitiveType( this._assets ))
                     && !this._context.IsConfiguredAsSafe( sr.Symbol ) )
                {
                    this._context.ReportDiagnostic(
                        DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis
                            .WithArguments(
                                "Only private instance fields of the current type, fields belonging to primitive types, readonly fields of primitive types, and fields configured as safe for dependency analysis are supported." ),
                        sr.Node.GetLocation() );
                }
            }
            else if ( sr.Symbol.Kind is SymbolKind.Method )
            {
                bool? isConfiguredAsSafe = null;
                var containingType = sr.Symbol.ContainingType;

                if ( this._declaringType.IsOrInheritsFrom( containingType ) )
                {
                    if ( !sr.Symbol.IsStatic )
                    {
                        isConfiguredAsSafe ??= this._context.IsConfiguredAsSafe( sr.Symbol );

                        if ( isConfiguredAsSafe != true )
                        {
                            this._context.ReportDiagnostic(
                                DiagnosticDescriptors.WarningMethodOrPropertyIsNotSupportedForDependencyAnalysis.WithArguments( (sr.Symbol.Kind, sr.Symbol) ),
                                sr.Node.GetLocation() );
                        }
                    }
                }
                else if ( !containingType.IsPrimitiveType( this._assets ) )
                {
                    // Only members of primitive types are implicitly safe to access.

                    isConfiguredAsSafe ??= this._context.IsConfiguredAsSafe( sr.Symbol );

                    if ( isConfiguredAsSafe != true )
                    {
                        this._context.ReportDiagnostic(
                            DiagnosticDescriptors.WarningMethodOrPropertyIsNotSupportedForDependencyAnalysis.WithArguments( (sr.Symbol.Kind, sr.Symbol) ),
                            sr.Node.GetLocation() );
                    }
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

                    // Skip chains which are not read, but are only the target of an assignment.

                    if ( symbols[^1].Node.GetAccessKind() is not (AccessKind.Read or AccessKind.ReadWrite) )
                    {
                        continue;
                    }

                    // Only consider chains that start with a local member. And for now, only fields and properties.
                    // TODO: Expand logic here to support methods.

                    var firstSymbol = symbols[0].Symbol;

                    var supportedStemAndLeafCount = this.IsLocalInstanceMember( firstSymbol )
                        ? symbols.TakeWhile(
                                sr => sr.Symbol.Kind == SymbolKind.Property
                                      || (sr.Symbol.Kind == SymbolKind.Field && sr.Symbol.EffectiveAccessibility() == Accessibility.Private) )
                            .Count()
                        : 0;

                    var treeNode = this._tree;

                    for ( var i = 0; i < symbols.Count; ++i )
                    {
                        var chainSection =
                            i < supportedStemAndLeafCount - 1
                                ? ChainSection.Stem
                                : i < supportedStemAndLeafCount
                                    ? ChainSection.Leaf
                                    : ChainSection.Unsupported;

                        this.ValidateChainSymbol( symbols, i, chainSection );

                        if ( chainSection is ChainSection.Stem or ChainSection.Leaf )
                        {
                            treeNode = treeNode.GetOrAddChild( symbols[i].Symbol );
                        }
                    }

                    if ( supportedStemAndLeafCount > 0 )
                    {
                        treeNode.AddReferencedBy( this._tree.GetOrAddChild( this._originSymbol ) );
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
            var symbolInfo = this._semanticModel.GetSymbolInfo( node.Type, this._cancellationToken );
            var variableType = ((ITypeSymbol?) symbolInfo.Symbol)?.GetElementaryType();

            if ( variableType != null && !(variableType.IsPrimitiveType( this._assets ) || this._context.IsConfiguredAsSafe( variableType )) )
            {
                this._context.ReportDiagnostic(
                    DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments(
                        "Variables of types other than primitive types and types configured as safe for dependency analysis are not supported." ),
                    node.GetLocation() );
            }

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

            var symbol = this._semanticModel.GetSymbolInfo( node, this._cancellationToken ).Symbol;

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