using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged;

public sealed partial class NotifyPropertyChangedAttribute
{
    [CompileTime]
    private struct NodeData
    {
        public void Initialize( BuildAspectContext ctx, DependencyGraph.Node<NodeData> node )
        {
            // TODO: Better checks/exceptions.
            this.FieldOrProperty = (IFieldOrProperty) ctx.Target.Compilation.GetDeclaration( node.Symbol );
            this.PropertyTypeInpcInstrumentationKind = ctx.GetInpcInstrumentationKind( this.FieldOrProperty.Type );
        }

        /// <summary>
        /// Gets the <see cref="IFieldOrProperty"/> for the node.
        /// </summary>
        public IFieldOrProperty FieldOrProperty { get; private set; }

        // TODO: Is this actually worth caching per-node?
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
        /// is defined. If the target type is sealed, all required code is inlined within the body
        /// of <see cref="UpdateMethod"/>; otherwise, two additional `protected virtual` methods are generated -
        /// OnA2C2Changed and OnA2C2ChildChanged - and these are called from <see cref="UpdateMethod"/>.
        /// 
        /// When processing a derived type, we first search base types for the corresponding <see cref="OnChangedMethod"/>
        /// and <see cref="OnChildChangedMethod"/>. <see cref="OnChildChangedMethod"/> is only defined for property
        /// types that implement <see cref="INotifyPropertyChanged"/>. If present,
        /// the derived type will override these methods if required, tail-calling the base method. If neither are present,
        /// then <see cref="UpdateMethod"/> must be generated (see the first paragraph above).
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