using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged;

public partial class NotifyPropertyChangedAttribute
{
    [Template]
    private static void GenerateOnChildChangedOverride(
        string propertyName,
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData> node )
    {
        meta.InsertComment( "Template: " + nameof( GenerateOnChildChangedOverride ) );

        GenerateOnChildChangedBody( ctx, node, ExpressionFactory.Capture( propertyName ), true );
    }

    [Template]
    private static void GenerateOnChangedBody(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData>? node,
        [CompileTime] string? propertyName,
        [CompileTime] bool disableNotifySelfChanged = false,
        [CompileTime] bool proceedAtEnd = false )
    {
        meta.InsertComment( $"Template: {nameof( GenerateOnChangedBody )} for {propertyName}" );
        meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );

        if ( node != null )
        {
            var cascadeUpdateMethods = node.Children.Select( n => n.Data.UpdateMethod ).Where( m => m != null );

            foreach ( var method in cascadeUpdateMethods )
            {
                method.Invoke();
            }

            var affectedPropertyNames =
                node.Children
                .SelectMany( c => c.GetAllReferences() )
                .Concat( node.GetAllReferences() )
                .Distinct()
                .Select( n => n.Name )
                .OrderBy( s => s );

            foreach ( var name in affectedPropertyNames )
            {
                ctx.OnPropertyChangedMethod.Declaration.Invoke( name );
            }
        }

        // node is null for unreferenced (according to static compile time analsysis) root properties.
        if ( !disableNotifySelfChanged && (node == null || node.Parent!.IsRoot) )
        {
            propertyName ??= node?.Name;

            if ( propertyName == null )
            {
                meta.InsertComment( "Error: must specify node and/or propertyName." );
                CompileTimeThrow( new InvalidOperationException( "Template: must specify node and/or propertyName." ) );
            }

            ctx.OnPropertyChangedMethod.Declaration.Invoke( propertyName );
        }

        if ( proceedAtEnd )
        {
            meta.Proceed();
        }
    }

    [Template]
    private static void GenerateOnChildChangedBody(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData>? node,
        [CompileTime] IExpression propertyNameExpression,
        [CompileTime] bool proceedBeforeReturn = false )
    {
        meta.InsertComment( "Template: " + nameof( GenerateOnChildChangedBody ) );
        meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );

        if ( node != null )
        {
            // TODO: How to build a switch statement nicely in a template?
            // For now, use if. Also, might use a runtime static readonly dictionary at least for the OnPropertyChanged calls.
            foreach ( var child in node.Children )
            {
                var hasRefs = child.DirectReferences.Count > 0;
                var hasUpdateMethod = child.Data.UpdateMethod != null;

                if ( hasRefs || hasUpdateMethod )
                {
                    if ( propertyNameExpression.Value == child.Name )
                    {
                        if ( hasUpdateMethod )
                        {
                            child.Data.UpdateMethod.Invoke();
                        }

                        if ( hasRefs )
                        {
                            var affectedPropertyNames = child.GetAllReferences().Select( n => n.Name ).OrderBy( s => s );

                            foreach ( var name in affectedPropertyNames )
                            {
                                ctx.OnPropertyChangedMethod.Declaration.Invoke( name );
                            }
                        }

                        // TODO: How to build an if..else if.. statement nicely in a template?
                        // For now, use return.
                        if ( proceedBeforeReturn )
                        {
                            meta.Proceed();
                        }

                        return;
                    }
                }
            }
        }

        if ( proceedBeforeReturn )
        {
            meta.Proceed();
        }
    }

}