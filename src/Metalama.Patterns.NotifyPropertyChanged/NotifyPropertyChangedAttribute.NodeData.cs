using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

namespace Metalama.Patterns.NotifyPropertyChanged;

public sealed partial class NotifyPropertyChangedAttribute
{
    // TODO: Consider merging NodeData.
    // NodeData is currently separated from DependencyGraph.Node to support different future [NPC]
    // implementation strategies. Consider merging common features back into Node, and/or
    // merge the whole of NodeData back into Node.

    /// <summary>
    /// Per-node data required by the current implementation strategy of <see cref="NotifyPropertyChangedAttribute"/>.
    /// </summary>
    /// <remarks>
    /// Avoid putting complex logic here. Aim for data only, and caching of simple LINQ expression results.
    /// </remarks>
    [CompileTime]
    private struct NodeData
    {
        /// <summary>
        /// Method called for each node once the graph has been built. Called in <see cref="DependencyGraph.Node{T}.DecendantsDepthFirst"/> order.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="node"></param>
        public void Initialize( BuildAspectContext ctx, DependencyGraph.Node<NodeData> node )
        {
            this._node = node;

            // TODO: Better checks/exceptions.
            this.FieldOrProperty = (IFieldOrProperty) ctx.Target.Compilation.GetDeclaration( node.Symbol );
            this.PropertyTypeInpcInstrumentationKind = ctx.GetInpcInstrumentationKind( this.FieldOrProperty.Type );

            // Parent will have been initialized due to defined init order.
            this.DottedPropertyPath = node.Parent!.IsRoot ? node.Name : $"{node.Parent.Data.DottedPropertyPath}.{node.Name}";
            this.ContiguousPropertyPath = node.Parent!.IsRoot ? node.Name : node.Parent.Data.ContiguousPropertyPath + node.Name;
        }

        public void Initialize2( InpcBaseHandling inpcBaseHandling )
        {
            this.InpcBaseHandling = inpcBaseHandling;
        }

        private DependencyGraph.Node<NodeData> _node;

        /// <summary>
        /// Gets a property path like "A1" or "A1.B1".
        /// </summary>
        public string DottedPropertyPath { get; private set; }

        /// <summary>
        /// Gets a property path like "A1" or "A1B1".
        /// </summary>
        public string ContiguousPropertyPath { get; private set; }

        /// <summary>
        /// Gets the <see cref="IFieldOrProperty"/> for the node.
        /// </summary>
        public IFieldOrProperty FieldOrProperty { get; private set; }

        /// <summary>
        /// Gets the <see cref="InpcInstrumentationKind"/> for the <see cref="IHasType.Type"/> of <see cref="FieldOrProperty"/>.
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
        public void SetLastValueField( IField field )
            => this.LastValueField = field;

        /// <summary>
        /// Gets the potentially uninitialized field like "PropertyChangedEventHandler? _onA2PropertyChangedHandler".
        /// Typically, use <see cref="BuildAspectContext.GetOrCreateHandlerField(DependencyGraph.Node{NodeData})"/> instead.
        /// </summary>
        public IField? HandlerField { get; private set; }

        /// <summary>
        /// Should only be called by <see cref="BuildAspectContext.GetOrCreateHandlerField(DependencyGraph.Node{NodeData})"/>.
        /// </summary>
        /// <param name="field"></param>
        public void SetHandlerField( IField field )
            => this.HandlerField = field;

        private IReadOnlyCollection<DependencyGraph.Node<NodeData>>? _immediateReferences;

        /// <summary>
        /// Gets the distinct set of "immediate family" references - the references to the current node and the children of the current node.
        /// </summary>
        public IReadOnlyCollection<DependencyGraph.Node<NodeData>> ImmediateReferences
        {
            get
            {
                this._immediateReferences ??= this._node.Children
                    .SelectMany( c => c.GetAllReferences() )
                    .Concat( this._node.GetAllReferences() )
                    .Distinct()
                    .ToList();

                return this._immediateReferences;
            }
        }

        private IReadOnlyCollection<IMethod>? _cascadeUpdateMethods;

        /// <summary>
        /// Gets the <see cref="UpdateMethod"/> of the children of the current node.
        /// </summary>
        public IReadOnlyCollection<IMethod> CascadeUpdateMethods
        {
            get
            {
                // NB: This will intentionally throw if any encountered node has not yet had UpdateMethod set.
                // This ensures that the outcome is consistent and the result can be cached.

                this._cascadeUpdateMethods ??= this._node.Children
                    .Select( n => n.Data.UpdateMethod )
                    .Where( m => m != null )
                    .ToList()!;

                return this._cascadeUpdateMethods;
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
                    throw new InvalidOperationException( $"{nameof( this.UpdateMethod )} for node {this._node.Name} has not been set yet, so cannot be read." );
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
    }
}