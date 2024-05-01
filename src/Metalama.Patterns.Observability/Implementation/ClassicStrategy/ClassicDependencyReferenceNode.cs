// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal class ClassicDependencyReferenceNode : DependencyReferenceNode
{
    public new IEnumerable<ClassicDependencyReferenceNode> Children => base.Children.Cast<ClassicDependencyReferenceNode>();

    /// <summary>
    /// Gets the potentially uninitialized field like "B? _lastA2". From non-template code, use
    /// <see cref="ClassicObservabilityStrategyImpl.GetOrCreateLastValueField(ClassicProcessingNode)"/> instead.
    /// </summary>
    /// <remarks>
    /// The value will only be set for applicable nodes. Template code should only be accessing the value of
    /// <see cref="LastValueField"/> when logic determines that it should have been set, and <see cref="Deferred{T}"/>
    /// will helpfully throw if there is an error in the program design.
    /// </remarks>
    public Deferred<IField> LastValueField { get; } = new();

    /// <summary>
    /// Gets the potentially uninitialized field like "PropertyChangedEventHandler? _onA2PropertyChangedHandler".
    /// From non-template code, use <see cref="ClassicObservabilityStrategyImpl.GetOrCreateHandlerField(ClassicProcessingNode)"/> instead.
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
                .Cast<ClassicDependencyReferenceNode>()
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
    public Deferred<IMethod?> UpdateMethod { get; } = new();

    public Deferred<IMethod> SubscribeMethod { get; } = new();

    /// <summary>
    /// Gets the <see cref="InpcInstrumentationKind"/> for the type of the field or property.
    /// </summary>
    public InpcInstrumentationKind PropertyTypeInpcInstrumentationKind { get; private set; }

    public InpcBaseHandling InpcBaseHandling { get; private set; }

    public string Name => this.Depth == 1 ? this.FieldOrProperty.Name : throw new InvalidOperationException();

    public ClassicDependencyReferenceNode(
        DependencyPropertyNode referencedPropertyNode,
        DependencyReferenceNode? parent,
        ClassicDependencyGraphBuilder builder,
        ClassicProcessingNodeInitializationContext ctx ) : base(
        referencedPropertyNode,
        parent,
        builder )
    {
        this.PropertyTypeInpcInstrumentationKind = ctx.Helper.GetInpcInstrumentationKind( referencedPropertyNode.FieldOrProperty.Type );
        this.InpcBaseHandling = ctx.Helper.DetermineInpcBaseHandlingForNode( this );
    }

    public new IEnumerable<ClassicDependencyPropertyNode> ReferencingProperties => base.ReferencingProperties.Cast<ClassicDependencyPropertyNode>();

    public IReadOnlyCollection<ClassicDependencyPropertyNode> GetAllReferencingProperties(
        Func<ClassicDependencyReferenceNode, bool>? shouldIncludeImmediateChild = null )
        => (IReadOnlyCollection<ClassicDependencyPropertyNode>)
            base.GetAllReferencingProperties( 
                shouldIncludeImmediateChild == null ? null :
                node => shouldIncludeImmediateChild.Invoke( (ClassicDependencyReferenceNode) node ) );
}