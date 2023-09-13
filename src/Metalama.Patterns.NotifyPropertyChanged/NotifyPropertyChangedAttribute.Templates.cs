// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged;

public partial class NotifyPropertyChangedAttribute
{
    [CompileTime]
    private static void CompileTimeThrow( Exception e )
        => throw e;

    [CompileTime]
    private static T ExpectNotNull<T>( T? obj )
        => obj ?? throw new InvalidOperationException( "A null value was not expected here." );

    // TODO: Make this private pending #33686
    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;

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
                        handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;

                        if ( eventRequiresCast )
                        {
                            meta.Cast( ctx.Type_INotifyPropertyChanged, value ).PropertyChanged += handlerField.Value;
                        }
                        else
                        {
                            value.PropertyChanged += handlerField.Value;
                        }

                        void OnChildPropertyChanged( object sender, PropertyChangedEventArgs e )
                        {
                            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name, e.PropertyName );
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

    [Template]
    private void OnPropertyChanged( string propertyName, [CompileTime] BuildAspectContext ctx )
    {
        meta.InsertComment( "Template: " + nameof( this.OnPropertyChanged ) );
        meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString() );

        foreach ( var node in ctx.DependencyGraph.Children )
        {
            if ( node.Data.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged && ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined )
            {
                meta.InsertComment( $"Skipping '{node.Name}', InpcBaseHandling = {node.Data.InpcBaseHandling}, and OnUnmonitoredInpcPropertyChangedMethod will be defined." );
                continue;
            }

            var cascadeUpdateMethods = node.Data.CascadeUpdateMethods;
            var affectedNodes = node.Data.ImmediateReferences;

            if ( affectedNodes.Count > 0 
                || cascadeUpdateMethods.Count > 0 
                || ( node.HasChildren && node.Data.InpcBaseHandling is InpcBaseHandling.OnUnmonitoredInpcPropertyChanged or InpcBaseHandling.OnPropertyChanged ) )
            {
                var affectedPropertyNames = affectedNodes
                    .Select( n => n.Name )
                    .OrderBy( s => s );

                if ( propertyName == node.Name )
                {
                    meta.InsertComment( $"InpcBaseHandling = {node.Data.InpcBaseHandling}" );

                    switch ( node.Data.InpcBaseHandling )
                    {
                        case InpcBaseHandling.Unknown:
                            meta.InsertComment(
                                $"Warning: the type of property '{node.Name}' could not be analysed at design time, so it has been treated",
                                "as not implementing INotifyPropertyChanged. Code generated at compile time may differ." );
                            break;

                        case InpcBaseHandling.NA:
                        case InpcBaseHandling.OnChildPropertyChanged:
                            break;

                        // NB: The OnUnmonitoredInpcPropertyChanged case, when ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined is true, is handled above.
                        case InpcBaseHandling.OnUnmonitoredInpcPropertyChanged:
                        case InpcBaseHandling.OnPropertyChanged:
                            if ( node.HasChildren )
                            {
                                var handlerField = ExpectNotNull( node.Data.HandlerField );
                                var lastValueField = ExpectNotNull( node.Data.LastValueField );
                                var eventRequiresCast = node.Data.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Explicit;

                                var oldValue = lastValueField.Value;
                                var newValue = node.Data.FieldOrProperty.Value;

                                if ( !ReferenceEquals( oldValue, newValue ) )
                                {
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

                                    lastValueField.Value = newValue;

                                    if ( newValue != null )
                                    {
                                        handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;

                                        if ( eventRequiresCast )
                                        {
                                            meta.Cast( ctx.Type_INotifyPropertyChanged, newValue ).PropertyChanged += handlerField.Value;
                                        }
                                        else
                                        {
                                            newValue.PropertyChanged += handlerField.Value;
                                        }

                                        void OnChildPropertyChanged( object sender, PropertyChangedEventArgs e )
                                        {
                                            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Name, e.PropertyName );
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            CompileTimeThrow( new InvalidOperationException( $"InpcBaseHandling '{node.Data.InpcBaseHandling}' was not expected here." ) );
                            break;
                    }

                    foreach ( var method in cascadeUpdateMethods )
                    {
                        method.Invoke();
                    }

                    // TODO: Consider replacing with dictionary lookup
                    foreach ( var name in affectedPropertyNames )
                    {
                        // TODO: Question: what if the method of the current template is renamed during introduction? Is this the correct way to call the current method recursively?
                        // meta.Target.Method.Invoke generates OnPropertyChanged_Empty and calls it.
                        meta.This.OnPropertyChanged( name );
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
        if ( node.Parent!.IsRoot )
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
                onPropertyChangedHandlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;
                newValue.PropertyChanged += onPropertyChangedHandlerField.Value;

                void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                {
                    ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Data.DottedPropertyPath, e.PropertyName );
                }
            }

            lastValueField.Value = newValue;

            // TODO: Remove unused branch if we end up not allowing root props
            if ( node.Parent!.IsRoot )
            {
                ctx.OnPropertyChangedMethod.Declaration.Invoke( node.Name );
            }
            else
            {
                ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Parent!.Data.DottedPropertyPath, node.Name );
            }
        }
    }

    [Template]
    private static void OnChildPropertyChanged(
        string parentPropertyPath,
        string propertyName,
        [CompileTime] BuildAspectContext ctx )
    {
        meta.InsertComment( "Template: " + nameof( OnChildPropertyChanged ) );
        meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString() );

        foreach ( var node in ctx.DependencyGraph.DecendantsDepthFirst().Where( n => n.Depth > 1 ) )
        {
            var cascadeUpdateMethods = node.Data.CascadeUpdateMethods;
            var affectedNodes = node.Data.ImmediateReferences;

            if ( affectedNodes.Count > 0 || cascadeUpdateMethods.Count > 0 )
            {
                var affectedPropertyNames = affectedNodes
                    .Select( n => n.Name )
                    .OrderBy( s => s );

                if ( parentPropertyPath == node.Parent!.Data.DottedPropertyPath && propertyName == node.Name )
                {
                    foreach ( var method in cascadeUpdateMethods )
                    {
                        method.Invoke();
                    }

                    // TODO: Consider replacing with dictionary lookup
                    foreach ( var name in affectedPropertyNames )
                    {
                        // TODO: Question: what if the method of the current template is renamed during introduction? Is this the correct way to call the current method recursively?
                        // meta.Target.Method.Invoke generates OnPropertyChanged_Empty and calls it.
                        meta.This.OnPropertyChanged( name );
                    }
                }
            }
        }

        meta.Proceed();
    }

    [Template]
    private static void OnUnmonitoredInpcPropertyChanged(
        string propertyPath,
        INotifyPropertyChanged? oldValue,
        INotifyPropertyChanged? newValue,
        [CompileTime] BuildAspectContext ctx )
    {
        meta.InsertComment( "Template: " + nameof( OnUnmonitoredInpcPropertyChanged ) );
        meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString() );

        // NB: OnUnmonitoredInpcPropertyChanged is only called when a ref has changed - the caller checks this first.

        foreach ( var node in ctx.DependencyGraph.DecendantsDepthFirst().Where( n => n.Data.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged ) )
        {
            meta.InsertComment( $"Node '{node.Data.DottedPropertyPath}'" );
            
            if ( propertyPath == node.Data.DottedPropertyPath )
            {
                var handlerField = ExpectNotNull( node.Data.HandlerField );
                
                if ( oldValue != null )
                {
                    oldValue.PropertyChanged -= handlerField.Value;
                }

                if ( newValue != null )
                {
                    handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;
                    newValue.PropertyChanged += handlerField.Value;

                    void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                    {
                        ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Data.DottedPropertyPath, e.PropertyName );
                    }
                }

                foreach ( var method in node.Data.CascadeUpdateMethods )
                {
                    method.Invoke();
                }

                var affectedPropertyNames =
                    node.Data.ImmediateReferences
                    .Select( n => n.Name )
                    .OrderBy( s => s );

                // TODO: Consider replacing with dictionary lookup
                foreach ( var name in affectedPropertyNames )
                {
                    // TODO: Question: what if the method of the current template is renamed during introduction? Is this the correct way to call the current method recursively?
                    // meta.Target.Method.Invoke generates OnPropertyChanged_Empty and calls it.
                    meta.This.OnPropertyChanged( name );
                }
            }
        }
    }
}