// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;
using Microsoft.CodeAnalysis;
using System.Text;

namespace Metalama.Patterns.Observability.Implementation;

[CompileTime]
internal abstract class ProcessingNode<TDerived, TReadOnlyDerivedInterface> :
    IReadOnlyProcessingNode,
    IDependencyNode<TDerived>,
    IInitializableNode<TDerived, ProcessingNodeInitializationContext>
    where TDerived : ProcessingNode<TDerived, TReadOnlyDerivedInterface>, TReadOnlyDerivedInterface, new()
    where TReadOnlyDerivedInterface :
    IReadOnlyProcessingNode,
    IDependencyNode<TReadOnlyDerivedInterface>
{
    private TDerived? _parent;
    private ISymbol? _symbol;
    private IFieldOrProperty? _fieldOrProperty;
    private string? _dottedPropertyPath;
    private string? _contiguousPropertyPath;
    private TDerived[]? _children;
    private TDerived[]? _referencedBy;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessingNode{TDerived, TReadOnlyDerivedInterface}"/> class which represents the root node of a tree.
    /// </summary>
    protected ProcessingNode() { }

    /// <summary>
    /// Initializes the node during duplication from a <see cref="DependencyNode"/> graph.
    /// </summary>
    void IInitializableNode<TDerived, ProcessingNodeInitializationContext>.Initialize(
        ProcessingNodeInitializationContext? initializationContext,
        int depth,
        ISymbol? symbol,
        TDerived? parent,
        TDerived[]? children,
        TDerived[]? referencedBy )
    {
        if ( initializationContext == null )
        {
            throw new ArgumentNullException( nameof(initializationContext) );
        }

        // NB: Nodes are initialized in GraphExtensions.DescendantsDepthFirst{T}(T) order.

        this.Depth = depth;
        this._symbol = symbol;
        this._parent = parent;
        this._children = children;
        this._referencedBy = referencedBy;

        this._fieldOrProperty = (IFieldOrProperty?) (symbol == null ? null : initializationContext.Compilation.GetDeclaration( symbol ));

        this.AfterInitializableNodeInitialize( initializationContext );
    }

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual void AfterInitializableNodeInitialize( ProcessingNodeInitializationContext initializationContext ) { }

    public bool IsRoot => this._parent == null;

    public TDerived Parent => this._parent ?? throw DependencyNodeExtensions.NewNotSupportedOnRootNodeException();

    /// <summary>
    /// Gets the depth of a tree node. The root node has depth zero, the children of the root node have depth 1, and so on.
    /// </summary>
    public int Depth { get; private set; }

    // ReSharper disable once MemberCanBePrivate.Global
    /// <summary>
    /// Gets the Roslyn symbol of the node. Use <see cref="FieldOrProperty"/> for the Metalama equivalent.
    /// </summary>
    /// <exception cref="NotSupportedException"><see cref="IsRoot"/> is <see langword="true"/>.</exception>
    public ISymbol Symbol => this._symbol ?? throw DependencyNodeExtensions.NewNotSupportedOnRootNodeException();

    /// <summary>
    /// Gets the Metalama <see cref="IFieldOrProperty"/> for the node. Use <see cref="Symbol"/> for the Roslyn equivalent.
    /// </summary>
    public IFieldOrProperty FieldOrProperty => this._fieldOrProperty ?? throw DependencyNodeExtensions.NewNotSupportedOnRootNodeException();

    /// <summary>
    /// Gets a property path like "A1" or "A1.B1".
    /// </summary>
    public string DottedPropertyPath
        => this._dottedPropertyPath ??=
            this.IsRoot
                ? throw DependencyNodeExtensions.NewNotSupportedOnRootNodeException()
                : this.Parent.IsRoot
                    ? this.Name
                    : $"{this.Parent.DottedPropertyPath}.{this.Name}";

    /// <summary>
    /// Gets a property path like "A1" or "A1B1".
    /// </summary>
    public string ContiguousPropertyPath
        => this._contiguousPropertyPath ??=
            this.IsRoot
                ? throw DependencyNodeExtensions.NewNotSupportedOnRootNodeException()
                : this.Parent.IsRoot
                    ? this.Name
                    : $"{this.Parent.ContiguousPropertyPath}{this.Name}";

    /// <summary>
    /// Gets the name of the field or property for the node.
    /// </summary>
    public string Name => this.Symbol.Name;

    public IReadOnlyCollection<TDerived> Children => (IReadOnlyCollection<TDerived>?) this._children ?? Array.Empty<TDerived>();

    public bool HasChildren => this._children != null;

    public bool HasReferencedBy => this._referencedBy != null;

    /// <summary>
    /// Gets the direct references to the current node. Indirect references are not included.
    /// </summary>
    public IReadOnlyCollection<TDerived> ReferencedBy => (IReadOnlyCollection<TDerived>?) this._referencedBy ?? Array.Empty<TDerived>();

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

    // ReSharper disable once MemberCanBePrivate.Global
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

        var allRefs = ((TDerived) this).AllReferencedBy();

        if ( allRefs.Count > 0 )
        {
            appendTo.Append( " [ " ).Append( string.Join( ", ", allRefs.Select( n => n.Name ).OrderBy( n => n ) ) ).Append( " ]" );
        }

        this.ToStringAppendToLine( appendTo, format );

        appendTo.AppendLine();

        if ( this._children != null )
        {
            indent += 2;

            foreach ( var child in this._children.OrderBy( c => c.Name ) )
            {
                child.ToString( appendTo, indent, shouldHighlight );
            }
        }
    }

    string IReadOnlyProcessingNode.ToString( object? highlight, string? format ) => this.ToString( highlight as TDerived, format );
}