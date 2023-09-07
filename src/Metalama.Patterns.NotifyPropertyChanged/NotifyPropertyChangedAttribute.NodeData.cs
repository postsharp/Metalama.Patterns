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

        public IMethod? UpdateMethod;
    }
}