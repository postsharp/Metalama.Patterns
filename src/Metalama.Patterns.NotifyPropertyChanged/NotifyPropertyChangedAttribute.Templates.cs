﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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

            if ( ctx.InsertDiagnosticComments )
            {
                meta.InsertComment( "Template: " + nameof( OverrideInpcRefTypeProperty ) );
                meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
            }

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

                // TODO: Is this similar to [Frag4] and/or [Frag6] and/or [Frag8]? Can we refactor?
                // XXX [Frag3]
                if ( node != null )
                {
                    // Update methods will deal with notifications - *for those children which have update methods*
                    foreach ( var method in node.Data.CascadeUpdateMethods )
                    {
                        method.Invoke();
                    }
                    
                    // Notify refs to the current node and any children without an update method.
                    foreach ( var name in node.GetAllReferences( includeImmediateChild: n => n.Data.UpdateMethod == null ).Select( n => n.Name ).OrderBy( n => n ) )
                    {
                        ctx.OnPropertyChangedMethod.Declaration.Invoke( name );
                    }
                }
                // XXX

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

                        // Local Function Alert (it's easy to miss when skimming through the code)
                        // -----------------------------------------------------------------------

                        void OnChildPropertyChanged( object sender, PropertyChangedEventArgs e )
                        {                            
                            var propertyName = e.PropertyName;

                            // TODO: If this can be similar to [Frag2] and/or [Frag5] and/or [Frag7], refactor
                            // XXX [Frag1]
                            // NB: If handlerField is not null, node must also be non-null.
                            foreach ( var childNode in node.Children )
                            {
                                var hasUpdateMethod = childNode.Data.UpdateMethod != null;
                                var hasRefs = childNode.DirectReferences.Count > 0;

                                if ( hasUpdateMethod || hasRefs )
                                {
                                    if ( propertyName == childNode.Name )
                                    {
                                        if ( hasUpdateMethod )
                                        {
                                            // Update method will deal with notifications
                                            childNode.Data.UpdateMethod.Invoke();
                                        }
                                        else
                                        {
                                            // No update method, notify here.
                                            foreach ( var refName in childNode.GetAllReferences().Select( n => n.Name ).OrderBy( n => n ) )
                                            {
                                                ctx.OnPropertyChangedMethod.Declaration.Invoke( refName );
                                            }
                                            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name, childNode.Name );
                                        }
                                        return;
                                    }
                                }
                            }
                            // XXX
                            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( meta.Target.FieldOrProperty.Name, propertyName );
                        }
                    }
                }
            }
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
        if ( node.Depth <= 1 )
        {
            CompileTimeThrow( new InvalidOperationException( $"{nameof( UpdateChildInpcProperty )} template must not be called on a root property node." ) );
        }

        if ( ctx.InsertDiagnosticComments )
        {
            meta.InsertComment( "Template: " + nameof( UpdateChildInpcProperty ) );
            meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
        }

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

                // Local Function Alert (it's easy to miss when skimming through the code)
                // -----------------------------------------------------------------------

                void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                {
                    var propertyName = e.PropertyName;

                    // TODO: If this can be similar to [Frag1] and/or [Frag5] and/or [Frag7], refactor
                    // XXX [Frag2]
                    foreach ( var childNode in node.Children )
                    {
                        var hasUpdateMethod = childNode.Data.UpdateMethod != null;
                        var hasRefs = childNode.DirectReferences.Count > 0;
                        
                        if ( hasUpdateMethod || hasRefs )
                        {
                            if ( propertyName == childNode.Name )
                            {
                                if ( hasUpdateMethod )
                                {
                                    // Update method will deal with notifications
                                    childNode.Data.UpdateMethod.Invoke();
                                }
                                else
                                {
                                    // No update method, notify here.
                                    foreach ( var refName in childNode.GetAllReferences().Select( n => n.Name ).OrderBy( n => n ) )
                                    {
                                        ctx.OnPropertyChangedMethod.Declaration.Invoke( refName );
                                    }
                                    ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Data.DottedPropertyPath, childNode.Name );
                                }
                                return;
                            }
                        }
                    }
                    // XXX                    
                    ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Data.DottedPropertyPath, e.PropertyName );
                }
            }

            lastValueField.Value = newValue;

            // TODO: Is this similar to [Frag3] and/or [Frag6] and/or [Frag8]? Can we refactor?
            // XXX [Frag4]
            // Update methods will deal with notifications - *for those children which have update methods*
            foreach ( var method in node.Data.CascadeUpdateMethods )
            {
                method.Invoke();
            }

            // Notify refs to the current node and any children without an update method.
            foreach ( var name in node.GetAllReferences( includeImmediateChild: n => n.Data.UpdateMethod == null ).Select( n => n.Name ).OrderBy( n => n ) )
            {
                ctx.OnPropertyChangedMethod.Declaration.Invoke( name );
            }
            // XXX

            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Parent!.Data.DottedPropertyPath, node.Name );
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

            if ( ctx.InsertDiagnosticComments )
            {
                meta.InsertComment( "Template: " + nameof( OverrideUninstrumentedTypeProperty ) );
                meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
            }

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
        if ( ctx.InsertDiagnosticComments )
        {
            meta.InsertComment( "Template: " + nameof( this.OnPropertyChanged ) );
            meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( ( ref NodeData data ) => $"ibh:{data.InpcBaseHandling}" ) );
        }

        foreach ( var node in ctx.DependencyGraph.Children )
        {
            if ( node.Data.FieldOrProperty.DeclaringType == ctx.Target )
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.Name}': The property is defined by the current type." );
                }
                continue;
            }

            if ( node.Data.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged && ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined )
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.Name}': A base type supports OnUnmonitoredInpcPropertyChanged for this property, and the current type is configured to use that feature." );
                }
                continue;
            }

            IReadOnlyCollection<DependencyGraph.Node<NodeData>> refsToNotify;

            if ( node.Data.InpcBaseHandling == InpcBaseHandling.OnChildPropertyChanged )
            {
                if ( node.DirectReferences.Count == 0 )
                {
                    if ( ctx.InsertDiagnosticComments )
                    {
                        meta.InsertComment( $"Skipping '{node.Name}': A base type supports OnChildPropertyChanged for this property, and the property itself has no references." );
                    }
                    continue;
                }
                else
                {
                    // Only notify references to the node itself, child node changes will be handled via OnChildPropertyChanged.
                    refsToNotify = node.GetAllReferences();
                }
            }
            else
            {
                // Notify refs to the current node and any children without an update method.
                refsToNotify = node.GetAllReferences( includeImmediateChild: n => n.Data.UpdateMethod == null );
            }

            var cascadeUpdateMethods = node.Data.CascadeUpdateMethods;

            if ( refsToNotify.Count > 0 
                || cascadeUpdateMethods.Count > 0 
                || ( node.HasChildren && node.Data.InpcBaseHandling is InpcBaseHandling.OnUnmonitoredInpcPropertyChanged or InpcBaseHandling.OnPropertyChanged ) )
            {
                var rootPropertyNamesToNotify = refsToNotify
                    .Select( n => n.Name )
                    .OrderBy( s => s );

                if ( propertyName == node.Name )
                {
                    if ( ctx.InsertDiagnosticComments )
                    {
                        meta.InsertComment( $"InpcBaseHandling = {node.Data.InpcBaseHandling}" );
                    }

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
                                // We get here because the current type as a ref to a base property of an INPC type, but we can't use
                                // OnChildPropertyChanged or OnUnmonitoredInpcPropertyChanged from the base type (the base doesn't provide support, or we're
                                // configured not to use it). So this is like retrospecitvely adding a property setter override. Note that
                                // the base *must* provide OnPropertyChanged support for each of its properties as a minimum contract.

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

                                    // TODO: Is this similar to [Frag3] and/or [Frag4] and/or [Frag8]? Can we refactor?
                                    // XXX [Frag6]
                                    // Update methods will deal with notifications - *for those children which have update methods*
                                    foreach ( var method in node.Data.CascadeUpdateMethods )
                                    {
                                        method.Invoke();
                                    }

                                    // Notify refs to the current node and any children without an update method.
                                    foreach ( var name in node.GetAllReferences( includeImmediateChild: n => n.Data.UpdateMethod == null ).Select( n => n.Name ).OrderBy( n => n ) )
                                    {
                                        ctx.OnPropertyChangedMethod.Declaration.Invoke( name );
                                    }
                                    // XXX

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

                                        // Local Function Alert (it's easy to miss when skimming through the code)
                                        // -----------------------------------------------------------------------

                                        void OnChildPropertyChanged( object sender, PropertyChangedEventArgs e )
                                        {
                                            var propertyName = e.PropertyName;

                                            // TODO: If this can be similar to [Frag1] and/or [Frag2] and/or [Frag7], refactor
                                            // XXX [Frag5]
                                            foreach ( var childNode in node.Children )
                                            {
                                                var hasUpdateMethod = childNode.Data.UpdateMethod != null;
                                                var hasRefs = childNode.DirectReferences.Count > 0;

                                                if ( hasUpdateMethod || hasRefs )
                                                {
                                                    if ( propertyName == childNode.Name )
                                                    {
                                                        if ( hasUpdateMethod )
                                                        {
                                                            // Update method will deal with notifications
                                                            childNode.Data.UpdateMethod.Invoke();
                                                        }
                                                        else
                                                        {
                                                            // No update method, notify here.
                                                            foreach ( var refName in childNode.GetAllReferences().Select( n => n.Name ).OrderBy( n => n ) )
                                                            {
                                                                ctx.OnPropertyChangedMethod.Declaration.Invoke( refName );
                                                            }
                                                            ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Name, childNode.Name );
                                                        }
                                                        return;
                                                    }
                                                }
                                            }
                                            // XXX                    
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

                    foreach ( var name in rootPropertyNamesToNotify )
                    {
                        meta.This.OnPropertyChanged( name );
                    }
                }
            }
            else
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.Name}': Because there is nothing to do." );
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
    private static void OnChildPropertyChanged(
        string parentPropertyPath,
        string propertyName,
        [CompileTime] BuildAspectContext ctx )
    {
        if ( ctx.InsertDiagnosticComments )
        {
            meta.InsertComment( "Template: " + nameof( OnChildPropertyChanged ) );
            meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( ( ref NodeData data ) => $"ibh:{data.InpcBaseHandling}" ) );
        }

        foreach ( var node in ctx.DependencyGraph.DecendantsDepthFirst().Where( n => n.Depth > 1 ) )
        {
            var rootPropertyNode = node.GetAncestorOrSelfAtDepth( 1 );

            if ( rootPropertyNode.Data.FieldOrProperty.DeclaringType == ctx.Target )
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.Data.DottedPropertyPath}': Root property '{rootPropertyNode.Name}' is defined by the current type." );
                }
                continue;
            }

            if ( node.Parent!.Data.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged && ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined )
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.Data.DottedPropertyPath}': A base type supports OnUnmonitoredInpcPropertyChanged for the parent property, and the current type is configured to use that feature." );
                }
                continue;
            }

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
        if ( ctx.InsertDiagnosticComments )
        {
            meta.InsertComment( "Template: " + nameof( OnUnmonitoredInpcPropertyChanged ) );
            meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( ( ref NodeData data ) => $"ibh:{data.InpcBaseHandling}" ) );
        }

        // Think of the generated OnUnmonitoredInpcPropertyChanged method as an enhanced overload of OnPropertyChanged, the difference being that
        // the caller provides the old and new values, and it only applies to property types which implement INotifyPropertyChanged.

        // NB: OnUnmonitoredInpcPropertyChanged is only called when a ref has changed - the caller checks this first.
        // NB: Nodes only appear in the graph if they are relevant to the current class.        

        // NB: The logic which selects nodes for which support will be generated must be kept in sync with the logic in
        // AddMetadataForNewlyMonitoredBaseOnUnmonitoredInpcPropertyChangedProperties.

        foreach ( var node in ctx.DependencyGraph.DecendantsDepthFirst().Where( n => n.Data.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged ) )
        {
            if ( ctx.InsertDiagnosticComments )
            {
                meta.InsertComment( $"Node '{node.Data.DottedPropertyPath}'" );
            }
            
            if ( propertyPath == node.Data.DottedPropertyPath )
            {
                // This is very similar to an Update... method or a property setter override, except the base type keeps track of the "old" value so we don't have to.
                // Note that here we may be dealing with a root property defined in another type (so like a setter), or any descendant node (so
                // like an Update method).

                var handlerField = ExpectNotNull( node.Data.HandlerField );
                
                if ( oldValue != null )
                {
                    oldValue.PropertyChanged -= handlerField.Value;
                }

                if ( newValue != null )
                {
                    handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;
                    newValue.PropertyChanged += handlerField.Value;

                    // Local Function Alert (it's easy to miss when skimming through the code)
                    // -----------------------------------------------------------------------

                    void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                    {
                        var propertyName = e.PropertyName;

                        // TODO: If this can be similar to [Frag1] and/or [Frag5] and/or [Frag2], refactor
                        // XXX [Frag7]
                        foreach ( var childNode in node.Children )
                        {
                            var hasUpdateMethod = childNode.Data.UpdateMethod != null;
                            var hasRefs = childNode.DirectReferences.Count > 0;

                            if ( hasUpdateMethod || hasRefs )
                            {
                                if ( propertyName == childNode.Name )
                                {
                                    if ( hasUpdateMethod )
                                    {
                                        // Update method will deal with notifications
                                        childNode.Data.UpdateMethod.Invoke();
                                    }
                                    else
                                    {
                                        // No update method, notify here.
                                        foreach ( var refName in childNode.GetAllReferences().Select( n => n.Name ).OrderBy( n => n ) )
                                        {
                                            ctx.OnPropertyChangedMethod.Declaration.Invoke( refName );
                                        }
                                        ctx.OnChildPropertyChangedMethod.Declaration.Invoke( node.Data.DottedPropertyPath, childNode.Name );
                                    }
                                    return;
                                }
                            }
                        }
                        // XXX
                    }
                }

                // TODO: Is this similar to [Frag3] and/or [Frag6] and/or [Frag4]? Can we refactor?
                // XXX [Frag8]
                // Update methods will deal with notifications - *for those children which have update methods*
                foreach ( var method in node.Data.CascadeUpdateMethods )
                {
                    method.Invoke();
                }

                // Notify refs to the current node and any children without an update method.
                foreach ( var name in node.GetAllReferences( includeImmediateChild: n => n.Data.UpdateMethod == null ).Select( n => n.Name ).OrderBy( n => n ) )
                {
                    ctx.OnPropertyChangedMethod.Declaration.Invoke( name );
                }
                // XXX                
            }
        }

        if ( ctx.BaseOnUnmonitoredInpcPropertyChangedMethod != null )
        {
            meta.Proceed();
        }
    }
}