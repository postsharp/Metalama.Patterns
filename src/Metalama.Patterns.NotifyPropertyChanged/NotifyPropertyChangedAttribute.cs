using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Framework.Code.SyntaxBuilders;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Metalama.Patterns.NotifyPropertyChanged;

/* Notes
 * 
 * PS impl does not appear to support *explicit* user INPC impl - PropertyChanged must be implicit.
 * 
 * What is supposed to happen with indexers?
 * 
 */

[AttributeUsage( AttributeTargets.Class )]
[Inheritable]
public sealed class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
{
    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();
    }

    private enum InpcInstrumentationKind
    {
        None,
        Implicit,
        Explicit
    }

    private sealed class BuildAspectContext
    {
        private readonly Dictionary<IType, InpcInstrumentationKind> _inpcInstrumentationKindLookup = new();

        public BuildAspectContext( IAspectBuilder<INamedType> builder )
        {
            this.Builder = builder;
            this.Type_INotifyPropertyChanged = (INamedType) TypeFactory.GetType( typeof( INotifyPropertyChanged ) );
            this.Event_INotifyPropertyChanged_PropertyChanged = this.Type_INotifyPropertyChanged.Events.First();
            this.Type_PropertyChangedEventHandler = (INamedType) TypeFactory.GetType( typeof( PropertyChangedEventHandler ) );
            this.Type_IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof( IgnoreAutoChangeNotificationAttribute ) );
        }

        public IAspectBuilder<INamedType> Builder { get; }

        public INamedType Target => this.Builder.Target;

        public INamedType Type_INotifyPropertyChanged { get; }

        public IEvent Event_INotifyPropertyChanged_PropertyChanged { get; }

        public INamedType Type_PropertyChangedEventHandler { get; }

        public INamedType Type_IgnoreAutoChangeNotificationAttribute { get; }

        public IMethod OnPropertyChangedMethod { get; set; } = null!;

        private DependencyHelper.TreeNode<TreeNodeData>? _dependencyGraph;

        private DependencyHelper.TreeNode<TreeNodeData> PrepareDependencyGraph()
        {
            var graph = DependencyHelper.GetDependencyGraph<TreeNodeData>( this.Target );
            foreach ( var node in graph.DecendantsDepthFirst() )
            {
                node.Data.Initialize( this, node );
            }
            return graph;
        }

        public DependencyHelper.TreeNode<TreeNodeData> DependencyGraph => this._dependencyGraph ??= this.PrepareDependencyGraph();

        public InpcInstrumentationKind GetInpcInstrumentationKind( IType type )
        {
            if ( this._inpcInstrumentationKindLookup.TryGetValue( type, out var result ) )
            {
                return result;
            }
            else
            {
                result = Check( type );
                this._inpcInstrumentationKindLookup.Add( type, result );
                return result;
            }

            InpcInstrumentationKind Check( IType type )
            {
                switch ( type )
                {
                    case INamedType namedType:
                        if ( namedType.Equals( this.Type_INotifyPropertyChanged ) )
                        {
                            return InpcInstrumentationKind.Implicit;
                        }
                        else if ( namedType.Is( this.Type_INotifyPropertyChanged ) )
                        {
                            if ( namedType.TryFindImplementationForInterfaceMember( this.Event_INotifyPropertyChanged_PropertyChanged, out var result ) )
                            {
                                return result.IsExplicitInterfaceImplementation ? InpcInstrumentationKind.Explicit : InpcInstrumentationKind.Implicit;
                            }

                            throw new InvalidOperationException();
                        }
                        else if ( namedType.Enhancements().HasAspect<NotifyPropertyChangedAttribute>() )
                        {
                            // For now, the aspect always introduces implicit implementation.
                            return InpcInstrumentationKind.Implicit;
                        }
                        else
                        {
                            return InpcInstrumentationKind.None;
                        }

                    case ITypeParameter typeParameter:
                        var hasImplicit = false;

                        foreach ( var t in typeParameter.TypeConstraints )
                        {
                            var k = this.GetInpcInstrumentationKind( t );
                            switch ( k )
                            {
                                case InpcInstrumentationKind.Implicit:
                                    return InpcInstrumentationKind.Implicit;

                                case InpcInstrumentationKind.Explicit:
                                    hasImplicit = true;
                                    break;
                            }
                        }

                        return hasImplicit ? InpcInstrumentationKind.Implicit : InpcInstrumentationKind.None;

                    default:
                        return InpcInstrumentationKind.None;
                }
            }
        }
    }

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var ctx = new BuildAspectContext( builder );

        var target = builder.Target;

        var implementsInpc = target.Is( ctx.Type_INotifyPropertyChanged );

        IMethod? onPropertyChangedMethod = null;

        if ( implementsInpc )
        {
            onPropertyChangedMethod = GetOnPropertyChangedMethod( target );

            if ( onPropertyChangedMethod == null )
            {
                builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( target ) );

                return;
            }
        }
        else
        {
            var implementInterfaceResult = builder.Advice.ImplementInterface( target, ctx.Type_INotifyPropertyChanged, OverrideStrategy.Fail );

            var introduceOnPropertyChangedMethodResult = builder.Advice.IntroduceMethod(
                builder.Target,
                nameof( this.OnPropertyChanged ),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b =>
                {
                    if ( target.IsSealed )
                    {
                        b.Accessibility = Accessibility.Private;
                    }
                    else
                    {
                        b.Accessibility = Accessibility.Protected;
                        b.IsVirtual = true;
                    }
                } );

            onPropertyChangedMethod = introduceOnPropertyChangedMethodResult.Declaration;
        }

        ctx.OnPropertyChangedMethod = onPropertyChangedMethod;

        GenerateUpdateMethods( ctx );
        //ProcessAutoProperties( ctx );
    }

    private struct TreeNodeData
    {
        public void Initialize( BuildAspectContext ctx, DependencyHelper.TreeNode<TreeNodeData> node )
        {
            // TODO: Better checks/exceptions.
            this.FieldOrProperty = (IFieldOrProperty) ctx.Target.Compilation.GetDeclaration( node.Symbol );
        }

        public IFieldOrProperty FieldOrProperty { get; private set; }

        public IMethod? UpdateMethod;
    }

    private static void GenerateUpdateMethods( BuildAspectContext ctx )
    {
        var allNodesDepthFirst = ctx.DependencyGraph.DecendantsDepthFirst().ToList();
        allNodesDepthFirst.Reverse();

        HashSet<string>? usedMemberNames = null;

        foreach ( var node in allNodesDepthFirst )
        {
            if ( node.ReferencedBy.Count > 0 )
            {
                var parent = node.Parent;

                if ( parent == null )
                {
                    // node is a root property of the target type. Do we need to do anything for it?
                    continue;
                }

                if ( parent.Data.UpdateMethod == null )
                {
                    // eg, for node X.Y.Z, parentElementNames is [X,Y]
                    var parentElementNames = parent.AncestorsAndSelf().Reverse().Select( n => n.Symbol.Name ).ToList();

                    var pathForMemberNames = string.Join( "", parentElementNames );

                    var lastValueFieldName = GetUnusedMemberName(
                        ctx.Target,
                        $"_last{pathForMemberNames}",
                        ref usedMemberNames );
                   
                    var introduceLastValueFieldResult = ctx.Builder.Advice.IntroduceField(
                        ctx.Target,
                        lastValueFieldName,
                        parent.Data.FieldOrProperty.Type,
                        IntroductionScope.Instance,
                        OverrideStrategy.Fail,
                        b => b.Accessibility = Accessibility.Private );

                    var lastValueField = introduceLastValueFieldResult.Declaration;

                    var onPropertyChangedHandlerFieldName = GetUnusedMemberName(
                        ctx.Target,
                        $"_on{pathForMemberNames}ChangedHandler",
                        ref usedMemberNames );

                    var introduceOnPropertyChangedHandlerFieldName = ctx.Builder.Advice.IntroduceField(
                        ctx.Target,
                        onPropertyChangedHandlerFieldName,
                        ctx.Type_PropertyChangedEventHandler,
                        IntroductionScope.Instance,
                        OverrideStrategy.Fail,
                        b => b.Accessibility = Accessibility.Private );

                    var onPropertyChangedHandlerField = introduceOnPropertyChangedHandlerFieldName.Declaration;

                    var methodName = GetUnusedMemberName(
                                            ctx.Target,
                                            $"Update{pathForMemberNames}",
                                            ref usedMemberNames );

                    var accessChildExprBuilder = new ExpressionBuilder();
                    // TODO: Assuming all ref types, which is probably a correct requirement, but we don't actaully check anywhere (I think).
                    accessChildExprBuilder.AppendVerbatim( string.Join( "?.", parentElementNames ) );

                    var accessChildExpression = accessChildExprBuilder.ToExpression();

                    ctx.Builder.Advice.IntroduceMethod(
                        ctx.Target,
                        nameof( UpdateChildProperty ),
                        IntroductionScope.Instance,
                        OverrideStrategy.Fail,
                        b =>
                        {
                            b.Name = methodName;

                            if ( ctx.Target.IsSealed )
                            {
                                b.Accessibility = Accessibility.Private;
                            }
                            else
                            {
                                b.Accessibility = Accessibility.Protected;
                                b.IsVirtual = true;
                            }
                        },
                        args: new
                        {
                            accessChildExpression,
                            lastValueField,
                            onPropertyChangedHandlerField,
                            onPropertyChangedMethod = ctx.OnPropertyChangedMethod
                        } );
                    /*
    [Template]
    private static void UpdateChildProperty( 
        [CompileTime] IExpression accessChildExpression, 
        [CompileTime] IField lastValueField,
        [CompileTime] IField onPropertyChangedHandlerField,
        [CompileTime] IMethod onPropertyChangedMethod ) // UpdateA2B2
                     * 
                     */
                }
            }
        }
    }

    private static void ProcessAutoProperties( BuildAspectContext ctx )
    {
        var target = ctx.Target;
        var typeOfInpc = (INamedType) TypeFactory.GetType( typeof( INotifyPropertyChanged ) );

        var onPropertyChangedMethodHasCallerMemberNameAttribute = ctx.OnPropertyChangedMethod.Parameters[0].Attributes.Any( typeof( CallerMemberNameAttribute ) );

        // PS appears to consider all instance properties regardless of accessibility.
        var autoProperties =
            target.Properties
            .Where( p =>
                !p.IsStatic
                && p.IsAutoPropertyOrField == true
                && !p.Attributes.Any( ctx.Type_IgnoreAutoChangeNotificationAttribute ) )
            .ToList();

        foreach ( var p in autoProperties )
        {
            switch ( p.Type.IsReferenceType )
            {
                case null:
                    throw new NotImplementedException( "Not implemented: unconstrained generic properties" );

                case true:

                    var propertyTypeInstrumentationKind = ctx.GetInpcInstrumentationKind( p.Type );

                    if ( propertyTypeInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit )
                    {
                        /* TODO: Dependency analysis requirements:
                         * 
                         * The current property is a ref type implementing INPC. Other properties of the target class may depend
                         * on direct (value.child) or indirect children (value.child...child) rooted in the value of this property.
                         * 
                         * For the distinct set of parents of all indirect child leaves we must:
                         *   - Introduce an instance of template `UpdateChildProperty` which will need:
                         *     - An expression which accesses the leaf-parent from the backing field of the current property,
                         *       using null conditional access at each level. For example, if there is a dependency on leaf property
                         *       `A2.B2.C1`, then the expression must be `_a2?.B2`; for `A2.B2.C2.D1` the expression would be
                         *       `_a2?.B2?.C2`.
                         *     - A field of the current property type for storing the last value, eg `_lastA2B2` or `_lastA2B2C2`.
                         *     - A field of type PropertyChangedEventHandler? for caching the delegate registered with the 
                         *       leaf-parent's PropertyChanged event.
                         *     - 
                         * 
                         * We must introduce a method which calls each of the introduced `UpdateChildProperty` instances, using the
                         * `UpdateChildren` template. This simply requires a list of `IMethod`. If there is only one `UpdateChildProperty`
                         * instance, it could be treated as the `UpdateChildren` method.
                         * 
                         * 
                         */
                        /*
                         *     private static void UpdateChildProperty( 
                                [CompileTime] IExpression accessChildExpression, 
                                [CompileTime] IField lastValueField,
                                [CompileTime] IField onPropertyChangedHandlerField,
                                [CompileTime] IMethod onPropertyChangedMethod ) // UpdateA2B2
                         */
                        // Do any properties depend on a child of this property?
                        var hasDependentProperties = true;

                        IField? handlerField = null;

                        if ( hasDependentProperties )
                        {
                            var handlerFieldName = GetUnusedMemberName( ctx.Target, $"_on{p.Name}PropertyChangedHandler" );

                            var introduceHandlerFieldResult = ctx.Builder.Advice.IntroduceField(
                                ctx.Target,
                                handlerFieldName,
                                ctx.Type_PropertyChangedEventHandler,
                                whenExists: OverrideStrategy.Fail );

                            handlerField = introduceHandlerFieldResult.Declaration;
                        }

                        ctx.Builder.Advice.Override( p, nameof( OverrideInpcRefTypeProperty ), tags: new
                        {
                            ctx,
                            onPropertyChangedMethodHasCallerMemberNameAttribute,
                            handlerField
                        } );
                    }
                    else
                    {
                        throw new NotImplementedException( "Not implemented: uninstrumented reference-type properties" );
                    }
                    break;

                case false:
                    ctx.Builder.Advice.Override( p, nameof( OverrideValueTypeProperty ), tags: new
                    {
                        ctx,
                        onPropertyChangedMethodHasCallerMemberNameAttribute,
                    } );
                    break;
            }
        }
    }

    private static IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault( m =>
            !m.IsStatic
            && (type.IsSealed || m.Accessibility is Accessibility.Public or Accessibility.Protected)
            && m.ReturnType.SpecialType == SpecialType.Void
            && m.Parameters.Count == 1
            && m.Parameters[0].Type.SpecialType == SpecialType.String
            && _onPropertyChangedMethodNames.Contains( m.Name ) );

    private static string GetUnusedMemberName( INamedType type, string desiredName )
    {
        HashSet<string>? existingMemberNames = null;
        return GetUnusedMemberName( type, desiredName, ref existingMemberNames, false );
    }

    /// <summary>
    /// Gets an unused member name for the given type by adding an numeric suffix until an unused name is found.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="desiredName"></param>
    /// <param name="existingMemberNames">
    /// If not <see langword="null"/> on entry, specifies the known set of member names to consider (the actual member names of <paramref name="type"/>
    /// will be ignored). If <see langword="null"/> on entry, on exit will be set to the member names of <paramref name="type"/> (including the names of nested types),
    /// optionally also including the return value according to <paramref name="addResultToExistingMemberNames"/>.
    /// </param>
    /// <returns></returns>
    private static string GetUnusedMemberName( INamedType type, string desiredName, ref HashSet<string>? existingMemberNames, bool addResultToExistingMemberNames = true )
    {
        string result;

        existingMemberNames ??= new( ((IEnumerable<INamedDeclaration>) type.AllMembers()).Concat( type.NestedTypes ).Select( m => m.Name ) );

        if ( !existingMemberNames.Contains( desiredName ) )
        {
            result = desiredName;
        }
        else
        {
            for ( var i = 2; true; i++ )
            {
                result = $"{desiredName}{i}";

                if ( !existingMemberNames.Contains( result ) )
                {
                    break;
                }
            }
        }

        if ( addResultToExistingMemberNames )
        {
            existingMemberNames.Add( result );
        }

        return result;
    }

    [Template]
    private static void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
    {
        // TODO: Supply a compile-time map of (relevant e.PropertyName) => (list of properties which depend on that e.PropertyName)
        meta.InsertComment( "Not implemented yet. Will a swich on e.PropertyName where cases call OnPropertyChanged for the affected properties." );
    }

    [Template]
    private static dynamic? OverrideInpcRefTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;
            var handlerField = (IField?) meta.Tags["handlerField"];
            var eventRequiresCast = ctx.GetInpcInstrumentationKind( meta.Target.Property.Type ) is InpcInstrumentationKind.Explicit;

            if ( !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ) )
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

                meta.Target.FieldOrProperty.Value = value;


                meta.Target.FieldOrProperty.Value = value;
                ctx.OnPropertyChangedMethod.Invoke( meta.Target.Property.Name );

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

            // This must be implemented as a local function because Metalama does not currently support delegates in any other way.
            void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
            {
                // TODO: Supply a compile-time map of (relevant e.PropertyName) => (list of properties which depend on that e.PropertyName)
                meta.InsertComment( "Not implemented yet. Will a swich on e.PropertyName where cases call OnPropertyChanged for the affected properties." );
            }
        }
    }

    [Template]
    private static dynamic? OverrideNonInpcRefTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;

            if ( !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ) )
            {
                meta.Target.FieldOrProperty.Value = value;
                ctx.OnPropertyChangedMethod.Invoke( meta.Target.Property.Name );
            }
        }
    }

    [Template]
    private static dynamic? OverrideValueTypeProperty
    {
        set
        {
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;

            if ( value != meta.Target.FieldOrProperty.Value )
            {
                meta.Target.FieldOrProperty.Value = value;

                // TODO: IMethod.Invoke is not aware of/does not support default args, so we can't currently emit this idomatic expression.
                //       If/when supported, update other invocations, this is the only commented example.
#if true
                ctx.OnPropertyChangedMethod.Invoke( meta.Target.Property.Name );
#else
                if ( false || (bool)meta.Tags["onPropertyChangedMethodHasCallerMemberNameAttribute"]! )
                {
                    // Emit human idomatic "OnPropertyChanged()" where the property name will be injected by the C# compiler thanks to [CallerMemberName] on
                    // the introduced or existing OnPropertyChanged method.
                    ctx.OnPropertyChangedMethod.Invoke();
                }
                else
                {
                    ctx.OnPropertyChangedMethod.Invoke( meta.Target.Property.Name );
                }
#endif
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
        [CompileTime] IExpression accessChildExpression, 
        [CompileTime] IField lastValueField,
        [CompileTime] IField onPropertyChangedHandlerField,
        [CompileTime] IMethod onPropertyChangedMethod ) // UpdateA2B2
    {
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

            // TODO: Lookup from dependency dictionary for those props affected by change to "A2.B2"
            /*
            var affectedProperties = meta.RunTime( new string[] { "A4" } );

            foreach ( var name in affectedProperties )
            {
                onPropertyChangedMethod.Invoke( name );
            }
            */
        }

        // OnA2B2PropertyChanged
        void OnSpecificPropertyChanged( object? sender, PropertyChangedEventArgs e )
        {
            // TODO: Lookup from dependency dictionary for those props affected by change to "A2.B2" sub-dictionary e.PropertyName
            // Or possibly one static dictionary for the target class's own props, and one static dictionary per child propertypath
            // (eg, one for A2.B2) - otherwise we could get a lot of nested dictionary lookups. Or we do string concat to get the key.
            // Or some fancy heap-free key thingy. Or we could generated a sequence of calls to OnPropertyChanged with the "firm-coded"
            // values.
            /*
            var affectedProperties = meta.RunTime( new string[] { "A4" } );

            foreach ( var name in affectedProperties )
            {
                onPropertyChangedMethod.Invoke( name );
            }
            */
        }
    }
}