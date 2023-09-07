using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Metalama.Patterns.NotifyPropertyChanged;

public partial class NotifyPropertyChangedAttribute
{
    [Template]
    private static dynamic? OverrideInpcRefTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;
            var handlerField = (IField?) meta.Tags["handlerField"];
            var node = (DependencyGraph.Node<NodeData>?) meta.Tags["node"];
            var eventRequiresCast = ctx.GetInpcInstrumentationKind( meta.Target.Property.Type ) is InpcInstrumentationKind.Explicit;

            meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );

            if ( !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ) )
            {
                if ( node != null )
                {
                    var oldValue = meta.Target.FieldOrProperty.Value;

                    if ( handlerField != null )
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
                    }
                }

                meta.Target.FieldOrProperty.Value = value;
                
                GenerateNotificationsAndCascadingUpdates( ctx, node, meta.Target.Property.Name );

                if ( node != null )
                {
                    if ( handlerField != null )
                    {
                        if ( value != null )
                        {
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

            // This must be implemented as a local function because Metalama does not currently support delegates in any other way.
            void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
            {
                // TODO: Don't emit this method at all if not needed.
                // Currently, by my reckoning, the only way to do this is to have two variants of the OverrideInpcRefTypeProperty
                // template, one with the local method and one without. Alternatively, richer support for delegates in templates
                // could provide an cleaner solution.
                if ( node != null )
                {
                    // TODO: Subtemplates emit unwanted extra nesting inside curly braces if the subtemplate defines local vars.
                    // So for emit the local var here then call the subtemplate to have more idomatic generated code.
                    // Also see other uses of GenerateBodyOfOnSpecificPropertyChanged.
                    var propertyName = e.PropertyName;
                    var getPropertyNameExpression = ExpressionFactory.Capture( propertyName );

                    GenerateBodyOfOnSpecificPropertyChanged( ctx, node, getPropertyNameExpression );
                }
            }
        }
    }

    [CompileTime]
    private enum EqualityComparisonKind
    {
        ReferenceEquals,
        ThisEquals,
        EqualityOperator,
        None
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

            meta.InsertComment( "Dependency graph (current node highlighted if defined):", "\n" + ctx.DependencyGraph.ToString( node ) );
            
            if ( propertyTypeInstrumentationKind == InpcInstrumentationKind.Unknown )
            {
                meta.InsertComment(
                    "Warning: the type of this property could not be analysed at design time, so it has been treated",
                    "as not implementing INotifyPropertyChanged. Code generated at compile time may differ." );
            }

            switch ( compareUsing )
            {
                case EqualityComparisonKind.EqualityOperator:
                    if ( meta.Target.FieldOrProperty.Value != value )
                    {
                        meta.Target.FieldOrProperty.Value = value;
                        GenerateNotificationsAndCascadingUpdates( ctx, node, meta.Target.Property.Name );
                    }

                    break;

                case EqualityComparisonKind.ThisEquals:
                    if ( !meta.Target.FieldOrProperty.Value.Equals( value ) )
                    {
                        meta.Target.FieldOrProperty.Value = value;
                        GenerateNotificationsAndCascadingUpdates( ctx, node, meta.Target.Property.Name );
                    }

                    break;

                case EqualityComparisonKind.ReferenceEquals:
                    if ( !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ) )
                    {
                        meta.Target.FieldOrProperty.Value = value;
                        GenerateNotificationsAndCascadingUpdates( ctx, node, meta.Target.Property.Name );
                    }

                    break;

                case EqualityComparisonKind.None:
                    meta.Target.FieldOrProperty.Value = value;
                    GenerateNotificationsAndCascadingUpdates( ctx, node, meta.Target.Property.Name );

                    break;

                default:
                    throw new NotSupportedException( compareUsing.ToString() );
            }
        }
    }

    // TODO: Make this private pending #33686
    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;

    [Template]
    private void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        this.PropertyChanged?.Invoke( meta.This, new PropertyChangedEventArgs( propertyName ) );
    }

    [Template]
    private static void UpdateChildren( [CompileTime] IEnumerable<IMethod> updateChildPropertyMethods ) // UpdateA2Children
    {
        // UpdateA2B2();
        // ...
        foreach ( var method in updateChildPropertyMethods )
        {
            method.Invoke();
        }
    }

    [Template]
    private static void UpdateChildProperty(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData> node,
        [CompileTime] IExpression accessChildExpression, 
        [CompileTime] IField lastValueField,
        [CompileTime] IField onPropertyChangedHandlerField )
    {
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

            GenerateNotificationsAndCascadingUpdates( ctx, node, node.Symbol.Name );
        }

        void OnSpecificPropertyChanged( object? sender, PropertyChangedEventArgs e )
        {
            var propertyName = e.PropertyName;
            var getPropertyNameExpression = ExpressionFactory.Capture( propertyName );

            GenerateBodyOfOnSpecificPropertyChanged( ctx, node, getPropertyNameExpression );
        }
    }

    [Template]
    private static void GenerateNotificationsAndCascadingUpdates(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData>? node,
        [CompileTime] string propertyName )
    {
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
                .Select( n => n.Symbol.Name )
                .OrderBy( s => s );

            foreach ( var name in affectedPropertyNames )
            {
                ctx.OnPropertyChangedMethod.Invoke( name );
            }
        }

        // node is null for unreferenced (according to static compile time analsysis) root properties.
        if ( node == null || node.Parent!.IsRoot )
        {
            ctx.OnPropertyChangedMethod.Invoke( propertyName );
        }
    }

    [Template]
    private static void GenerateBodyOfOnSpecificPropertyChanged(
        [CompileTime] BuildAspectContext ctx,
        [CompileTime] DependencyGraph.Node<NodeData> node,        
        [CompileTime] IExpression getPropertyNameExpression )
    {
        // TODO: How to build a switch statement nicely in a template?
        // For now, use if. Also, might use a runtime static readonly dictionary at least for the OnPropertyChanged calls.
        foreach ( var child in node.Children )
        {
            var hasRefs = child.DirectReferences.Count > 0;
            var hasUpdateMethod = child.Data.UpdateMethod != null;

            if ( hasRefs || hasUpdateMethod )
            {
                if ( getPropertyNameExpression.Value == child.Symbol.Name )
                {
                    if ( hasUpdateMethod )
                    {
                        child.Data.UpdateMethod.Invoke();
                    }

                    if ( hasRefs )
                    {
                        var affectedPropertyNames = child.GetAllReferences().Select( n => n.Symbol.Name ).OrderBy( s => s );

                        foreach ( var name in affectedPropertyNames )
                        {
                            ctx.OnPropertyChangedMethod.Invoke( name );
                        }
                    }

                    // TODO: How to build an if..else if.. statement nicely in a template?
                    // For now, use return.
                    return;
                }
            }
        }
    }
}