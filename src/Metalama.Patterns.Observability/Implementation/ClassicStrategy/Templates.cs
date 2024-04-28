// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Patterns.Observability.Implementation.Graph;
using System.ComponentModel;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal sealed class Templates : ITemplateProvider
{
    internal static TemplateProvider Provider { get; } = TemplateProvider.FromInstance( new Templates() );

    private Templates() { }

    private static void CompileTimeThrow( Exception e ) => throw e;

    // ReSharper disable once EventNeverSubscribedTo.Global
    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;

    [Template]
    internal static void OverrideInpcRefTypePropertySetter(
        dynamic? value,
        [CompileTime] IDeferred<ObservabilityTemplateArgs> templateArgs,
        [CompileTime] IField? handlerField,
        [CompileTime] IMethod? subscribeMethod,
        [CompileTime] IReadOnlyClassicProcessingNode? node )
    {
        var ctx = templateArgs.Value;
        var inpcImplementationKind = node?.PropertyTypeInpcInstrumentationKind ?? ctx.InpcInstrumentationKindLookup.Get( meta.Target.Property.Type );
        var eventRequiresCast = inpcImplementationKind == InpcInstrumentationKind.Explicit;

        if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 0 )
        {
            meta.InsertComment( "Template: " + nameof(OverrideInpcRefTypePropertySetter) );

            if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 1 )
            {
                meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
            }
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
                            meta.Cast( ctx.Assets.INotifyPropertyChanged, oldValue ).PropertyChanged -= handlerField.Value;
                        }
                        else
                        {
                            oldValue.PropertyChanged -= handlerField.Value;
                    }
                }

                meta.Target.FieldOrProperty.Value = value;
            }
            else if ( ctx.OnObservablePropertyChangedMethod != null )
            {
                var oldValue = meta.Target.FieldOrProperty.Value;
                meta.Target.FieldOrProperty.Value = value;

                ctx.OnObservablePropertyChangedMethod.With( InvokerOptions.Final )
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
                foreach ( var r in node.AllReferencedBy( shouldIncludeImmediateChild: n => n.UpdateMethod.Value == null )
                             .Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private )
                             .OrderBy( n => n.Name ) )
                {
                    ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( r.Name );
                }
            }

            if ( node?.FieldOrProperty.DeclarationKind != DeclarationKind.Field && meta.Target.FieldOrProperty.Accessibility != Accessibility.Private )
            {
                ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( meta.Target.FieldOrProperty.Name );
            }

#pragma warning disable IDE0031 // Use null propagation
            if ( subscribeMethod != null )
            {
                subscribeMethod.Invoke( value );
            }
#pragma warning restore IDE0031 // Use null propagation
        }
    }

    [Template]
    internal static void SubscribeTo<[CompileTime] TValue>(
        TValue? value,
        [CompileTime] IDeferred<ObservabilityTemplateArgs> templateArgs,
        [CompileTime] ClassicProcessingNode node,
        [CompileTime] IField handlerField )
        where TValue : class, INotifyPropertyChanged
    {
        var ctx = templateArgs.Value;
        var inpcImplementationKind = node.PropertyTypeInpcInstrumentationKind;
        var eventRequiresCast = inpcImplementationKind == InpcInstrumentationKind.Explicit;

        if ( value != null )
        {
            handlerField.Value ??= (PropertyChangedEventHandler) Handle;

            if ( eventRequiresCast )
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                meta.Cast( ctx.Assets.INotifyPropertyChanged, value ).PropertyChanged += handlerField.Value;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            else
            {
                value.PropertyChanged += handlerField.Value;
            }
        }

        // -----------------------------------------------------------------------
        //                Local Function: OnChildPropertyChanged
        // -----------------------------------------------------------------------

        // ReSharper disable once LocalFunctionHidesMethod
        void Handle( object? sender, PropertyChangedEventArgs e )
        {
            // We use WithNullability to work around a bug in Metalama.Framework (perhaps rather in Roslyn) that randomly gives no
            // nullability information for 'e'.
            OnChildPropertyChangedDelegateBody( ctx, node, ExpressionFactory.Capture( e ).WithNullability( false ) );
        }
    }

    [Template]
    internal static void SubscribeInitializer(
        [CompileTime] IFieldOrProperty fieldOrProperty,
        [CompileTime] IMethod subscribeMethod )
    {
        subscribeMethod.Invoke( fieldOrProperty.Value );
    }

    [Template]
    internal static void UpdateChildInpcProperty(
        [CompileTime] IDeferred<ObservabilityTemplateArgs> templateArgs,
        [CompileTime] ClassicProcessingNode node,
        [CompileTime] IExpression accessChildExpression,
        [CompileTime] IField lastValueField,
        [CompileTime] IField onPropertyChangedHandlerField )
    {
        var ctx = templateArgs.Value;

        if ( node.Depth <= 1 )
        {
            CompileTimeThrow( new InvalidOperationException( $"{nameof(UpdateChildInpcProperty)} template must not be called on a root property node." ) );
        }

        if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 0 )
        {
            meta.InsertComment( "Template: " + nameof(UpdateChildInpcProperty) );

            if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 1 )
            {
                meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
            }
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
            foreach ( var r in node.AllReferencedBy( shouldIncludeImmediateChild: n => n.UpdateMethod.Value == null )
                         .Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private )
                         .OrderBy( n => n.Name ) )
            {
                ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( r.Name );
            }

            if ( node.Parent.FieldOrProperty.Accessibility != Accessibility.Private )
            {
                // Don't notify if we're joining on to existing NotifyChildPropertyChanged support from a base type, or we'll be stuck in a loop.
                if ( node.Parent.InpcBaseHandling != InpcBaseHandling.OnChildPropertyChanged )
                {
                    ctx.OnChildPropertyChangedMethod!.With( InvokerOptions.Final ).Invoke( node.Parent.DottedPropertyPath, node.Name );
                }
                else if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 0 )
                {
                    meta.InsertComment(
                        $"Not calling OnChildPropertyChanged('{node.Parent.DottedPropertyPath}','{node.Name}') because a base type already provides OnChildPropertyChanged support for the parent property." );
                }
            }
            else
            {
                meta.InsertComment( $"Not calling OnChildPropertyChanged for private parent field or property '{node.Parent.DottedPropertyPath}'." );
            }
        }
    }

    [Template]
    internal static void OverrideUninstrumentedTypePropertySetter(
        dynamic? value,
        [CompileTime] IDeferred<ObservabilityTemplateArgs> templateArgs,
        [CompileTime] IReadOnlyClassicProcessingNode? node,
        [CompileTime] EqualityComparisonKind compareUsing,
        [CompileTime] InpcInstrumentationKind propertyTypeInstrumentationKind )
    {
        var ctx = templateArgs.Value;

        if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 0 )
        {
            meta.InsertComment( "Template: " + nameof(OverrideUninstrumentedTypePropertySetter) );

            if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 1 )
            {
                meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
            }
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
                !ctx.Assets.GetDefaultEqualityComparerForType( meta.Target.FieldOrProperty.Type ).Value!.Equals( value, meta.Target.FieldOrProperty.Value ),
            _ => null
        };

        var isField = node?.FieldOrProperty.DeclarationKind == DeclarationKind.Field;

        if ( compareExpr == null )
        {
            meta.Target.FieldOrProperty.Value = value;

            if ( node != null )
            {
                foreach ( var r in node.AllReferencedBy().Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private ).OrderBy( n => n.Name ) )
                {
                    ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( r.Name );
                }
            }

            if ( !isField && meta.Target.FieldOrProperty.Accessibility != Accessibility.Private )
            {
                ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( meta.Target.FieldOrProperty.Name );
            }
        }
        else
        {
            if ( compareExpr.Value )
            {
                meta.Target.FieldOrProperty.Value = value;

                if ( node != null )
                {
                    foreach ( var r in node.AllReferencedBy().Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private ).OrderBy( n => n.Name ) )
                    {
                        ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( r.Name );
                    }
                }

                if ( !isField && meta.Target.FieldOrProperty.Accessibility != Accessibility.Private )
                {
                    ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( meta.Target.FieldOrProperty.Name );
                }
            }
        }
    }

    [Template]
    internal void OnPropertyChanged(
        string propertyName,
        [CompileTime] IDeferred<ObservabilityTemplateArgs> templateArgs )
    {
        var templateArgsValue = templateArgs.Value;

        if ( templateArgsValue.CommonOptions.DiagnosticCommentVerbosity! > 0 )
        {
            meta.InsertComment( "Template: " + nameof(this.OnPropertyChanged) );

            if ( templateArgsValue.CommonOptions.DiagnosticCommentVerbosity! > 1 )
            {
                meta.InsertComment( "Dependency graph:", "\n" + templateArgsValue.DependencyGraph.ToString( "[ibh]" ) );
            }
        }

        foreach ( var node in templateArgsValue.DependencyGraph.Children )
        {
            if ( node.FieldOrProperty.DeclaringType == templateArgsValue.TargetType )
            {
                if ( templateArgsValue.CommonOptions.DiagnosticCommentVerbosity! > 0 )
                {
                    meta.InsertComment( $"Skipping '{node.Name}': The field or property is defined by the current type." );
                }

                continue;
            }

            if ( node.InpcBaseHandling == InpcBaseHandling.OnObservablePropertyChanged && templateArgsValue.OnObservablePropertyChangedMethod != null )
            {
                if ( templateArgsValue.CommonOptions.DiagnosticCommentVerbosity! > 0 )
                {
                    meta.InsertComment(
                        $"Skipping '{node.Name}': A base type supports OnObservablePropertyChanged for this property, and the current type is configured to use that feature." );
                }

                continue;
            }

            IReadOnlyCollection<IReadOnlyClassicProcessingNode> refsToNotify;

            // When a base supports OnChildPropertyChanged for a root property, changes to the ref itself will
            // be notified by OnPropertyChanged (the base won't call OnChildPropertyChanged for each child property
            // when the ref changes).

            if ( node is { InpcBaseHandling: InpcBaseHandling.OnChildPropertyChanged, Depth: > 1 } )
            {
                if ( node.ReferencedBy.Count == 0 )
                {
                    if ( templateArgsValue.CommonOptions.DiagnosticCommentVerbosity! > 0 )
                    {
                        meta.InsertComment(
                            $"Skipping '{node.Name}': A base type supports OnChildPropertyChanged for this property, and the property itself has no references." );
                    }

                    continue;
                }
                else
                {
                    // Only notify references to the node itself, child node changes will be handled via OnChildPropertyChanged.
                    refsToNotify = node.AllReferencedBy();
                }
            }
            else
            {
                // Notify refs to the current node and any children without an update method.
                refsToNotify = node.AllReferencedBy( shouldIncludeImmediateChild: n => n.UpdateMethod.Value == null );
            }

            var childUpdateMethods = node.ChildUpdateMethods;

            if ( (refsToNotify.Count > 0 && refsToNotify.Any( n => n.FieldOrProperty.Accessibility != Accessibility.Private ))
                 || childUpdateMethods.Count > 0
                 || node is
                 {
                     HasChildren: true, InpcBaseHandling: InpcBaseHandling.OnObservablePropertyChanged or InpcBaseHandling.OnPropertyChanged
                 } )
            {
                var rootPropertyNamesToNotify = refsToNotify
                    .Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private )
                    .Select( n => n.Name )
                    .OrderBy( s => s );

                if ( propertyName == node.Name )
                {
                    var emitDefaultNotifications = meta.CompileTime( true );

                    if ( templateArgsValue.CommonOptions.DiagnosticCommentVerbosity! > 0 )
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

                        // NB: The OnObservablePropertyChanged case, when ctx.OnObservablePropertyChangedMethod.WillBeDefined is true, is handled above.
                        case InpcBaseHandling.OnObservablePropertyChanged:
                        case InpcBaseHandling.OnPropertyChanged:
                            if ( node.HasChildren )
                            {
                                // We get here because the current type as a ref to a base property of an INPC type, but we can't use
                                // OnChildPropertyChanged or OnObservablePropertyChanged from the base type (the base doesn't provide support, or we're
                                // configured not to use it). So this is like retrospectively adding a property setter override. Note that
                                // the base *must* provide OnPropertyChanged support for each of its properties as a minimum contract.

                                var handlerField = node.HandlerField.Value;

                                if ( !node.LastValueField.HasBeenSet )
                                {
                                    meta.InsertComment( $"Error: LastValueField for {node.FieldOrProperty} has not been set." );
                                }
                                else
                                {

                                    var lastValueField = node.LastValueField.Value;
                                    var eventRequiresCast = node.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Explicit;

                                    var oldValue = lastValueField.Value;
                                    var newValue = node.FieldOrProperty.Value;

                                    if ( !ReferenceEquals( oldValue, newValue ) )
                                    {
                                        if ( oldValue != null )
                                        {
                                            if ( eventRequiresCast )
                                            {
                                                meta.Cast( templateArgsValue.Assets.INotifyPropertyChanged, oldValue ).PropertyChanged -= handlerField.Value;
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
                                            templateArgsValue.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( name );
                                        }

                                        if ( newValue != null )
                                        {
                                            handlerField.Value ??= (PropertyChangedEventHandler) OnChildPropertyChanged;

                                            if ( eventRequiresCast )
                                            {
                                                meta.Cast( templateArgsValue.Assets.INotifyPropertyChanged, newValue )!.PropertyChanged += handlerField.Value;
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
                                                OnChildPropertyChangedDelegateBody( templateArgsValue, node, ExpressionFactory.Capture( e ) );
                                            }
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
                            templateArgsValue.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( name );
                        }
                    }
                }
            }
            else
            {
                if ( templateArgsValue.CommonOptions.DiagnosticCommentVerbosity! > 0 )
                {
                    meta.InsertComment( $"Skipping '{node.Name}' because there is nothing to do." );
                }
            }
        }

        if ( templateArgsValue.BaseOnPropertyChangedMethod == null )
        {
            this.PropertyChanged?.Invoke( meta.This, new PropertyChangedEventArgs( propertyName ) );
        }
        else
        {
            meta.Proceed();
        }
    }

    [Template]
    internal static void OnChildPropertyChanged(
        string parentPropertyPath,
        string propertyName,
        [CompileTime] IDeferred<ObservabilityTemplateArgs> templateArgs,
        IReadOnlyList<IReadOnlyClassicProcessingNode> nodesForOnChildPropertyChanged )
    {
        var ctx = templateArgs.Value;

        if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 0 )
        {
            meta.InsertComment( "Template: " + nameof(OnChildPropertyChanged) );

            if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 1 )
            {
                meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( "[ibh]" ) );
            }
        }

        foreach ( var node in nodesForOnChildPropertyChanged )
        {
            

            // NB: The following code is similar to the OnChildPropertyChangedDelegateBody template. Consider keeping any changes to relevant logic in sync.

            var hasUpdateMethod = node.UpdateMethod.Value != null;
            var hasRefs = node.ReferencedBy.Count > 0;

            if ( hasUpdateMethod || hasRefs )
            {
                if ( parentPropertyPath == node.Parent.DottedPropertyPath && propertyName == node.Name )
                {
                    if ( hasUpdateMethod )
                    {
                        // Update method will deal with notifications
                        node.UpdateMethod.Value!.With( InvokerOptions.Final ).Invoke();
                    }
                    else
                    {
                        // No update method, notify here.
                        foreach ( var r in node.AllReferencedBy()
                                     .Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private )
                                     .OrderBy( n => n.Name ) )
                        {
                            ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( r.Name );
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
    internal static void OnObservablePropertyChanged(
        string propertyPath,
        INotifyPropertyChanged? oldValue,
        INotifyPropertyChanged? newValue,
        [CompileTime] IDeferred<ObservabilityTemplateArgs> templateArgs,
        IReadOnlyList<IReadOnlyClassicProcessingNode> nodesProcessedByOnObservablePropertyChanged )
    {
        var ctx = templateArgs.Value;

        if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 0 )
        {
            meta.InsertComment( "Template: " + nameof(OnObservablePropertyChanged) );

            if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 1 )
            {
                meta.InsertComment( "Dependency graph:", "\n" + ctx.DependencyGraph.ToString( "[ibh]" ) );
            }
        }

        /*
         * The generated OnObservablePropertyChanged method is like an enhanced overload of OnPropertyChanged, the differences
         * being that the caller provides the old and new values, the method receives a property path rather than a root property name,
         * and it only applies to property types which implement INotifyPropertyChanged.
         *
         * NB:
         *
         * - In the current implementation (outside this template), the generated OnObservablePropertyChanged
         *   method is only called for root properties. As/when false positive detection is implemented and enabled,
         *   then the generated OnObservablePropertyChanged method could receive calls for leaf INotifyPropertyChanged
         *   properties.
         *
         * - OnObservablePropertyChanged is only called when a ref has changed - the caller checks this first.
         *
         * - For root properties, the caller will also call OnPropertyChanged - this is to maintain compatibility with derivations
         *   which do not observe OnObservablePropertyChanged.
         *
         * - Nodes only appear in the graph if they are relevant to the current class.
         *
         * - The logic which selects nodes for which support will be generated must be kept in sync with the logic in
         *   NaturalAspect.AddPropertyPathsForOnChildPropertyChangedMethodAttribute.
         */

       
        foreach ( var node in nodesProcessedByOnObservablePropertyChanged )
        {
            if ( ctx.CommonOptions.DiagnosticCommentVerbosity! > 0 )
            {
                meta.InsertComment( $"Node '{node.DottedPropertyPath}'" );
            }

            if ( propertyPath == node.DottedPropertyPath )
            {
                // This is very similar to an Update... method or a property setter override, except the base type keeps track of the "old" value so we don't have to.
                // Note that here we may be dealing with a root property defined in another type (so like a setter), or any descendant node (so
                // like an Update method).

                var handlerField = node.HandlerField.Value;

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
                foreach ( var r in node.AllReferencedBy( shouldIncludeImmediateChild: n => n.UpdateMethod.Value == null )
                             .Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private )
                             .OrderBy( n => n.Name ) )
                {
                    ctx.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( r.Name );
                }
            }
        }

        if ( ctx.BaseOnObservablePropertyChangedMethod != null )
        {
            meta.Proceed();
        }
    }

    [Template]
    private static void OnChildPropertyChangedDelegateBody(
        [CompileTime] ObservabilityTemplateArgs templateArgs,
        [CompileTime] IReadOnlyClassicProcessingNode node,
        [CompileTime] IExpression propertyChangedEventArgs )
    {
        if ( templateArgs.CommonOptions.DiagnosticCommentVerbosity! > 0 )
        {
            meta.InsertComment( "Template: " + nameof(OnChildPropertyChangedDelegateBody) );

            if ( templateArgs.CommonOptions.DiagnosticCommentVerbosity! > 1 )
            {
                meta.InsertComment( "Dependency graph:", "\n" + templateArgs.DependencyGraph.ToString( node, "[ibh]" ) );
            }
        }

        var propertyName = propertyChangedEventArgs.Value!.PropertyName;
        var nodeIsAccessible = node.FieldOrProperty.Accessibility != Accessibility.Private;

        foreach ( var childNode in node.Children )
        {
            // NB: The following code is similar to part of the OnChildPropertyChanged template. Consider keeping any changes to relevant logic in sync.

            var hasUpdateMethod = childNode.UpdateMethod.Value != null;
            var hasRefs = childNode.ReferencedBy.Count > 0;

            if ( hasUpdateMethod || hasRefs )
            {
                if ( propertyName == childNode.Name )
                {
                    if ( hasUpdateMethod )
                    {
                        // Update method will deal with notifications
                        childNode.UpdateMethod.Value!.With( InvokerOptions.Final ).Invoke();
                    }
                    else
                    {
                        // No update method, notify here.
                        foreach ( var r in childNode.AllReferencedBy()
                                     .Where( n => n.FieldOrProperty.Accessibility != Accessibility.Private )
                                     .OrderBy( n => n.Name ) )
                        {
                            templateArgs.OnPropertyChangedMethod.With( InvokerOptions.Final ).Invoke( r.Name );
                        }

                        if ( nodeIsAccessible )
                        {
                            templateArgs.OnChildPropertyChangedMethod!.With( InvokerOptions.Final ).Invoke( node.DottedPropertyPath, childNode.Name );
                        }
                    }

                    return;
                }
            }
        }

        if ( nodeIsAccessible )
        {
            templateArgs.OnChildPropertyChangedMethod!.With( InvokerOptions.Final ).Invoke( node.DottedPropertyPath, propertyName );
        }
    }
}