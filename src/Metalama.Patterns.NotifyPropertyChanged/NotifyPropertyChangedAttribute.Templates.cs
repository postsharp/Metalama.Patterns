using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged;

public partial class NotifyPropertyChangedAttribute
{
    [CompileTime]
    private static void CompileTimeThrow( Exception e )
        => throw e;

    [Template]
    private static dynamic? OverrideInpcRefTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;
            var handlerField = (IField?) meta.Tags["handlerField"];
            var node = (DependencyGraph.Node<NodeData>?) meta.Tags["node"];
            var inpcImplementationKind = node == null
                    ? ctx.GetInpcInstrumentationKind( meta.Target.Property.Type )
                    : node.Data.PropertyTypeInpcInstrumentationKind;
            var eventRequiresCast = inpcImplementationKind == InpcInstrumentationKind.Explicit;

            meta.InsertComment( "Template: " + nameof( OverrideInpcRefTypeProperty ) );
            meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );

            if ( !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ) )
            {
                if ( handlerField != null )
                {
                    var oldValue = meta.Target.FieldOrProperty.Value;
                    if ( oldValue != null )
                    {
                        if ( eventRequiresCast )
                        {
                            meta.Cast( ctx.Type_INotifyPropertyChanged, oldValue ).PropertyChanged -= handlerField.Value;
                        }
                        else
                        {
                            oldValue.PropertyChanged -= handlerField.Value;
                        }
                    }

                    meta.Target.FieldOrProperty.Value = value;
                }
                else if ( ctx.OnUnmonitoredInpcPropertyChangedMethod.Declaration != null )
                {
                    var oldValue = meta.Target.FieldOrProperty.Value;
                    meta.Target.FieldOrProperty.Value = value;
                    ctx.OnUnmonitoredInpcPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name, oldValue, value );
                }
                else
                {
                    meta.Target.FieldOrProperty.Value = value;
                }

                ctx.OnPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name );

                if ( handlerField != null )
                {
                    if ( value != null )
                    {
                        // This must be implemented as a local function because Metalama does not currently support delegates in any other way.
                        void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
                        {
                            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name, e.PropertyName );
                        }

                        handlerField.Value ??= (PropertyChangedEventHandler) OnSpecificPropertyChanged;

                        if ( eventRequiresCast )
                        {
                            meta.Cast( ctx.Type_INotifyPropertyChanged, value ).PropertyChanged += handlerField.Value;
                        }
                        else
                        {
                            value.PropertyChanged += handlerField.Value;
                        }
                    }
                }
            }
        }
    }

    [Template]
    private static dynamic? OverrideUninstrumentedTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;
            var node = (DependencyGraph.Node<NodeData>?) meta.Tags["node"];
            var compareUsing = (EqualityComparisonKind) meta.Tags["compareUsing"]!;
            var propertyTypeInstrumentationKind = (InpcInstrumentationKind) meta.Tags["propertyTypeInstrumentationKind"]!;

            meta.InsertComment( "Template: " + nameof( OverrideUninstrumentedTypeProperty ) );
            meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
            
            if ( propertyTypeInstrumentationKind == InpcInstrumentationKind.Unknown )
            {
                meta.InsertComment(
                    "Warning: the type of this property could not be analysed at design time, so it has been treated",
                    "as not implementing INotifyPropertyChanged. Code generated at compile time may differ." );
            }

            var compareExpr = compareUsing switch
            {
                EqualityComparisonKind.EqualityOperator => (IExpression) (meta.Target.FieldOrProperty.Value != value),
                EqualityComparisonKind.ThisEquals => (IExpression) !meta.Target.FieldOrProperty.Value.Equals( value ),
                EqualityComparisonKind.ReferenceEquals => (IExpression) !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ),
                EqualityComparisonKind.DefaultEqualityComparer => (IExpression) !ctx.GetDefaultEqualityComparerForType( meta.Target.FieldOrProperty.Type ).Value.Equals( value, meta.Target.FieldOrProperty.Value ),
                _ => null
            };

            if ( compareExpr == null )
            {
                meta.Target.FieldOrProperty.Value = value;
                ctx.OnPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name );
            }
            else
            {
                if ( compareExpr.Value )
                {
                    meta.Target.FieldOrProperty.Value = value;
                    ctx.OnPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name );
                }
            }
        }
    }

    // TODO: Make this private pending #33686
    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;

    [Template]
    private void OnPropertyChanged( string propertyName, [CompileTime] BuildAspectContext ctx )
    {
        meta.InsertComment( "Template: " + nameof( this.OnPropertyChanged ) );

        foreach ( var node in ctx.DependencyGraph.Children )
        {
            if ( node.DirectReferences.Count > 0 )
            {
                if ( propertyName == node.Name )
                {
                    // TODO: Consider replacing with dictionary lookup
                    foreach ( var refNode in node.DirectReferences )
                    {
                        meta.Target.Method.Invoke( refNode.Name );
                    }
                }
            }
        }
        if ( ctx.BaseOnPropertyChangedMethod == null )
        {
            this.PropertyChanged?.Invoke( meta.This, new PropertyChangedEventArgs( propertyName ) );
        }
        else
        {
            meta.Proceed();
        }
    }
    
    [Template]
    private static void UpdateChildInpcProperty(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData> node,
        [CompileTime] IExpression accessChildExpression, 
        [CompileTime] IField lastValueField,
        [CompileTime] IField onPropertyChangedHandlerField )
    {
        if ( node.Parent!.IsRoot)
        {
            CompileTimeThrow( new InvalidOperationException( $"{nameof( UpdateChildInpcProperty )} template must not be called on a root property node." ) );
        }

        meta.InsertComment( "Template: " + nameof( UpdateChildInpcProperty ) );
        meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );

        var newValue = accessChildExpression.Value;

        if ( !ReferenceEquals( newValue, lastValueField.Value ) )
        {
            if ( !ReferenceEquals( lastValueField.Value, null ) )
            {
                lastValueField.Value.PropertyChanged -= onPropertyChangedHandlerField.Value;
            }

            if ( newValue != null )
            {
                onPropertyChangedHandlerField.Value ??= (PropertyChangedEventHandler) OnSpecificPropertyChanged;
                newValue.PropertyChanged += onPropertyChangedHandlerField.Value;
            }

            lastValueField.Value = newValue;

            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Parent!.Data.DottedPropertyPath, node.Name );
        }
        
        void OnSpecificPropertyChanged( object? sender, PropertyChangedEventArgs e )
        {
            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Data.DottedPropertyPath, e.PropertyName );
        }
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
    private static void GenerateOnChildChangedOverride(
        string propertyName,
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData> node )
    {
        meta.InsertComment( "Template: " + nameof( GenerateOnChildChangedOverride ) );

        GenerateOnChildChangedBody( ctx, node, ExpressionFactory.Capture( propertyName ), true );
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

    [Template]
    private static void OnChildPropertyChanged( string parentPropertyPath, string propertyName, [CompileTime] BuildAspectContext ctx )
    {
        // TODO: Implement!
        meta.InsertComment( "TODO" );
    }

    [Template]
    private static void OnUnmonitoredInpcPropertyChanged( string propertyName, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue, [CompileTime] BuildAspectContext ctx )
    {
        meta.InsertComment( "Template: " + nameof( GenerateOnChildChangedBody ) );
        meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString() );
    }
}