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
        public void Initialize( BuildAspectContext ctx, DependencyGraph.Node<NodeData> node )
        {
            // TODO: Better checks/exceptions.
            this.FieldOrProperty = (IFieldOrProperty) ctx.Target.Compilation.GetDeclaration( node.Symbol );
        }

        public IFieldOrProperty FieldOrProperty { get; private set; }

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
        /// and <see cref="OnChildChangedMethod"/>. Both or neither of these two must be present. If both are present,
        /// the derived type will override these methods if required, tail-calling the base method. If neither are present,
        /// then <see cref="UpdateMethod"/> must be generated (see the first paragraph above).
        /// </remarks>
        public IMethod? UpdateMethod { get; private set; }

        /// <summary>
        /// Gets a method like <c>void OnA2C2Changed()</c>.
        /// </summary>
        public IMethod? OnChangedMethod { get; private set; }

        /// <summary>
        /// Gets a method like <c>void OnA2C2ChildChanged( string propertyName )</c>.
        /// </summary>
        public IMethod? OnChildChangedMethod { get; private set; }

        public bool MethodsHaveBeenSet { get; private set; }

        public void SetMethods( IMethod? updateMethod, IMethod? onChangedMethod, IMethod? onChildChangedMethod ) 
        { 
            if ( this.MethodsHaveBeenSet )
            {
                throw new InvalidOperationException( "Methods have already been set." );
            }

            if ( onChangedMethod == null != (onChildChangedMethod == null) )
            {
                throw new ArgumentException( "Both or neither of " + nameof( onChangedMethod ) + " and " + nameof( onChildChangedMethod ) + " must be set." );
            }

            if ( updateMethod == null && onChangedMethod == null )
            {
                throw new ArgumentException( "At least one method must be set." );
            }

            this.UpdateMethod = updateMethod;
            this.OnChangedMethod = onChangedMethod;
            this.OnChildChangedMethod = onChildChangedMethod;
            this.MethodsHaveBeenSet = true;
        }
    }
}