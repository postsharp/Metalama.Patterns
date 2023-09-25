// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

[CompileTime]
internal sealed class BuildAspectContext
{
    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    private readonly Dictionary<IType, InpcInstrumentationKind> _inpcInstrumentationKindLookup = new();
    private readonly Lazy<IMethod?> _baseOnPropertyChangedMethod;
    private readonly Lazy<IMethod?> _baseOnChildPropertyChangedMethod;
    private readonly Lazy<IMethod?> _baseOnUnmonitoredInpcPropertyChangedMethod;

    private HashSet<string>? _inheritedOnChildPropertyChangedPropertyPaths;
    private HashSet<string>? _inheritedOnUnmonitoredInpcPropertyChangedPropertyNames;

    internal sealed class ElementsRecord
    {
        public ElementsRecord()
        {
            this.INotifyPropertyChanged = (INamedType) TypeFactory.GetType( typeof(INotifyPropertyChanged) );
            this.PropertyChangedEventOfINotifyPropertyChanged = this.INotifyPropertyChanged.Events.First();
            this.NullableINotifyPropertyChanged = this.INotifyPropertyChanged.ToNullableType();
            this.PropertyChangedEventHandler = (INamedType) TypeFactory.GetType( typeof(PropertyChangedEventHandler) );
            this.NullablePropertyChangedEventHandler = this.PropertyChangedEventHandler.ToNullableType();
            this.IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof(IgnoreAutoChangeNotificationAttribute) );
            this.EqualityComparerOfT = (INamedType) TypeFactory.GetType( typeof(EqualityComparer<>) );
            this.OnChildPropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof(OnChildPropertyChangedMethodAttribute) );
            this.OnUnmonitoredInpcPropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof(OnUnmonitoredInpcPropertyChangedMethodAttribute) );
        }

        // ReSharper disable once InconsistentNaming
        public INamedType INotifyPropertyChanged { get; }

        public INamedType NullableINotifyPropertyChanged { get; }

        public IEvent PropertyChangedEventOfINotifyPropertyChanged { get; }

        private INamedType PropertyChangedEventHandler { get; }

        public INamedType NullablePropertyChangedEventHandler { get; }

        public INamedType IgnoreAutoChangeNotificationAttribute { get; }

        public INamedType EqualityComparerOfT { get; }

        public INamedType OnChildPropertyChangedMethodAttribute { get; }

        public INamedType OnUnmonitoredInpcPropertyChangedMethodAttribute { get; }
    }

    /// <summary>
    /// Gets frequently used compilation elements resolved for the current compilation.
    /// </summary>
    public ElementsRecord Elements { get; }

    public BuildAspectContext( IAspectBuilder<INamedType> builder )
    {
        this.Elements = new ElementsRecord();
        this.Builder = builder;

        var target = builder.Target;

        // TODO: Consider using BaseType.TypeDefinition where possible for better performance.

        this.BaseImplementsInpc =
            target.BaseType != null && (
                target.BaseType.Is( this.Elements.INotifyPropertyChanged )
                || (target.BaseType is { BelongsToCurrentProject: true }
                    && (!this.Target.Compilation.IsPartial || this.Target.Compilation.Types.Contains( target.BaseType ))
                    && target.BaseType.TypeDefinition.Enhancements().HasAspect( typeof(NotifyPropertyChangedAttribute) )));

        this.TargetImplementsInpc = this.BaseImplementsInpc || target.Is( this.Elements.INotifyPropertyChanged );

        this._baseOnPropertyChangedMethod = new Lazy<IMethod?>( () => GetOnPropertyChangedMethod( target ) );
        this._baseOnChildPropertyChangedMethod = new Lazy<IMethod?>( () => GetOnChildPropertyChangedMethod( target ) );
        this._baseOnUnmonitoredInpcPropertyChangedMethod = new Lazy<IMethod?>( () => this.GetOnUnmonitoredInpcPropertyChangedMethod( target ) );
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool InsertDiagnosticComments { get; set; } // = true; // TODO: Set by configuration? Discuss.

    public IAspectBuilder<INamedType> Builder { get; }

    public INamedType Target => this.Builder.Target;

    public bool TargetImplementsInpc { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool BaseImplementsInpc { get; }

    public IMethod? BaseOnPropertyChangedMethod => this._baseOnPropertyChangedMethod.Value;

    public IMethod? BaseOnChildPropertyChangedMethod => this._baseOnChildPropertyChangedMethod.Value;

    public IMethod? BaseOnUnmonitoredInpcPropertyChangedMethod => this._baseOnUnmonitoredInpcPropertyChangedMethod.Value;

    public bool HasInheritedOnChildPropertyChangedPropertyPath( string parentPropertyPath )
    {
        this._inheritedOnChildPropertyChangedPropertyPaths ??=
            BuildPropertyPathLookup( GetPropertyPaths( this.Elements.OnChildPropertyChangedMethodAttribute, this.BaseOnChildPropertyChangedMethod ) );

        return this._inheritedOnChildPropertyChangedPropertyPaths.Contains( parentPropertyPath );
    }

    public bool HasInheritedOnUnmonitoredInpcPropertyChangedProperty( string propertyName )
    {
        this._inheritedOnUnmonitoredInpcPropertyChangedPropertyNames ??=
            BuildPropertyPathLookup(
                GetPropertyPaths( this.Elements.OnUnmonitoredInpcPropertyChangedMethodAttribute, this.BaseOnUnmonitoredInpcPropertyChangedMethod ) );

        return this._inheritedOnUnmonitoredInpcPropertyChangedPropertyNames.Contains( propertyName );
    }

    public CertainDeferredDeclaration<IMethod> OnPropertyChangedMethod { get; } = new();

    public CertainDeferredDeclaration<IMethod> OnChildPropertyChangedMethod { get; } = new();

    public DeferredDeclaration<IMethod> OnUnmonitoredInpcPropertyChangedMethod { get; } =
        new( willBeDefined: true ); // TODO: Decide according to configuration.

    public List<string> PropertyPathsForOnChildPropertyChangedMethodAttribute { get; } = new();

    public List<string> PropertyNamesForOnUnmonitoredInpcPropertyChangedMethodAttribute { get; } = new();

    /// <summary>
    /// Gets the <see cref="IProperty"/> for property <c>EqualityComparer<paramref name="type"/>>.Default</c>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IProperty GetDefaultEqualityComparerForType( IType type )
        => this.Elements.EqualityComparerOfT.WithTypeArguments( type ).Properties.Single( p => p.Name == "Default" );

    private DependencyGraphNode? _dependencyGraph;

    private DependencyGraphNode PrepareDependencyGraph()
    {
        var hasReportedDiagnosticError = false;

        var graph = Implementation.DependencyGraph.GetDependencyGraph<DependencyGraphNode>(
            this.Target,
            ( diagnostic, location ) =>
            {
                hasReportedDiagnosticError |= diagnostic.Definition.Severity == Severity.Error;
                this.Builder.Diagnostics.Report( diagnostic, new LocationWrapper( location ) );
            } );

        foreach ( var node in graph.DescendantsDepthFirst() )
        {
            node.Initialize( this );
        }

        if ( hasReportedDiagnosticError )
        {
            throw new DiagnosticErrorReportedException();
        }

        return graph;
    }

    public DependencyGraphNode DependencyGraph => this._dependencyGraph ??= this.PrepareDependencyGraph();

    public IField GetOrCreateLastValueField( DependencyGraphNode node )
    {
        if ( node.LastValueField == null )
        {
            var lastValueFieldName = this.GetAndReserveUnusedMemberName( $"_last{node.ContiguousPropertyPath}" );

            var introduceLastValueFieldResult = this.Builder.Advice.IntroduceField(
                this.Target,
                lastValueFieldName,
                node.FieldOrProperty.Type.ToNullableType(),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b => b.Accessibility = Accessibility.Private );

            node.SetLastValueField( introduceLastValueFieldResult.Declaration );
        }

        return node.LastValueField!;
    }

    public IField GetOrCreateHandlerField( DependencyGraphNode node )
    {
        if ( node.HandlerField == null )
        {
            var handlerFieldName = this.GetAndReserveUnusedMemberName( $"_on{node.ContiguousPropertyPath}PropertyChangedHandler" );

            var introduceHandlerFieldResult = this.Builder.Advice.IntroduceField(
                this.Target,
                handlerFieldName,
                this.Elements.NullablePropertyChangedEventHandler,
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b => b.Accessibility = Accessibility.Private );

            node.SetHandlerField( introduceHandlerFieldResult.Declaration );
        }

        return node.HandlerField!;
    }

    public IMethod GetOrCreateRootPropertySubscribeMethod( DependencyGraphNode node )
    {
        if ( node.Depth != 1 )
        {
            throw new ArgumentException( "Must be a root property node (depth must be 1).", nameof(node) );
        }

        if ( !node.SubscribeMethod.DeclarationIsSet )
        {
            var handlerField = this.GetOrCreateHandlerField( node );

            var subscribeMethodName = this.GetAndReserveUnusedMemberName( $"Subscribe{node.Name}" );

            var result = this.Builder.Advice.IntroduceMethod(
                this.Target,
                nameof(NaturalAspect.Subscribe),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b =>
                {
                    b.Name = subscribeMethodName;
                    b.Accessibility = Accessibility.Private;
                },
                args: new { TValue = node.FieldOrProperty.Type, ctx = this, node, handlerField } );

            node.SubscribeMethod.Declaration = result.Declaration;
        }

        return node.SubscribeMethod.Declaration;
    }

    private HashSet<string>? _existingMemberNames;

    /// <summary>
    /// Gets an unused member name for the target type by adding an numeric suffix until an unused name is found.
    /// </summary>
    /// <param name="desiredName"></param>
    /// <returns></returns>
    public string GetAndReserveUnusedMemberName( string desiredName )
    {
        this._existingMemberNames ??= new HashSet<string>(
            ((IEnumerable<INamedDeclaration>) this.Target.AllMembers()).Concat( this.Target.NestedTypes ).Select( m => m.Name ) );

        if ( this._existingMemberNames.Add( desiredName ) )
        {
            return desiredName;
        }
        else
        {
            // ReSharper disable once BadSemicolonSpaces
            for ( var i = 1; ; i++ )
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

        InpcInstrumentationKind Check( IType type2 )
        {
            switch ( type2 )
            {
                case INamedType namedType:
                    if ( namedType.SpecialType != SpecialType.None )
                    {
                        // None of the special types implement INPC.
                        return InpcInstrumentationKind.None;
                    }
                    else if ( namedType.Equals( this.Elements.INotifyPropertyChanged ) )
                    {
                        return InpcInstrumentationKind.Implicit;
                    }
                    else if ( namedType.Is( this.Elements.INotifyPropertyChanged ) )
                    {
                        if ( namedType.TryFindImplementationForInterfaceMember( this.Elements.PropertyChangedEventOfINotifyPropertyChanged, out var member ) )
                        {
                            return member.IsExplicitInterfaceImplementation ? InpcInstrumentationKind.Explicit : InpcInstrumentationKind.Implicit;
                        }

                        throw new InvalidOperationException( "Could not find implementation of interface member." );
                    }
                    else if ( !namedType.BelongsToCurrentProject )
                    {
                        return InpcInstrumentationKind.None;
                    }
                    else
                    {
                        if ( this.Target.Compilation.IsPartial && !this.Target.Compilation.Types.Contains( type2 ) )
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
        => propertyPaths == null ? new HashSet<string>() : new HashSet<string>( propertyPaths );

    [return: NotNullIfNotNull( nameof(method) )]
    private static IEnumerable<string>? GetPropertyPaths( INamedType attributeType, IMethod? method, bool includeInherited = true )
    {
        // NB: Assumes that attributeType instances will always be constructed with one arg of type string[].

        if ( method == null )
        {
            return null;
        }

        return includeInherited
            ? EnumerableExtensions.SelectRecursive( method, m => m.OverriddenMethod ).SelectMany( m => GetPropertyPathsCore( attributeType, m ) )
            : GetPropertyPathsCore( attributeType, method );

        static IEnumerable<string> GetPropertyPathsCore( INamedType attributeType, IMethod method )
            => method.Attributes
                .OfAttributeType( attributeType )
                .SelectMany( a => a.ConstructorArguments[0].Values.Select( k => (string?) k.Value ) )
                .Where( s => !string.IsNullOrWhiteSpace( s ) )!;
    }

    private static IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }]
                && _onPropertyChangedMethodNames.Contains( m.Name ) );

    private static IMethod? GetOnChildPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && m.Attributes.Any( typeof(OnChildPropertyChangedMethodAttribute) )
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }, { Type.SpecialType: SpecialType.String }] );

    private IMethod? GetOnUnmonitoredInpcPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && m.Attributes.Any( typeof(OnUnmonitoredInpcPropertyChangedMethodAttribute) )
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }, _, _]
                && m.Parameters[1].Type == this.Elements.NullableINotifyPropertyChanged
                && m.Parameters[2].Type == this.Elements.NullableINotifyPropertyChanged );
}