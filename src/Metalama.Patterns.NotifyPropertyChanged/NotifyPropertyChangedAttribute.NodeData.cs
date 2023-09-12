using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

namespace Metalama.Patterns.NotifyPropertyChanged;

public sealed partial class NotifyPropertyChangedAttribute
{
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
            // TODO: Better checks/exceptions.
            this.FieldOrProperty = (IFieldOrProperty) ctx.Target.Compilation.GetDeclaration( node.Symbol );
            this.PropertyTypeInpcInstrumentationKind = ctx.GetInpcInstrumentationKind( this.FieldOrProperty.Type );

            // Parent will have been initialized due to defined init order.
            this.DottedPropertyPath = node.Parent!.IsRoot ? node.Name : $"{node.Parent.Data.DottedPropertyPath}.{node.Name}";
            this.ContiguousPropertyPath = node.Parent!.IsRoot ? node.Name : node.Parent.Data.ContiguousPropertyPath + node.Name;
        }

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

        /// <summary>
        /// Gets a method like <c>void UpdateA2C2()</c>. 
        /// </summary>
        /// <remarks>
        /// In a given inheritance hierarchy, for a given property path, this method is defined in
        /// the type closest to the root type where a reference to a property of A2.C2 first occurs.
        /// The method is always private as it is only called by other members of the type where it
        /// is defined.
        /// </remarks>
        public IMethod? UpdateMethod { get; private set; }

        public bool MethodsHaveBeenSet { get; private set; }

        public void SetMethods( IMethod? updateMethod )
        {
            if ( this.MethodsHaveBeenSet )
            {
                throw new InvalidOperationException( "Methods have already been set." );
            }

            this.UpdateMethod = updateMethod;
            this.MethodsHaveBeenSet = true;
        }
    }
}