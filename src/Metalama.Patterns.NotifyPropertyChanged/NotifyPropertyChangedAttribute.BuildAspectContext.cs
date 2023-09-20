// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Engine.CodeModel;
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

            // TODO: Consider using BaseType.TypeDefinition where possible for better performance.
            
            this.BaseImplementsInpc =
                target.BaseType != null && (
                    target.BaseType.Is( this.Type_INotifyPropertyChanged )
                    || (target.BaseType is { BelongsToCurrentProject: true } 
                        && ( !this.Target.Compilation.IsPartial || this.Target.Compilation.Types.Contains( target.BaseType ) ) 
                        && target.BaseType.TypeDefinition.Enhancements().HasAspect( typeof( NotifyPropertyChangedAttribute ) )));

            this.TargetImplementsInpc = this.BaseImplementsInpc || target.Is( this.Type_INotifyPropertyChanged );

            this._baseOnPropertyChangedMethod = new( () => GetOnPropertyChangedMethod( target ) );
            this._baseOnChildPropertyChangedMethod = new( () => GetOnChildPropertyChangedMethod( target ) );
            this._baseOnUnmonitoredInpcPropertyChangedMethod = new( () => this.GetOnUnmonitoredInpcPropertyChangedMethod( target ) );
        }

        public bool InsertDiagnosticComments { get; set; } // = true; // TODO: Set by configuration? Discuss.

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
            var hasReportedDiagnosticError = false;

            var graph = Implementation.DependencyGraph.GetDependencyGraph<NodeData>(
                this.Target,
                ( diagnostic, location ) =>
                {
                    hasReportedDiagnosticError |= diagnostic.Definition.Severity == Framework.Diagnostics.Severity.Error;
                    this.Builder.Diagnostics.Report( diagnostic, new LocationWrapper( location ) );
                } );
            
            foreach ( var node in graph.DecendantsDepthFirst() )
            {
                node.Data.Initialize( this, node );

                var baseHandling = this.DetermineInpcBaseHandling( node );
                node.Data.Initialize2( baseHandling );
            }

            if ( hasReportedDiagnosticError )
            {
                throw new DiagnosticErrorReportedException();
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
                        return node.FieldOrProperty.DeclaringType == this.Target
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
                        return this.HasInheritedOnChildPropertyChangedPropertyPath( node.DottedPropertyPath )
                            ? InpcBaseHandling.OnChildPropertyChanged
                            : this.HasInheritedOnUnmonitoredInpcPropertyChangedProperty( node.DottedPropertyPath )
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
                var lastValueFieldName = this.GetAndReserveUnusedMemberName( $"_last{node.ContiguousPropertyPath}" );

                var introduceLastValueFieldResult = this.Builder.Advice.IntroduceField(
                    this.Target,
                    lastValueFieldName,
                    node.FieldOrProperty.Type.ToNullableType(),
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
                var handlerFieldName = this.GetAndReserveUnusedMemberName( $"_on{node.ContiguousPropertyPath}PropertyChangedHandler" );

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

                            return namedType.TypeDefinition.Enhancements().HasAspect<NotifyPropertyChangedAttribute>()
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

        private static HashSet<string> BuildPropertyPathLookup( IEnumerable<string>? propertyPaths ) 
            => propertyPaths == null ? new() : new( propertyPaths );

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
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters.Count == 1
                && m.Parameters[0].Type.SpecialType == SpecialType.String
                && _onPropertyChangedMethodNames.Contains( m.Name ) );

        private static IMethod? GetOnChildPropertyChangedMethod( INamedType type )
            => type.AllMethods.FirstOrDefault( m =>
                !m.IsStatic
                && m.Attributes.Any( typeof( OnChildPropertyChangedMethodAttribute ) )
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters.Count == 2
                && m.Parameters[0].Type.SpecialType == SpecialType.String
                && m.Parameters[1].Type.SpecialType == SpecialType.String );

        private IMethod? GetOnUnmonitoredInpcPropertyChangedMethod( INamedType type )
            => type.AllMethods.FirstOrDefault( m =>
                !m.IsStatic
                && m.Attributes.Any( typeof( OnUnmonitoredInpcPropertyChangedMethodAttribute ) )
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters.Count == 3
                && m.Parameters[0].Type.SpecialType == SpecialType.String
                && m.Parameters[1].Type == this.Type_Nullable_INotifyPropertyChanged
                && m.Parameters[2].Type == this.Type_Nullable_INotifyPropertyChanged );
    }
}