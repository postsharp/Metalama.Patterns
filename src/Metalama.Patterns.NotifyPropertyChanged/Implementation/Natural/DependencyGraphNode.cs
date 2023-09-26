// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Text;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

[CompileTime]
internal interface IReadOnlyDependencyGraphNode : DependencyGraph.IReadOnlyNode<IReadOnlyDependencyGraphNode>
{
    /// <summary>
    /// Gets the <see cref="InpcInstrumentationKind"/> for the type of the field or property.
    /// </summary>
    InpcInstrumentationKind PropertyTypeInpcInstrumentationKind { get; }

    /// <summary>
    /// Gets a method like <c>void UpdateA2C2()</c>. 
    /// </summary>
    /// <remarks>
    /// In a given inheritance hierarchy, for a given property path, this method is defined in
    /// the type closest to the root type where a reference to a property of A2.C2 first occurs.
    /// The method is always private as it is only called by other members of the type where it
    /// is defined.
    /// </remarks>
    IReadOnlyUncertainDeferredDeclaration<IMethod> UpdateMethod { get; }

    /// <summary>
    /// Gets the non-null <see cref="UpdateMethod"/> declarations of the children of the current node.
    /// </summary>
    IReadOnlyCollection<IMethod> ChildUpdateMethods { get; }

    InpcBaseHandling InpcBaseHandling { get; }

    /// <summary>
    /// Gets the field like "PropertyChangedEventHandler? _onA2PropertyChangedHandler", if it has been introduced.
    /// </summary>
    IField? HandlerField { get; }

    /// <summary>
    /// Gets the field like "B? _lastA2", if it has been introduced.
    /// </summary>
    public IField? LastValueField { get; }
}

/// <summary>
/// Dependency graph node specialized for the current implementation strategy of <see cref="NotifyPropertyChangedAttribute"/>.
/// </summary>
[CompileTime]
internal sealed class DependencyGraphNode : DependencyGraph.Node<DependencyGraphNode, IReadOnlyDependencyGraphNode>, IReadOnlyDependencyGraphNode
{
    protected override void Initialize()
    {
        base.Initialize();
        
        if ( this.Depth == 1 )
        {
            this._subscribeMethod = new CertainDeferredDeclaration<IMethod>();
        }
    }

    /// <summary>
    /// Method called for each node once the graph has been built. Called in <see cref="DependencyGraph.Node{TDerived}.DescendantsDepthFirst"/> order.
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns>A value indicating success. <see langword="true"/> if initialized without error, or <see langword="false"/> if diagnostic errors were reported.</returns>
    public bool Initialize( BuildAspectContext ctx )
    {
        this.PropertyTypeInpcInstrumentationKind = ctx.InpcInstrumentationKindLookup.Get( this.FieldOrProperty.Type );
        this.InpcBaseHandling = DetermineInpcBaseHandling();

        return ctx.ValidateFieldOrProperty( this.FieldOrProperty );

        InpcBaseHandling DetermineInpcBaseHandling()
        {
            switch ( this.PropertyTypeInpcInstrumentationKind )
            {
                case InpcInstrumentationKind.Unknown:
                    return InpcBaseHandling.Unknown;

                case InpcInstrumentationKind.None:
                    return InpcBaseHandling.NotApplicable;

                case InpcInstrumentationKind.Implicit:
                case InpcInstrumentationKind.Explicit:
                    if ( this.Depth == 1 )
                    {
                        // Root property
                        return this.FieldOrProperty.DeclaringType == ctx.Target
                            ? InpcBaseHandling.NotApplicable
                            : ctx.HasInheritedOnChildPropertyChangedPropertyPath( this.Name )
                                ? InpcBaseHandling.OnChildPropertyChanged
                                : ctx.HasInheritedOnUnmonitoredObservablePropertyChangedProperty( this.Name )
                                    ? InpcBaseHandling.OnUnmonitoredObservablePropertyChanged
                                    : InpcBaseHandling.OnPropertyChanged;
                    }
                    else
                    {
                        // Child property
                        return ctx.HasInheritedOnChildPropertyChangedPropertyPath( this.DottedPropertyPath )
                            ? InpcBaseHandling.OnChildPropertyChanged
                            : ctx.HasInheritedOnUnmonitoredObservablePropertyChangedProperty( this.DottedPropertyPath )
                                ? InpcBaseHandling.OnUnmonitoredObservablePropertyChanged
                                : InpcBaseHandling.None;
                    }

                default:
                    throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Gets the <see cref="InpcInstrumentationKind"/> for the type of the field or property.
    /// </summary>
    public InpcInstrumentationKind PropertyTypeInpcInstrumentationKind { get; private set; }

    public InpcBaseHandling InpcBaseHandling { get; private set; }

    /// <summary>
    /// Gets the potentially uninitialized field like "B? _lastA2". Typically, use
    /// <see cref="BuildAspectContext.GetOrCreateLastValueField(DependencyGraphNode)"/> instead.
    /// </summary>
    public IField? LastValueField { get; private set; }

    /// <summary>
    /// Should only be called by <see cref="BuildAspectContext.GetOrCreateLastValueField(DependencyGraphNode)"/>.
    /// </summary>
    /// <param name="field"></param>
    public void SetLastValueField( IField field ) => this.LastValueField = field;

    /// <summary>
    /// Gets the potentially uninitialized field like "PropertyChangedEventHandler? _onA2PropertyChangedHandler".
    /// Typically, use <see cref="BuildAspectContext.GetOrCreateHandlerField(DependencyGraphNode)"/> instead.
    /// </summary>
    public IField? HandlerField { get; private set; }

    /// <summary>
    /// Should only be called by <see cref="BuildAspectContext.GetOrCreateHandlerField(DependencyGraphNode)"/>.
    /// </summary>
    /// <param name="field"></param>
    public void SetHandlerField( IField field ) => this.HandlerField = field;

    private IReadOnlyCollection<IMethod>? _childUpdateMethods;

    /// <summary>
    /// Gets the non-null <see cref="UpdateMethod"/> declarations of the children of the current node.
    /// </summary>
    public IReadOnlyCollection<IMethod> ChildUpdateMethods
    {
        get
        {
            // NB: This will intentionally throw if any encountered node has not yet had SetUpdateMethod called.
            // This ensures that the outcome is consistent and the result can be cached.

            this._childUpdateMethods ??= this.Children
                .Select( n => n.UpdateMethod.Declaration )
                .Where( m => m != null )
                .ToList()!;

            return this._childUpdateMethods;
        }
    }

    /// <inheritdoc cref="IReadOnlyDependencyGraphNode"/>
    public UncertainDeferredDeclaration<IMethod> UpdateMethod { get; } = new UncertainDeferredDeclaration<IMethod>( mustBeSetBeforeGet: true );

    IReadOnlyUncertainDeferredDeclaration<IMethod> IReadOnlyDependencyGraphNode.UpdateMethod => this.UpdateMethod;

    private CertainDeferredDeclaration<IMethod>? _subscribeMethod;

    /// <summary>
    /// Gets a method like <c>void SubscribeToA1()</c> for applicable root property (depth 1) nodes.
    /// </summary>
    public CertainDeferredDeclaration<IMethod> SubscribeMethod
        => this._subscribeMethod ?? throw new InvalidOperationException(
            nameof(this.SubscribeMethod) + " is not applicable to this node, access indicates incorrect caller logic." );
    

    protected override void ToStringAppendToLine( StringBuilder appendTo, string? format )
    {
#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity
#endif
        if ( format != null && format.Contains( "[ibh]" ) )
        {
            appendTo.Append( " ibh:" ).Append( this.InpcBaseHandling.ToString() );
        }
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif
    }
}