using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
public sealed partial class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
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
        Explicit,
        /// <summary>
        /// Returned at design time for types other than the current type and its ancestors.
        /// </summary>
        Unknown
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
            this.Type_Nullable_PropertyChangedEventHandler = this.Type_PropertyChangedEventHandler.ToNullableType();
            this.Type_IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof( IgnoreAutoChangeNotificationAttribute ) );
        }

        public IAspectBuilder<INamedType> Builder { get; }

        public INamedType Target => this.Builder.Target;

        public INamedType Type_INotifyPropertyChanged { get; }

        public IEvent Event_INotifyPropertyChanged_PropertyChanged { get; }

        public INamedType Type_PropertyChangedEventHandler { get; }

        public INamedType Type_Nullable_PropertyChangedEventHandler { get; }

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
                        if ( namedType.SpecialType != SpecialType.None )
                        {
                            // None of the special types implement INPC.
                            return InpcInstrumentationKind.None;
                        }
                        else if ( namedType.Equals( this.Type_INotifyPropertyChanged ) )
                        {
                            return InpcInstrumentationKind.Implicit;
                        }
                        else if ( namedType.Is( this.Type_INotifyPropertyChanged ) )
                        {
                            if ( namedType.TryFindImplementationForInterfaceMember( this.Event_INotifyPropertyChanged_PropertyChanged, out var result ) )
                            {
                                return result.IsExplicitInterfaceImplementation ? InpcInstrumentationKind.Explicit : InpcInstrumentationKind.Implicit;
                            }

                            throw new InvalidOperationException( "Could not find implementation of interface member." );
                        }
                        else if ( !namedType.BelongsToCurrentProject )
                        {
                            return InpcInstrumentationKind.None;
                        }
                        else 
                        {
                            if ( this.Target.Compilation.IsPartial && !this.Target.Compilation.Types.Contains( type ) )
                            {
                                return InpcInstrumentationKind.Unknown;
                            }

                            return namedType.Enhancements().HasAspect<NotifyPropertyChangedAttribute>()
                                ? InpcInstrumentationKind.Implicit // For now, the aspect always introduces implicit implementation.
                                : InpcInstrumentationKind.None;
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
        ProcessAutoProperties( ctx );
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

        /* Iterate all nodes (except root), depth-first, in leaf-to-root order (this is important).
         * For each node that is directly referenced, we then make sure to build the
         * update method for the *parent* - this is because the parent object will
         * notify changes to the child that we are considering.
         */

        // TODO: I suspect/know some scenarios are not handled yet. Don't assume the logic here is correct or complete.

        foreach ( var node in allNodesDepthFirst )
        {
            var parent = node.Parent!;

            if ( parent.IsRoot )
            {
                // *node* is a root property of the target type. Do we need to do anything for it?
                continue;
            }

            if ( parent.Parent!.IsRoot )
            {
                // *parent* is a root property of the target type. Do we need to do anything for it?
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
                    parent.Data.FieldOrProperty.Type.ToNullableType(),
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
                    ctx.Type_Nullable_PropertyChangedEventHandler,
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

                var introduceUpdateChildPropertyMethodResult = ctx.Builder.Advice.IntroduceMethod(
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
                        ctx,
                        node = parent,
                        accessChildExpression,
                        lastValueField,
                        onPropertyChangedHandlerField,
                    } );

                parent.Data.UpdateMethod = introduceUpdateChildPropertyMethodResult.Declaration;
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
            var node = ctx.DependencyGraph.GetChild( p.GetSymbol() );
            var propertyTypeInstrumentationKind = ctx.GetInpcInstrumentationKind( p.Type );

            switch ( p.Type.IsReferenceType )
            {
                case null:
                    // This might require INPC-type code which is used at runtime only when T implements INPC,
                    // and non-INPC-type code which is used at runtime when T does not implement INPC.                    
                    throw new NotSupportedException( "Unconstrained generic properties are not supported." );

                case true:

                    if ( propertyTypeInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit )
                    {
                        var hasDependentProperties = node != null;

                        IField? handlerField = null;

                        if ( hasDependentProperties )
                        {
                            var handlerFieldName = GetUnusedMemberName( ctx.Target, $"_on{p.Name}PropertyChangedHandler" );

                            var introduceHandlerFieldResult = ctx.Builder.Advice.IntroduceField(
                                ctx.Target,
                                handlerFieldName,
                                ctx.Type_Nullable_PropertyChangedEventHandler,
                                whenExists: OverrideStrategy.Fail );

                            handlerField = introduceHandlerFieldResult.Declaration;
                        }

                        ctx.Builder.Advice.Override( p, nameof( OverrideInpcRefTypeProperty ), tags: new
                        {
                            ctx,
                            onPropertyChangedMethodHasCallerMemberNameAttribute,
                            handlerField,
                            node
                        } );
                    }
                    else
                    {
                        ctx.Builder.Advice.Override( p, nameof( OverrideUninstrumentedTypeProperty ), tags: new
                        {
                            ctx,
                            node,
                            compareUsing = EqualityComparisonKind.ReferenceEquals,
                            propertyTypeInstrumentationKind
                        } );
                    }
                    break;

                case false:

                    if ( propertyTypeInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit )
                    {
                        // TODO: Proper error reporting.
                        throw new NotSupportedException( "structs implementing INotifyPropertyChanged are not supported." );
                    }

                    ctx.Builder.Advice.Override( p, nameof( OverrideUninstrumentedTypeProperty ), tags: new
                    {
                        ctx,
                        node,
                        compareUsing = EqualityComparisonKind.EqualityOperator,
                        propertyTypeInstrumentationKind
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
}