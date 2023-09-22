// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Code.SyntaxBuilders;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

internal partial class NaturalAspect
{
    [CompileTime]
    private static void CompileTimeThrow( Exception e ) => throw e;

    [CompileTime]
    private static T ExpectNotNull<T>( T? obj ) => obj ?? throw new InvalidOperationException( "A null value was not expected here." );

    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;

    [Template]
    private static dynamic? OverrideInpcRefTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;
            var handlerField = (IField?) meta.Tags["handlerField"];
            var node = (DependencyGraphNode?) meta.Tags["node"];

            var inpcImplementationKind = node?.PropertyTypeInpcInstrumentationKind ?? ctx.GetInpcInstrumentationKind( meta.Target.Property.Type );

            var eventRequiresCast = inpcImplementationKind == InpcInstrumentationKind.Explicit;

            if ( ctx.InsertDiagnosticComments )
            {
                meta.InsertComment( "Template: " + nameof(OverrideInpcRefTypeProperty) );
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
                            meta.Cast( ctx.Elements.INotifyPropertyChanged, oldValue ).PropertyChanged -= handlerField.Value;
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

                    ctx.OnUnmonitoredInpcPropertyChangedMethod.Declaration.With( InvokerOptions.Final )
                        .Invoke( meta.Target.FieldOrProperty.Name, oldValue, value );
                }
                else
                {
                    meta.Target.FieldOrProperty.Value = value;
                }

                if ( node != null )
                {
                    // Update methods will deal with notifications - * for those children which have update methods *
                    foreach ( var method in node.ChildUpdateMethods )
                    {
                        method.With( InvokerOptions.Final ).Invoke();
                    }

                    // Notify refs to the current node and any children without an update method.
                    foreach ( var name in node.GetAllReferences( includeImmediateChild: n => n.UpdateMethod == null ).Select( n => n.Name ).OrderBy( n => n ) )
                    {
                        ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( name );
                    }
                }

                ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( meta.Target.FieldOrProperty.Name );

                if ( handlerField != null )
                {
                    if ( value != null )
                    {
                        handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;

                        if ( eventRequiresCast )
                        {
                            meta.Cast( ctx.Elements.INotifyPropertyChanged, value ).PropertyChanged += handlerField.Value;
                        }
                        else
                        {
                            value.PropertyChanged += handlerField.Value;
                        }

                        // -----------------------------------------------------------------------
                        //                Local Function: OnChildPropertyChanged
                        // -----------------------------------------------------------------------

                        // ReSharper disable once LocalFunctionHidesMethod
                        void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                        {
                            // NB: If handlerField is not null, node must also be non-null.

                            OnChildPropertyChangedDelegateBody( ctx, node!, ExpressionFactory.Capture( e ) );
                        }
                    }
                }
            }
        }
    }

    [Template]
    private static void UpdateChildInpcProperty(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraphNode node,
        [CompileTime] IExpression accessChildExpression,
        [CompileTime] IField lastValueField,
        [CompileTime] IField onPropertyChangedHandlerField )
    {
        if ( node.Depth <= 1 )
        {
            CompileTimeThrow( new InvalidOperationException( $"{nameof(UpdateChildInpcProperty)} template must not be called on a root property node." ) );
        }

        if ( ctx.InsertDiagnosticComments )
        {
            meta.InsertComment( "Template: " + nameof(UpdateChildInpcProperty) );
            meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
        }

        var newValue = accessChildExpression.Value;

        if ( !ReferenceEquals( newValue, lastValueField.Value ) )
        {
            if ( !ReferenceEquals( lastValueField.Value, null ) )
            {
                lastValueField.Value!.PropertyChanged -= onPropertyChangedHandlerField.Value;
            }

            if ( newValue != null )
            {
                onPropertyChangedHandlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;
                newValue.PropertyChanged += onPropertyChangedHandlerField.Value;

                // -----------------------------------------------------------------------
                //                Local Function: OnChildPropertyChanged
                // -----------------------------------------------------------------------

                // ReSharper disable once LocalFunctionHidesMethod
                void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                {
                    OnChildPropertyChangedDelegateBody( ctx, node, ExpressionFactory.Capture( e ) );
                }
            }

            lastValueField.Value = newValue;

            // Update methods will deal with notifications - * for those children which have update methods *
            foreach ( var method in node.ChildUpdateMethods )
            {
                method.With( InvokerOptions.Final ).Invoke();
            }

            // Notify refs to the current node and any children without an update method.
            foreach ( var name in node.GetAllReferences( includeImmediateChild: n => n.UpdateMethod == null ).Select( n => n.Name ).OrderBy( n => n ) )
            {
                ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( name );
            }

            // Don't notify if we're joining on to existing NotifyChildPropertyChanged support from a base type, or we'll be stuck in a loop.
            if ( node.Parent!.InpcBaseHandling != InpcBaseHandling.OnChildPropertyChanged )
            {
                ctx.OnChildPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( node.Parent!.DottedPropertyPath, node.Name );
            }
            else if ( ctx.InsertDiagnosticComments )
            {
                meta.InsertComment(
                    $"Not calling OnChildPropertyChanged('{node.Parent!.DottedPropertyPath}','{node.Name}') because a base type already provides OnChildPropertyChanged support for the parent property." );
            }
        }
    }

    [Template]
    private static dynamic? OverrideUninstrumentedTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;
            var node = (DependencyGraphNode?) meta.Tags["node"];
            var compareUsing = (EqualityComparisonKind) meta.Tags["compareUsing"]!;
            var propertyTypeInstrumentationKind = (InpcInstrumentationKind) meta.Tags["propertyTypeInstrumentationKind"]!;

            if ( ctx.InsertDiagnosticComments )
            {
                meta.InsertComment( "Template: " + nameof(OverrideUninstrumentedTypeProperty) );
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
                EqualityComparisonKind.ThisEquals => (IExpression) !meta.Target.FieldOrProperty.Value!.Equals( value ),
                EqualityComparisonKind.ReferenceEquals => (IExpression) !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ),
                EqualityComparisonKind.DefaultEqualityComparer => (IExpression)
                    !ctx.GetDefaultEqualityComparerForType( meta.Target.FieldOrProperty.Type ).Value!.Equals( value, meta.Target.FieldOrProperty.Value ),
                _ => null
            };

            if ( compareExpr == null )
            {
                meta.Target.FieldOrProperty.Value = value;
                ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( meta.Target.FieldOrProperty.Name );
            }
            else
            {
                if ( compareExpr.Value )
                {
                    meta.Target.FieldOrProperty.Value = value;
                    ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( meta.Target.FieldOrProperty.Name );
                }
            }
        }
    }

    [Template]
    private void OnPropertyChanged( string propertyName, [CompileTime] BuildAspectContext ctx )
    {
        if ( ctx.InsertDiagnosticComments )
        {
            meta.InsertComment( "Template: " + nameof(this.OnPropertyChanged) );
            meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( "[ibh]" ) );
        }

        foreach ( var node in ctx.DependencyGraph.Children )
        {
            if ( node.FieldOrProperty.IsAutoPropertyOrField == true && node.FieldOrProperty.DeclaringType == ctx.Target )
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.Name}': The field or auto property is defined by the current type." );
                }

                continue;
            }

            if ( node.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged && ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined )
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment(
                        $"Skipping '{node.Name}': A base type supports OnUnmonitoredInpcPropertyChanged for this property, and the current type is configured to use that feature." );
                }

                continue;
            }

            IReadOnlyCollection<DependencyGraphNode> refsToNotify;

            // When a base supports OnChildPropertyChanged for a root property, changes to the ref itself will
            // be notified by OnPropertyChanged (the base won't call OnChildPropertyChanged for each child property
            // when the ref changes).

            if ( node is { InpcBaseHandling: InpcBaseHandling.OnChildPropertyChanged, Depth: > 1 } )
            {
                if ( node.DirectReferences.Count == 0 )
                {
                    if ( ctx.InsertDiagnosticComments )
                    {
                        meta.InsertComment(
                            $"Skipping '{node.Name}': A base type supports OnChildPropertyChanged for this property, and the property itself has no references." );
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
                refsToNotify = node.GetAllReferences( includeImmediateChild: n => n.UpdateMethod == null );
            }

            var childUpdateMethods = node.ChildUpdateMethods;

            if ( refsToNotify.Count > 0
                 || childUpdateMethods.Count > 0
                 || node is { HasChildren: true, InpcBaseHandling: InpcBaseHandling.OnUnmonitoredInpcPropertyChanged or InpcBaseHandling.OnPropertyChanged } )
            {
                var rootPropertyNamesToNotify = refsToNotify
                    .Select( n => n.Name )
                    .OrderBy( s => s );

                if ( propertyName == node.Name )
                {
                    var emitDefaultNotifications = meta.CompileTime( true );

                    if ( ctx.InsertDiagnosticComments )
                    {
                        meta.InsertComment( $"InpcBaseHandling = {node.InpcBaseHandling}" );
                    }

                    switch ( node.InpcBaseHandling )
                    {
                        case InpcBaseHandling.Unknown:
                            meta.InsertComment(
                                $"Warning: the type of property '{node.Name}' could not be analysed at design time, so it has been treated",
                                "as not implementing INotifyPropertyChanged. Code generated at compile time may differ." );

                            break;

                        case InpcBaseHandling.NotApplicable:
                        case InpcBaseHandling.OnChildPropertyChanged:
                            break;

                        // NB: The OnUnmonitoredInpcPropertyChanged case, when ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined is true, is handled above.
                        case InpcBaseHandling.OnUnmonitoredInpcPropertyChanged:
                        case InpcBaseHandling.OnPropertyChanged:
                            if ( node.HasChildren )
                            {
                                // We get here because the current type as a ref to a base property of an INPC type, but we can't use
                                // OnChildPropertyChanged or OnUnmonitoredInpcPropertyChanged from the base type (the base doesn't provide support, or we're
                                // configured not to use it). So this is like retrospectively adding a property setter override. Note that
                                // the base *must* provide OnPropertyChanged support for each of its properties as a minimum contract.

                                var handlerField = ExpectNotNull( node.HandlerField );
                                var lastValueField = ExpectNotNull( node.LastValueField );
                                var eventRequiresCast = node.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Explicit;

                                var oldValue = lastValueField.Value;
                                var newValue = node.FieldOrProperty.Value;

                                if ( !ReferenceEquals( oldValue, newValue ) )
                                {
                                    if ( oldValue != null )
                                    {
                                        if ( eventRequiresCast )
                                        {
                                            meta.Cast( ctx.Elements.INotifyPropertyChanged, oldValue ).PropertyChanged -= handlerField.Value;
                                        }
                                        else
                                        {
                                            oldValue.PropertyChanged -= handlerField.Value;
                                        }
                                    }

                                    lastValueField.Value = newValue;

                                    // Update methods will deal with notifications - * for those children which have update methods *
                                    foreach ( var method in childUpdateMethods )
                                    {
                                        method.With( InvokerOptions.Final ).Invoke();
                                    }

                                    // rootPropertyNamesToNotify excludes children with update methods
                                    // ReSharper disable once PossibleMultipleEnumeration
                                    foreach ( var name in rootPropertyNamesToNotify )
                                    {
                                        ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( name );
                                    }

                                    if ( newValue != null )
                                    {
                                        handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;

                                        if ( eventRequiresCast )
                                        {
                                            meta.Cast( ctx.Elements.INotifyPropertyChanged, newValue ).PropertyChanged += handlerField.Value;
                                        }
                                        else
                                        {
                                            newValue.PropertyChanged += handlerField.Value;
                                        }

                                        // -----------------------------------------------------------------------
                                        //                Local Function: OnChildPropertyChanged
                                        // -----------------------------------------------------------------------

                                        // ReSharper disable once LocalFunctionHidesMethod
                                        void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                                        {
                                            OnChildPropertyChangedDelegateBody( ctx, node, ExpressionFactory.Capture( e ) );
                                        }
                                    }
                                }
                            }

                            emitDefaultNotifications = false;

                            break;

                        default:
                            CompileTimeThrow( new InvalidOperationException( $"InpcBaseHandling '{node.InpcBaseHandling}' was not expected here." ) );

                            break;
                    }

                    if ( emitDefaultNotifications )
                    {
                        foreach ( var method in childUpdateMethods )
                        {
                            method.With( InvokerOptions.Final ).Invoke();
                        }

                        // ReSharper disable once PossibleMultipleEnumeration
                        foreach ( var name in rootPropertyNamesToNotify )
                        {
                            ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( name );
                        }
                    }
                }
            }
            else
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.Name}' because there is nothing to do." );
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
            meta.InsertComment( "Template: " + nameof(OnChildPropertyChanged) );
            meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( "[ibh]" ) );
        }

        foreach ( var node in ctx.DependencyGraph.DescendantsDepthFirst().Where( n => n.Depth > 1 ) )
        {
            var rootPropertyNode = node.GetAncestorOrSelfAtDepth( 1 );

            if ( rootPropertyNode.FieldOrProperty.DeclaringType == ctx.Target )
            {
                if ( ctx.InsertDiagnosticComments )
                {
                    meta.InsertComment( $"Skipping '{node.DottedPropertyPath}': Root property '{rootPropertyNode.Name}' is defined by the current type." );
                }

                continue;
            }

            var firstAncestorWithNotNoneHandling = node.Ancestors().FirstOrDefault( n => n.InpcBaseHandling != InpcBaseHandling.None );

            if ( firstAncestorWithNotNoneHandling != null )
            {
                switch ( firstAncestorWithNotNoneHandling.InpcBaseHandling )
                {
                    case InpcBaseHandling.OnUnmonitoredInpcPropertyChanged when ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined:
                        if ( ctx.InsertDiagnosticComments )
                        {
                            meta.InsertComment(
                                $"Skipping '{node.DottedPropertyPath}': A base type supports OnUnmonitoredInpcPropertyChanged for an ancestor of this property, and the current type is configured to use that feature." );
                        }

                        continue;

                    case InpcBaseHandling.OnChildPropertyChanged when node.Depth - firstAncestorWithNotNoneHandling.Depth > 1:
                        if ( ctx.InsertDiagnosticComments )
                        {
                            meta.InsertComment(
                                $"Skipping '{node.DottedPropertyPath}': A base type supports OnChildPropertyChanged for a non-immediate ancestor of this property." );
                        }

                        continue;

                    case InpcBaseHandling.OnPropertyChanged:
                        if ( ctx.InsertDiagnosticComments )
                        {
                            meta.InsertComment(
                                $"Skipping '{node.DottedPropertyPath}': A base type supports OnPropertyChanged for root property '{rootPropertyNode.Name}'." );
                        }

                        continue;
                }
            }

            // NB: The following code is similar to the OnChildPropertyChangedDelegateBody template. Consider keeping any changes to relevant logic in sync.

            var hasUpdateMethod = node.UpdateMethod != null;
            var hasRefs = node.DirectReferences.Count > 0;

            if ( hasUpdateMethod || hasRefs )
            {
                if ( parentPropertyPath == node.Parent!.DottedPropertyPath && propertyName == node.Name )
                {
                    if ( hasUpdateMethod )
                    {
                        // Update method will deal with notifications
                        node.UpdateMethod!.With( InvokerOptions.Final ).Invoke();
                    }
                    else
                    {
                        // No update method, notify here.
                        foreach ( var refName in node.GetAllReferences().Select( n => n.Name ).OrderBy( n => n ) )
                        {
                            ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( refName );
                        }
                    }

                    if ( ctx.BaseOnChildPropertyChangedMethod != null )
                    {
                        meta.Proceed();
                    }

                    return;
                }
            }
        }

        if ( ctx.BaseOnChildPropertyChangedMethod != null )
        {
            meta.Proceed();
        }
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
            meta.InsertComment( "Template: " + nameof(OnUnmonitoredInpcPropertyChanged) );
            meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( "[ibh]" ) );
        }

        /* 
         * The generated OnUnmonitoredInpcPropertyChanged method is like an enhanced overload of OnPropertyChanged, the differences
         * being that the caller provides the old and new values, the method receives a property path rather than a root property name,
         * and it only applies to property types which implement INotifyPropertyChanged.
         * 
         * NB:
         * 
         * - In the current implementation (outside this template), the generated OnUnmonitoredInpcPropertyChanged 
         *   method is only called for root properties. As/when false positive detection is implemented and enabled, 
         *   then the generated OnUnmonitoredInpcPropertyChanged method could receive calls for leaf INotifyPropertyChanged 
         *   properties.
         * 
         * - OnUnmonitoredInpcPropertyChanged is only called when a ref has changed - the caller checks this first.
         * 
         * - For root properties, the caller will also call OnPropertyChanged - this is to maintain compatibility with derivations
         *   which do not observe OnUnmonitoredInpcPropertyChanged.
         * 
         * - Nodes only appear in the graph if they are relevant to the current class.
         * 
         * - The logic which selects nodes for which support will be generated must be kept in sync with the logic in
         *   NaturalAspect.AddPropertyPathsForOnChildPropertyChangedMethodAttribute.
        */

        foreach ( var node in ctx.DependencyGraph.DescendantsDepthFirst().Where( n => n.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged ) )
        {
            if ( ctx.InsertDiagnosticComments )
            {
                meta.InsertComment( $"Node '{node.DottedPropertyPath}'" );
            }

            if ( propertyPath == node.DottedPropertyPath )
            {
                // This is very similar to an Update... method or a property setter override, except the base type keeps track of the "old" value so we don't have to.
                // Note that here we may be dealing with a root property defined in another type (so like a setter), or any descendant node (so
                // like an Update method).

                var handlerField = ExpectNotNull( node.HandlerField );

                if ( oldValue != null )
                {
                    oldValue.PropertyChanged -= handlerField.Value;
                }

                if ( newValue != null )
                {
                    handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;
                    newValue.PropertyChanged += handlerField.Value;

                    // -----------------------------------------------------------------------
                    //                Local Function: OnChildPropertyChanged
                    // -----------------------------------------------------------------------

                    // ReSharper disable once LocalFunctionHidesMethod
                    void OnChildPropertyChanged( object? sender, PropertyChangedEventArgs e )
                    {
                        OnChildPropertyChangedDelegateBody( ctx, node, ExpressionFactory.Capture( e ) );
                    }
                }

                // Update methods will deal with notifications - *for those children which have update methods*
                foreach ( var method in node.ChildUpdateMethods )
                {
                    method.With( InvokerOptions.Final ).Invoke();
                }

                // Notify refs to the current node and any children without an update method.
                foreach ( var name in node.GetAllReferences( includeImmediateChild: n => n.UpdateMethod == null ).Select( n => n.Name ).OrderBy( n => n ) )
                {
                    ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( name );
                }
            }
        }

        if ( ctx.BaseOnUnmonitoredInpcPropertyChangedMethod != null )
        {
            meta.Proceed();
        }
    }

    [Template]
    private static void OnChildPropertyChangedDelegateBody(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraphNode node,
        [CompileTime] IExpression propertyChangedEventArgs )
    {
        var propertyName = propertyChangedEventArgs.Value!.PropertyName;

        foreach ( var childNode in node.Children )
        {
            // NB: The following code is similar to part of the OnChildPropertyChanged template. Consider keeping any changes to relevant logic in sync.

            var hasUpdateMethod = childNode.UpdateMethod != null;
            var hasRefs = childNode.DirectReferences.Count > 0;

            if ( hasUpdateMethod || hasRefs )
            {
                if ( propertyName == childNode.Name )
                {
                    if ( hasUpdateMethod )
                    {
                        // Update method will deal with notifications
                        childNode.UpdateMethod!.With( InvokerOptions.Final ).Invoke();
                    }
                    else
                    {
                        // No update method, notify here.
                        foreach ( var refName in childNode.GetAllReferences().Select( n => n.Name ).OrderBy( n => n ) )
                        {
                            ctx.OnPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( refName );
                        }

                        ctx.OnChildPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( node.DottedPropertyPath, childNode.Name );
                    }

                    return;
                }
            }
        }

        ctx.OnChildPropertyChangedMethod.Declaration.With( InvokerOptions.Final ).Invoke( node.Name, propertyName );
    }
}