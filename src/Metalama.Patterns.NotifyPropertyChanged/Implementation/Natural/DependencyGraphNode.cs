// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Text;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

/// <summary>
/// Dependency graph node specialized for the current implementation strategy of <see cref="NotifyPropertyChangedAttribute"/>.
/// </summary>
[CompileTime]
internal class DependencyGraphNode : DependencyGraph.Node<DependencyGraphNode>
{
    /// <summary>
    /// Method called for each node once the graph has been built. Called in <see cref="DependencyGraph.Node{TDerived}.DecendantsDepthFirst"/> order.
    /// </summary>
    /// <param name="ctx"></param>
    public void Initialize( BuildAspectContext ctx )
    {
        this.PropertyTypeInpcInstrumentationKind = ctx.GetInpcInstrumentationKind( this.FieldOrProperty.Type );
        this.InpcBaseHandling = DetermineInpcBaseHandling();

        InpcBaseHandling DetermineInpcBaseHandling()
        {
            switch ( this.PropertyTypeInpcInstrumentationKind )
            {
                case InpcInstrumentationKind.Unknown:
                    return InpcBaseHandling.Unknown;

                case InpcInstrumentationKind.None:
                    return InpcBaseHandling.NA;

                case InpcInstrumentationKind.Implicit:
                case InpcInstrumentationKind.Explicit:
                    if ( this.Depth == 1 )
                    {
                        // Root property
                        return this.FieldOrProperty.DeclaringType == ctx.Target
                            ? InpcBaseHandling.NA
                            : ctx.HasInheritedOnChildPropertyChangedPropertyPath( this.Name )
                                ? InpcBaseHandling.OnChildPropertyChanged
                                : ctx.HasInheritedOnUnmonitoredInpcPropertyChangedProperty( this.Name )
                                    ? InpcBaseHandling.OnUnmonitoredInpcPropertyChanged
                                    : InpcBaseHandling.OnPropertyChanged;
                    }
                    else
                    {
                        // Child property
                        return ctx.HasInheritedOnChildPropertyChangedPropertyPath( this.DottedPropertyPath )
                            ? InpcBaseHandling.OnChildPropertyChanged
                            : ctx.HasInheritedOnUnmonitoredInpcPropertyChangedProperty( this.DottedPropertyPath )
                                ? InpcBaseHandling.OnUnmonitoredInpcPropertyChanged
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
    /// <see cref="BuildAspectContext.GetOrCreateLastValueField(DependencyGraph.Node{NodeData})"/> instead.
    /// </summary>
    public IField? LastValueField { get; private set; }

    /// <summary>
    /// Should only be called by <see cref="BuildAspectContext.GetOrCreateLastValueField(DependencyGraph.Node{NodeData})"/>.
    /// </summary>
    /// <param name="field"></param>
    public void SetLastValueField( IField field ) => this.LastValueField = field;

    /// <summary>
    /// Gets the potentially uninitialized field like "PropertyChangedEventHandler? _onA2PropertyChangedHandler".
    /// Typically, use <see cref="BuildAspectContext.GetOrCreateHandlerField(DependencyGraph.Node{NodeData})"/> instead.
    /// </summary>
    public IField? HandlerField { get; private set; }

    /// <summary>
    /// Should only be called by <see cref="BuildAspectContext.GetOrCreateHandlerField(DependencyGraph.Node{NodeData})"/>.
    /// </summary>
    /// <param name="field"></param>
    public void SetHandlerField( IField field ) => this.HandlerField = field;

    private IReadOnlyCollection<IMethod>? _childUpdateMethods;

    /// <summary>
    /// Gets the <see cref="UpdateMethod"/> of the children of the current node.
    /// </summary>
    public IReadOnlyCollection<IMethod> ChildUpdateMethods
    {
        get
        {
            // NB: This will intentionally throw if any encountered node has not yet had SetUpdateMethod called.
            // This ensures that the outcome is consistent and the result can be cached.

            this._childUpdateMethods ??= this.Children
                .Select( n => n.UpdateMethod )
                .Where( m => m != null )
                .ToList()!;

            return this._childUpdateMethods;
        }
    }

    private IMethod? _updateMethod;

    /// <summary>
    /// Gets a method like <c>void UpdateA2C2()</c>. 
    /// </summary>
    /// <remarks>
    /// In a given inheritance hierarchy, for a given property path, this method is defined in
    /// the type closest to the root type where a reference to a property of A2.C2 first occurs.
    /// The method is always private as it is only called by other members of the type where it
    /// is defined.
    /// </remarks>
    public IMethod? UpdateMethod
    {
        get
        {
            if ( !this.UpdateMethodHasBeenSet )
            {
                throw new InvalidOperationException( $"{nameof(this.UpdateMethod)} for node {this.Name} has not been set yet, so cannot be read." );
            }

            return this._updateMethod;
        }
    }

    public bool UpdateMethodHasBeenSet { get; private set; }

    public void SetUpdateMethod( IMethod? updateMethod )
    {
        if ( this.UpdateMethodHasBeenSet )
        {
            throw new InvalidOperationException( "Methods have already been set." );
        }

        this._updateMethod = updateMethod;
        this.UpdateMethodHasBeenSet = true;
    }

    protected override void ToStringAppendToLine( StringBuilder appendTo, string? format )
    {
#pragma warning disable CA1307 // Specify StringComparison for clarity
        if ( format != null && format.Contains( "[ibh]" ) )
        {
            appendTo.Append( " ibh:" ).Append( this.InpcBaseHandling.ToString() );
        }
#pragma warning restore CA1307 // Specify StringComparison for clarity
    }
}