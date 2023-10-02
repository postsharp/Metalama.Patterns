// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.Graph;
using System.Text;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

/// <summary>
/// Dependency graph node specialized for the processing using the classic implementation strategy.
/// </summary>
[CompileTime]
internal sealed class ClassicProcessingNode : ProcessingNode<ClassicProcessingNode, IReadOnlyClassicProcessingNode>, IReadOnlyClassicProcessingNode
{
    protected override void AfterInitializableNodeInitialize( ProcessingNodeInitializationContext initializationContext )
    {
        // NB: Nodes are initialized in GraphExtensions.DescendantsDepthFirst{T}(T) order.

        if ( initializationContext is not ClassicProcessingNodeInitializationContext ctx )
        {
            throw new ArgumentException( "Must be a " + nameof(ClassicProcessingNodeInitializationContext), nameof(initializationContext) );
        }

        if ( this.Depth == 1 )
        {
            this._subscribeMethod = new Deferred<IMethod>();
        }

        if ( !this.IsRoot )
        {
            this.PropertyTypeInpcInstrumentationKind = ctx.Helper.GetInpcInstrumentationKind( this.FieldOrProperty.Type );
            this.InpcBaseHandling = ctx.Helper.DetermineInpcBaseHandlingForNode( this );
        }
    }

    /// <summary>
    /// Gets the <see cref="InpcInstrumentationKind"/> for the type of the field or property.
    /// </summary>
    public InpcInstrumentationKind PropertyTypeInpcInstrumentationKind { get; private set; }

    public InpcBaseHandling InpcBaseHandling { get; private set; }

    /// <summary>
    /// Gets the potentially uninitialized field like "B? _lastA2". From non-template code, use
    /// <see cref="ClassicImplementationStrategyBuilder.GetOrCreateLastValueField(ClassicProcessingNode)"/> instead.
    /// </summary>
    /// <remarks>
    /// The value will only be set for applicable nodes. Template code should only be accessing the value of
    /// <see cref="LastValueField"/> when logic determines that it should have been set, and <see cref="Deferred{T}"/>
    /// will helpfully throw if there is an error in the program design.
    /// </remarks>
    public Deferred<IField> LastValueField { get; } = new();

    /// <summary>
    /// Gets the potentially uninitialized field like "PropertyChangedEventHandler? _onA2PropertyChangedHandler".
    /// From non-template code, use <see cref="ClassicImplementationStrategyBuilder.GetOrCreateHandlerField(ClassicProcessingNode)"/> instead.
    /// </summary>
    /// <remarks>
    /// The value will only be set for applicable nodes. Template code should only be accessing the value of
    /// <see cref="HandlerField"/> when logic determines that it should have been set, and <see cref="Deferred{T}"/>
    /// will helpfully throw if there is an error in the program design.
    /// </remarks>
    public Deferred<IField> HandlerField { get; } = new();

    private IReadOnlyCollection<IMethod>? _childUpdateMethods;

    /// <summary>
    /// Gets the non-null <see cref="UpdateMethod"/> declarations of the children of the current node.
    /// </summary>
    public IReadOnlyCollection<IMethod> ChildUpdateMethods
    {
        get
        {
            // NB: This will intentionally throw if any encountered node has not yet had the UpdateMethod value setter
            // invoked. This ensures that the outcome is consistent and the result can be cached.

            this._childUpdateMethods ??= this.Children
                .Select( n => n.UpdateMethod.Value )
                .Where( m => m != null )
                .ToList()!;

            return this._childUpdateMethods;
        }
    }

    /// <summary>
    /// Gets a method like <c>void UpdateA2C2()</c>. 
    /// </summary>
    /// <remarks>
    /// In a given inheritance hierarchy, for a given property path, this method is defined in
    /// the type closest to the root type where a reference to a property of A2.C2 first occurs.
    /// The method is always private as it is only called by other members of the type where it
    /// is defined. The value of <see cref="UpdateMethod"/> is always explicitly set for all 
    /// nodes (regardless of whether the value is <see langword="null"/> or not), and accessing the value
    /// will throw if it has not been set. This provides defense against incorrect program design
    /// that could otherwise lead to subtle incorrect behaviour.
    /// </remarks>
    public DeferredOptional<IMethod> UpdateMethod { get; } = new DeferredOptional<IMethod>( mustBeSetBeforeGet: true );

    private Deferred<IMethod>? _subscribeMethod;

    /// <summary>
    /// Gets a method like <c>void SubscribeToA1()</c> for applicable root property (depth 1) nodes.
    /// </summary>
    public Deferred<IMethod> SubscribeMethod
        => this._subscribeMethod ?? throw new InvalidOperationException(
            nameof(this.SubscribeMethod) + " is not applicable to this node, access indicates incorrect program design." );

    IReadOnlyDeferredOptional<IMethod> IReadOnlyClassicProcessingNode.UpdateMethod => this.UpdateMethod;

    IReadOnlyDeferred<IField> IReadOnlyClassicProcessingNode.HandlerField => this.HandlerField;

    IReadOnlyDeferred<IField> IReadOnlyClassicProcessingNode.LastValueField => this.LastValueField;

    IReadOnlyClassicProcessingNode IHasParent<IReadOnlyClassicProcessingNode>.Parent => this.Parent;

    IReadOnlyCollection<IReadOnlyClassicProcessingNode> IHasChildren<IReadOnlyClassicProcessingNode>.Children => this.Children;

    IReadOnlyCollection<IReadOnlyClassicProcessingNode> IHasReferencedBy<IReadOnlyClassicProcessingNode>.ReferencedBy => this.ReferencedBy;

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