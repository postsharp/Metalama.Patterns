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
        CancellationToken cancellationToken,
        Action<string>? trace = null)
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

        var visitor = new Visitor( tree, property, semanticModel, reportDiagnostic, cancellationToken, trace );
        
        visitor.Visit( body );
    }

    [CompileTime]
    private sealed class Visitor : CSharpSyntaxWalker
    {
        private sealed class GatherIdentifiersContext
        {
            public readonly record struct SymbolRecord( ISymbol Symbol, SyntaxNode Node, int Depth);

            private sealed class ForkItem
            {
                public GatherIdentifiersContext Fork { get; set; } = null!;

                public bool IsJoined { get; set; }
            }

            private readonly GatherIdentifiersContext _rootContext;
            private readonly ForkItem? _forkItem;
            
            private List<SymbolRecord>? _symbols;
            private List<ForkItem>? _forks;

            // NB: Only set on _rootContext.
            private List<ForkItem>? _allForks;

            public GatherIdentifiersContext()
            {
                this._rootContext = this;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="GatherIdentifiersContext"/> class as a fork of
            /// another <see cref="GatherIdentifiersContext"/>.
            /// </summary>
            /// <param name="parent"></param>
            private GatherIdentifiersContext( GatherIdentifiersContext rootContext, IReadOnlyCollection<SymbolRecord>? parentSymbols, ForkItem forkItem )
            {
                this._rootContext = rootContext;
                this._symbols = parentSymbols == null ? null : new List<SymbolRecord>( parentSymbols );
                this._forkItem = forkItem;
            }

            private void ThrowIfJoined()
            {
                if ( this._forkItem?.IsJoined == true )
                {
                    throw new NotSupportedException( "The " + nameof( GatherIdentifiersContext ) + " fork as been rejoined to its parent and cannot be modified directly." );
                }
            }

            private bool HasUnjoinedForks => this._forks != null && this._forks.Any( f => !f.IsJoined );

            public GatherIdentifiersContext Fork( int startDepth )
            {
                this.ThrowIfJoined();

                if ( startDepth < this.StartDepth )
                {
                    throw new ArgumentException( "Must be greater than or equal to the " + nameof( this.StartDepth ) + " of the current " + nameof( GatherIdentifiersContext ) + "." );
                }

                var forkItem = new ForkItem();
                var fork = new GatherIdentifiersContext( this._rootContext, this._symbols, forkItem ) { StartDepth = startDepth };
                forkItem.Fork = fork;

                (this._rootContext._allForks ??= new List<ForkItem>()).Add( forkItem );
                (this._forks ??= new List<ForkItem>()).Add( forkItem );

                return fork;
            }

            /// <summary>
            /// Join a forked context back to its parent.
            /// </summary>
            public void Join()
            {
                this.ThrowIfJoined();

                if ( this._forkItem == null )
                {
                    throw new InvalidOperationException( "The current " + nameof( GatherIdentifiersContext ) + " is not a fork so cannot be joined." );
                }

                if ( this.HasUnjoinedForks )
                {
                    throw new InvalidOperationException( "The current " + nameof( GatherIdentifiersContext ) + " has unjoined forks, so canot be joined." );
                }

                this._forkItem.IsJoined = true;
            }

            public void EnsureInitialized( int startDepth )
            {
                this.ThrowIfJoined();

                if ( startDepth <= 0 )
                {
                    throw new ArgumentException( "Must be greater than zero." );
                }

                if ( this.StartDepth == 0 )
                {
                    this.StartDepth = startDepth;
                }
            }

            public void Reset()
            {
                this.ThrowIfJoined();
                if ( this.HasUnjoinedForks )
                {
                    throw new InvalidOperationException( "The current " + nameof( GatherIdentifiersContext ) + " has unjoined forks." );
                }

                this.StartDepth = 0;
                this._symbols?.Clear();
                this._forks?.Clear();
            }

            public int StartDepth { get; private set; }

            public void AddSymbol( ISymbol symbol, SyntaxNode node, int depth )
            {
                this.ThrowIfJoined();
                var record = new SymbolRecord( symbol, node, depth );
                this.AddSymbolUnsafe( record );
            }

            private void AddSymbolUnsafe( in SymbolRecord record )
            {                
                (this._symbols ??= new()).Add( record );

                if ( this._forks != null )
                {
                    foreach ( var f in this._forks )
                    {
                        if ( f.IsJoined )
                        {
                            f.Fork.AddSymbolUnsafe( record );
                        }
                    }
                }
            }

            public IReadOnlyList<SymbolRecord> Symbols
                => (IReadOnlyList<SymbolRecord>?) this._symbols ?? Array.Empty<SymbolRecord>();

            public IEnumerable<IReadOnlyList<SymbolRecord>> SymbolsForAllForks()
            {
                if ( this != this._rootContext )
                {
                    throw new InvalidOperationException( nameof( this.SymbolsForAllForks ) + " must only be alled on a root " + nameof( GatherIdentifiersContext ) + "." );
                }

                if ( this._allForks != null && this._allForks.Any( f => !f.IsJoined ) )
                {
                    throw new InvalidOperationException( "The root " + nameof( GatherIdentifiersContext ) + " has unjoined forks." );
                }

                if ( this._symbols != null && this._symbols.Count > 0 )
                {
                    yield return this._symbols;
                }

                if ( this._allForks != null )
                {
                    foreach ( var f in this._allForks )
                    {
                        if ( f.Fork._symbols != null && f.Fork._symbols.Count > 0 )
                        {
                            yield return f.Fork._symbols;
                        }
                    }
                }
            }
        }

        private readonly IGraphBuildingNode _tree;
        private readonly INamedTypeSymbol _declaringType;
        private readonly ISymbol _originSymbol;
        private readonly SemanticModel _semanticModel;
        private readonly ReportDiagnostic _reportDiagnostic;
        private readonly CancellationToken _cancellationToken;
        private readonly Action<string>? _trace;

        private readonly Stack<GatherIdentifiersContext> _gatherIdentifiersContexts;
        
        private int _depth = 1;

        public Visitor(
            IGraphBuildingNode tree,
            ISymbol originSymbol,
            SemanticModel semanticModel,
            ReportDiagnostic reportDiagnostic,
            CancellationToken cancellationToken,
            Action<string>? trace )
        {
            this._trace = trace;
            this._tree = tree;
            this._declaringType = originSymbol.ContainingType;
            this._originSymbol = originSymbol;
            this._semanticModel = semanticModel;
            this._reportDiagnostic = reportDiagnostic;
            this._cancellationToken = cancellationToken;

            this._gatherIdentifiersContexts = new Stack<GatherIdentifiersContext>();
            this._gatherIdentifiersContexts.Push(new GatherIdentifiersContext());
        }

        private void ReportWarningNotImplementedForDependencyAnalysis( Location location, [CallerMemberName] string? name = null, [CallerLineNumber] int line = 0)
        {
            this._reportDiagnostic(
                DiagnosticDescriptors.WarningNotImplementedForDependencyAnalysis.WithArguments( $"{name}/{line}" ),
                location );
        }

        private static bool IsOrInheritsFrom( INamedTypeSymbol type, ITypeSymbol? candidateBaseType )
        {
            if ( type == null )
            {
                throw new ArgumentNullException( nameof( type ) );
            }

            if ( candidateBaseType == null )
            {
                return false;
            }

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
            => !symbol.IsStatic && IsOrInheritsFrom( this._declaringType, symbol.ContainingType );

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

        private void PushGatherIdentifiersContext()
        {
            this._gatherIdentifiersContexts.Push( new GatherIdentifiersContext() );
        }

        private void PopGatherIdenfiersContext()
        {
            var ctx = this._gatherIdentifiersContexts.Pop();
            this.ProcessAndResetIfApplicable( ctx );
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

            this.ProcessAndResetIfApplicable( this._gatherIdentifiersContexts.Peek() );

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
            this.PushGatherIdentifiersContext();

            base.VisitArgument( node );

            this.PopGatherIdenfiersContext();
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
            // TODO: For now we don't try to examine the body of local functions - much as we don't examine the body of
            // regular methods. Invocations of the local function will warn if they are not supported (primitive arg types only etc).            
#if false
            // TODO: Use more correct diagnostic.
            this._reportDiagnostic(
                DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Local function declarations are not supported." ),
                node.GetLocation() );
#endif
        }

        public override void VisitConditionalExpression( ConditionalExpressionSyntax node )
        {
            this.PushGatherIdentifiersContext();

            base.Visit( node.Condition );

            this.PopGatherIdenfiersContext();

            var current = this._gatherIdentifiersContexts.Peek();
            current.EnsureInitialized( this._depth );

            var forkWhenFalse = current.Fork( this._depth );

            this.Visit( node.WhenTrue);

            this._gatherIdentifiersContexts.Push( forkWhenFalse );

            this.Visit( node.WhenFalse );

            this._gatherIdentifiersContexts.Pop();

            forkWhenFalse.Join();
        }

        public override void VisitBinaryExpression( BinaryExpressionSyntax node )
        {
#if true
            if ( node.IsKind( SyntaxKind.CoalesceExpression ) )
            {
                var current = this._gatherIdentifiersContexts.Peek();
                current.EnsureInitialized( this._depth );

                var forkRight = current.Fork( this._depth );

                this.Visit( node.Left );

                this._gatherIdentifiersContexts.Push( forkRight );

                this.Visit( node.Right );

                this._gatherIdentifiersContexts.Pop();

                forkRight.Join();
            }
            else
            {
                base.VisitBinaryExpression( node );
            }

#else
            if ( node.IsKind( SyntaxKind.CoalesceExpression ) )
            {
                // This creates multiple potential access expressions.

                this._reportDiagnostic(
                    DiagnosticDescriptors.WarningNotSupportedForDependencyAnalysis.WithArguments( "Coalesce expressions are not supported." ),
                    node.GetLocation() );

                // Best effort
                this.ProcessAndResetIfApplicable( this._gatherIdentifiersContexts.Peek() );
            }
            else
            {
                base.VisitBinaryExpression( node );
            }
#endif
        }

        public override void VisitMemberAccessExpression( MemberAccessExpressionSyntax node )
        {
            this._gatherIdentifiersContexts.Peek().EnsureInitialized( this._depth );

            base.VisitMemberAccessExpression( node );
        }

        public override void VisitConditionalAccessExpression( ConditionalAccessExpressionSyntax node )
        {
            this._gatherIdentifiersContexts.Peek().EnsureInitialized( this._depth );

            base.VisitConditionalAccessExpression( node );
        }

        public override void VisitIdentifierName( IdentifierNameSyntax node )
        {
            var ctx = this._gatherIdentifiersContexts.Peek();

            var symbol = this._semanticModel.GetSymbolInfo( node ).Symbol;

            // Ignore identifiers with no symbol

            if ( symbol != null )
            {
                if ( ctx.StartDepth == 0 )
                {
                    // This happens when an identifier is used without qualification,
                    // eg. X in `int Y => X`
                    if ( node.GetAccessKind() is AccessKind.Read or AccessKind.ReadWrite )
                    {
                        ctx.EnsureInitialized( this._depth );
                    }
                }

                if ( ctx.StartDepth > 0 )
                {
                    if ( symbol != null )
                    {
                        ctx.AddSymbol( symbol, node, this._depth );
                    }
                    else
                    {
                        // eg, 'var', NameColon in argument list.
                        // TODO: When can this happen?
                        this.ReportWarningNotImplementedForDependencyAnalysis( node.GetLocation() );

                        // Best effort
                        this.ProcessAndResetIfApplicable( this._gatherIdentifiersContexts.Peek() );
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