using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

namespace Metalama.Patterns.NotifyPropertyChanged;

public sealed partial class NotifyPropertyChangedAttribute
{
    private struct NodeData
    {
        public void Initialize( BuildAspectContext ctx, DependencyHelper.TreeNode<NodeData> node )
        {
            // TODO: Better checks/exceptions.
            this.FieldOrProperty = (IFieldOrProperty) ctx.Target.Compilation.GetDeclaration( node.Symbol );
        }

        public IFieldOrProperty FieldOrProperty { get; private set; }

        public IMethod? UpdateMethod;
    }
}