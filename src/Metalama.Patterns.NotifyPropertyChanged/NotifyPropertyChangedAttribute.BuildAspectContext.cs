﻿using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged;

public sealed partial class NotifyPropertyChangedAttribute
{
    [CompileTime]
    private sealed class BuildAspectContext
    {
        private readonly Dictionary<IType, InpcInstrumentationKind> _inpcInstrumentationKindLookup = new();
        private readonly Lazy<IMethod?> _baseOnPropertyChangedMethod;
        private readonly Lazy<IMethod?> _baseOnChildPropertyChangedMethod;
        private readonly Lazy<IMethod?> _baseOnUnmonitoredInpcPropertyChangedMethod;

        private HashSet<string>? _inheritedOnChildPropertyChangedPropertyPaths;
        private HashSet<string>? _inheritedOnUnmonitoredInpcPropertyChangedPropertyNames;

        public BuildAspectContext( IAspectBuilder<INamedType> builder )
        {
            this.Builder = builder;
            this.Type_INotifyPropertyChanged = (INamedType) TypeFactory.GetType( typeof( INotifyPropertyChanged ) );
            this.Event_INotifyPropertyChanged_PropertyChanged = this.Type_INotifyPropertyChanged.Events.First();
            this.Type_Nullable_INotifyPropertyChanged = this.Type_INotifyPropertyChanged.ToNullableType();
            this.Type_PropertyChangedEventHandler = (INamedType) TypeFactory.GetType( typeof( PropertyChangedEventHandler ) );
            this.Type_Nullable_PropertyChangedEventHandler = this.Type_PropertyChangedEventHandler.ToNullableType();
            this.Type_IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof( IgnoreAutoChangeNotificationAttribute ) );
            this.Type_EqualityComparerOfT = (INamedType) TypeFactory.GetType( typeof( EqualityComparer<> ) );
            this.Type_OnChildPropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof( OnChildPropertyChangedMethodAttribute ) );
            this.Type_OnUnmonitoredInpcPropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof( OnUnmonitoredInpcPropertyChangedMethodAttribute ) );

            var target = builder.Target;

            // TODO: Remove workaround pending fix for Enhancements() not including inherited aspects.

#if false // Original
            this.BaseImplementsInpc =
                target.BaseType != null && (
                    target.BaseType.Is( this.Type_INotifyPropertyChanged )
                    || (target.BaseType is { BelongsToCurrentProject: true } && target.BaseType.Enhancements().HasAspect( typeof( NotifyPropertyChangedAttribute ) )));
#else // Workaround

            static bool AnyAncestorOrSelfInCompilationHasAspect( INamedType? type, Type aspectType )
            {
                while ( type != null )
                {
                    if ( type.BelongsToCurrentProject && type.Enhancements().HasAspect( aspectType ) )
                    {
                        return true;
                    }

                    type = type.BaseType;
                }

                return false;
            }

            this.BaseImplementsInpc =
                target.BaseType != null && (
                    target.BaseType.Is( this.Type_INotifyPropertyChanged )
                    || AnyAncestorOrSelfInCompilationHasAspect( target.BaseType, typeof( NotifyPropertyChangedAttribute ) ));
#endif

            this.TargetImplementsInpc = this.BaseImplementsInpc || target.Is( this.Type_INotifyPropertyChanged );

            this._baseOnPropertyChangedMethod = new( () => GetOnPropertyChangedMethod( target ) );
            this._baseOnChildPropertyChangedMethod = new( () => GetOnChildPropertyChangedMethod( target ) );
            this._baseOnUnmonitoredInpcPropertyChangedMethod = new( () => this.GetOnUnmonitoredInpcPropertyChangedMethod( target ) );
        }

        public bool InsertDiagnosticComments { get; set; } = false; // TODO: Set by configuration? Discuss.

        public IAspectBuilder<INamedType> Builder { get; }

        public INamedType Target => this.Builder.Target;

        public INamedType Type_INotifyPropertyChanged { get; }

        public INamedType Type_Nullable_INotifyPropertyChanged { get; }

        public IEvent Event_INotifyPropertyChanged_PropertyChanged { get; }

        public INamedType Type_PropertyChangedEventHandler { get; }

        public INamedType Type_Nullable_PropertyChangedEventHandler { get; }

        public INamedType Type_IgnoreAutoChangeNotificationAttribute { get; }

        public INamedType Type_EqualityComparerOfT { get; }

        public INamedType Type_OnChildPropertyChangedMethodAttribute { get; }

        public INamedType Type_OnUnmonitoredInpcPropertyChangedMethodAttribute { get; }

        public bool TargetImplementsInpc { get; }

        public bool BaseImplementsInpc { get; }

        public IMethod? BaseOnPropertyChangedMethod => this._baseOnPropertyChangedMethod.Value;

        public IMethod? BaseOnChildPropertyChangedMethod => this._baseOnChildPropertyChangedMethod.Value;

        public IMethod? BaseOnUnmonitoredInpcPropertyChangedMethod => this._baseOnUnmonitoredInpcPropertyChangedMethod.Value;

        public bool HasInheritedOnChildPropertyChangedPropertyPath( string parentPropertyPath )
        {
            this._inheritedOnChildPropertyChangedPropertyPaths ??=
                BuildPropertyPathLookup( GetPropertyPaths( this.Type_OnChildPropertyChangedMethodAttribute, this.BaseOnChildPropertyChangedMethod ) );

            return this._inheritedOnChildPropertyChangedPropertyPaths.Contains( parentPropertyPath );
        }

        public bool HasInheritedOnUnmonitoredInpcPropertyChangedProperty( string propertyName )
        {
            this._inheritedOnUnmonitoredInpcPropertyChangedPropertyNames ??=
                /*BuildPropertyNameLookup*/ // TODO: Decide, clean up.
                BuildPropertyPathLookup( GetPropertyPaths( this.Type_OnUnmonitoredInpcPropertyChangedMethodAttribute, this.BaseOnUnmonitoredInpcPropertyChangedMethod ) );

            return this._inheritedOnUnmonitoredInpcPropertyChangedPropertyNames.Contains( propertyName );
        }

        public CertainDeferredDeclaration<IMethod> OnPropertyChangedMethod { get; } = new();

        public CertainDeferredDeclaration<IMethod> OnChildPropertyChangedMethod { get; } = new();

        public DeferredDeclaration<IMethod> OnUnmonitoredInpcPropertyChangedMethod { get; } = new( willBeDefined: true ); // TODO: Decide according to configuration.

        public List<string> PropertyPathsForOnChildPropertyChangedMethodAttribute { get; } = new();

        public List<string> PropertyNamesForOnUnmonitoredInpcPropertyChangedMethodAttribute { get; } = new();

        /// <summary>
        /// Gets the <see cref="IProperty"/> for property <c>EqualityComparer<paramref name="type"/>>.Default</c>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IProperty GetDefaultEqualityComparerForType( IType type )
            => this.Type_EqualityComparerOfT.WithTypeArguments( type ).Properties.Single( p => p.Name == "Default" );

        private DependencyGraph.Node<NodeData>? _dependencyGraph;

        private DependencyGraph.Node<NodeData> PrepareDependencyGraph()
        {
            var graph = Implementation.DependencyGraph.GetDependencyGraph<NodeData>( this.Target );
            foreach ( var node in graph.DecendantsDepthFirst() )
            {
                node.Data.Initialize( this, node );

                var baseHandling = this.DetermineInpcBaseHandling( node );
                node.Data.Initialize2( baseHandling );
            }

            return graph;
        }

        private InpcBaseHandling DetermineInpcBaseHandling( DependencyGraph.Node<NodeData> node )
        {
            switch ( node.Data.PropertyTypeInpcInstrumentationKind )
            {
                case InpcInstrumentationKind.Unknown:
                    return InpcBaseHandling.Unknown;

                case InpcInstrumentationKind.None:
                    return InpcBaseHandling.NA;

                case InpcInstrumentationKind.Implicit:
                case InpcInstrumentationKind.Explicit:
                    if ( node.Depth == 1 )
                    {
                        // Root property
                        return node.Data.FieldOrProperty.DeclaringType == this.Target
                            ? InpcBaseHandling.NA
                            : this.HasInheritedOnChildPropertyChangedPropertyPath( node.Name )
                                ? InpcBaseHandling.OnChildPropertyChanged
                                : this.HasInheritedOnUnmonitoredInpcPropertyChangedProperty( node.Name )
                                    ? InpcBaseHandling.OnUnmonitoredInpcPropertyChanged
                                    : InpcBaseHandling.OnPropertyChanged;
                    }
                    else
                    {
                        // Child property
                        return this.HasInheritedOnChildPropertyChangedPropertyPath( node.Data.DottedPropertyPath )
                            ? InpcBaseHandling.OnChildPropertyChanged
                            : this.HasInheritedOnUnmonitoredInpcPropertyChangedProperty( node.Data.DottedPropertyPath )
                                ? InpcBaseHandling.OnUnmonitoredInpcPropertyChanged
                                : InpcBaseHandling.None;
                    }

                default:
                    throw new NotSupportedException();
            }
        }

        public DependencyGraph.Node<NodeData> DependencyGraph => this._dependencyGraph ??= this.PrepareDependencyGraph();

        public IField GetOrCreateLastValueField( DependencyGraph.Node<NodeData> node )
        {
            if ( node.Data.LastValueField == null )
            {
                var lastValueFieldName = this.GetAndReserveUnusedMemberName( $"_last{node.Data.ContiguousPropertyPath}" );

                var introduceLastValueFieldResult = this.Builder.Advice.IntroduceField(
                    this.Target,
                    lastValueFieldName,
                    node.Data.FieldOrProperty.Type.ToNullableType(),
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b => b.Accessibility = Accessibility.Private );

                node.Data.SetLastValueField( introduceLastValueFieldResult.Declaration );
            }

            return node.Data.LastValueField!;
        }

        public IField GetOrCreateHandlerField( DependencyGraph.Node<NodeData> node )
        {
            if ( node.Data.HandlerField == null )
            {
                var handlerFieldName = this.GetAndReserveUnusedMemberName( $"_on{node.Data.ContiguousPropertyPath}PropertyChangedHandler" );

                var introduceHandlerFieldResult = this.Builder.Advice.IntroduceField(
                    this.Target,
                    handlerFieldName,
                    this.Type_Nullable_PropertyChangedEventHandler,
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b => b.Accessibility = Accessibility.Private );

                node.Data.SetHandlerField( introduceHandlerFieldResult.Declaration );
            }

            return node.Data.HandlerField!;
        }

        private HashSet<string>? _existingMemberNames;

        /// <summary>
        /// Gets an unused member name for the target type by adding an numeric suffix until an unused name is found.
        /// </summary>
        /// <param name="desiredName"></param>
        /// <returns></returns>
        public string GetAndReserveUnusedMemberName( string desiredName )
        {
            this._existingMemberNames ??= new( ((IEnumerable<INamedDeclaration>) this.Target.AllMembers()).Concat( this.Target.NestedTypes ).Select( m => m.Name ) );

            if ( this._existingMemberNames.Add( desiredName ) )
            {
                return desiredName;
            }
            else
            {
                for ( var i = 1; true; i++ )
                {
                    var adjustedName = $"{desiredName}_{i}";
                    
                    if ( this._existingMemberNames.Add( adjustedName ) )
                    {
                        return adjustedName;
                    }
                }
            }
        }

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

        private static HashSet<string> BuildPropertyNameLookup( IEnumerable<string>? propertyNames )
        {
            var h = new HashSet<string>();

            if ( propertyNames != null )
            {
                foreach ( var s in propertyNames )
                {
                    if ( s.IndexOf( '.' ) > -1 )
                    {
                        throw new ArgumentException( "A property name must not contain period characters.", nameof( propertyNames ) );
                    }
                    h.Add( s );
                }
            }

            return h;
        }

        // NOTE: This hashset of path stems approach is a simple way to allow path stems to be checked, but
        // a tree-based structure might scale better if required. Keeping it simple for now.
        private static HashSet<string> BuildPropertyPathLookup( IEnumerable<string>? propertyPaths )
        {
            var h = new HashSet<string>();

            if ( propertyPaths != null )
            {
                foreach ( var s in propertyPaths )
                {
                    AddPropertyPathAndAllAncestorStems( h, s );
                }
            }

            return h;

            static void AddPropertyPathAndAllAncestorStems( HashSet<string> addTo, string propertyPath )
            {
                addTo.Add( propertyPath );

                var lastIdx = propertyPath.LastIndexOf( '.' );

                while ( lastIdx > 1 )
                {
                    addTo.Add( propertyPath.Substring( lastIdx - 1 ) );
                    lastIdx = propertyPath.LastIndexOf( '.', lastIdx - 1 );
                }
            }
        }

        [return: NotNullIfNotNull( nameof( method ) )]
        private static IEnumerable<string>? GetPropertyPaths( INamedType attributeType, IMethod? method, bool includeInherited = true )
        {
            // NB: Assumes that attributeType instances will always be constructed with one arg of type string[].

            if ( method == null )
            {
                return null;
            }

            return includeInherited
                ? EnumerableExtensions.SelectRecursive( method, m => m.OverriddenMethod ).SelectMany( m => GetPropertyPaths( attributeType, m ) )
                : GetPropertyPaths( attributeType, method );

            static IEnumerable<string> GetPropertyPaths( INamedType attributeType, IMethod method ) =>
                method.Attributes
                .OfAttributeType( attributeType )
                .SelectMany( a => a.ConstructorArguments[0].Values.Select( k => (string?) k.Value ) )
                .Where( s => !string.IsNullOrWhiteSpace( s ) )!;
        }

        private static IMethod? GetOnPropertyChangedMethod( INamedType type )
            => type.AllMethods.FirstOrDefault( m =>
                !m.IsStatic
                && (type.IsSealed || m.Accessibility is Accessibility.Public or Accessibility.Protected)
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters.Count == 1
                && m.Parameters[0].Type.SpecialType == SpecialType.String
                && _onPropertyChangedMethodNames.Contains( m.Name ) );

        private static IMethod? GetOnChildPropertyChangedMethod( INamedType type )
            => type.AllMethods.FirstOrDefault( m =>
                !m.IsStatic
                && m.Attributes.Any( typeof( OnChildPropertyChangedMethodAttribute ) )
                && (type.IsSealed || m.Accessibility is Accessibility.Public or Accessibility.Protected)
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters.Count == 2
                && m.Parameters[0].Type.SpecialType == SpecialType.String
                && m.Parameters[1].Type.SpecialType == SpecialType.String );

        private IMethod? GetOnUnmonitoredInpcPropertyChangedMethod( INamedType type )
            => type.AllMethods.FirstOrDefault( m =>
                !m.IsStatic
                && m.Attributes.Any( typeof( OnUnmonitoredInpcPropertyChangedMethodAttribute ) )
                && (type.IsSealed || m.Accessibility is Accessibility.Public or Accessibility.Protected)
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters.Count == 3
                && m.Parameters[0].Type.SpecialType == SpecialType.String
                && m.Parameters[1].Type == this.Type_Nullable_INotifyPropertyChanged
                && m.Parameters[2].Type == this.Type_Nullable_INotifyPropertyChanged );
    }
}