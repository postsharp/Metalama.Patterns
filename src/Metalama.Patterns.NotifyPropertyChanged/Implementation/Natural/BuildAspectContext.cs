// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Engine.Diagnostics;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

[CompileTime]
internal sealed partial class BuildAspectContext
{
    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    private readonly Dictionary<IFieldOrProperty, bool> _validateFieldOrPropertyLookup = new();
    private readonly Lazy<IMethod?> _baseOnPropertyChangedMethod;
    private readonly Lazy<IMethod?> _baseOnChildPropertyChangedMethod;
    private readonly Lazy<IMethod?> _baseOnUnmonitoredObservablePropertyChangedMethod;

    private HashSet<string>? _inheritedOnChildPropertyChangedPropertyPaths;
    private HashSet<string>? _inheritedOnUnmonitoredObservablePropertyChangedPropertyNames;

    /// <summary>
    /// Gets frequently used compilation elements resolved for the current compilation.
    /// </summary>
    public Elements Elements { get; }

    public InpcInstrumentationKindLookup InpcInstrumentationKindLookup { get; }

    public BuildAspectContext( IAspectBuilder<INamedType> builder )
    {
        this.Builder = builder;
        this.Elements = new Elements( builder.Target );
        this.InpcInstrumentationKindLookup = new( this.Elements );

        var target = builder.Target;

        // TODO: Consider using BaseType.Definition where possible for better performance.

        this.BaseImplementsInpc =
            target.BaseType != null && (
                target.BaseType.Is( this.Elements.INotifyPropertyChanged )
                || (target.BaseType is { BelongsToCurrentProject: true }
                    && target.BaseType.Definition.Enhancements().HasAspect( typeof(NotifyPropertyChangedAttribute) )));

        this.TargetImplementsInpc = this.BaseImplementsInpc || target.Is( this.Elements.INotifyPropertyChanged );

        this._baseOnPropertyChangedMethod = new Lazy<IMethod?>( () => GetOnPropertyChangedMethod( target ) );
        this._baseOnChildPropertyChangedMethod = new Lazy<IMethod?>( () => GetOnChildPropertyChangedMethod( target ) );
        this._baseOnUnmonitoredObservablePropertyChangedMethod = new Lazy<IMethod?>( () => this.GetOnUnmonitoredObservablePropertyChangedMethod( target ) );
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

    public IMethod? BaseOnUnmonitoredObservablePropertyChangedMethod => this._baseOnUnmonitoredObservablePropertyChangedMethod.Value;

    public bool HasInheritedOnChildPropertyChangedPropertyPath( string parentPropertyPath )
    {
        this._inheritedOnChildPropertyChangedPropertyPaths ??=
            BuildPropertyPathLookup( GetPropertyPaths( this.Elements.OnChildPropertyChangedMethodAttribute, this.BaseOnChildPropertyChangedMethod ) );

        return this._inheritedOnChildPropertyChangedPropertyPaths.Contains( parentPropertyPath );
    }

    public bool HasInheritedOnUnmonitoredObservablePropertyChangedProperty( string propertyName )
    {
        this._inheritedOnUnmonitoredObservablePropertyChangedPropertyNames ??=
            BuildPropertyPathLookup(
                GetPropertyPaths( this.Elements.OnUnmonitoredObservablePropertyChangedMethodAttribute, this.BaseOnUnmonitoredObservablePropertyChangedMethod ) );

        return this._inheritedOnUnmonitoredObservablePropertyChangedPropertyNames.Contains( propertyName );
    }

    public CertainDeferredDeclaration<IMethod> OnPropertyChangedMethod { get; } = new();

    public CertainDeferredDeclaration<IMethod> OnChildPropertyChangedMethod { get; } = new();

    public DeferredDeclaration<IMethod> OnUnmonitoredObservablePropertyChangedMethod { get; } =
        new( willBeDefined: true ); // TODO: Decide according to configuration.

    public List<string> PropertyPathsForOnChildPropertyChangedMethodAttribute { get; } = new();

    public List<string> PropertyNamesForOnUnmonitoredObservablePropertyChangedMethodAttribute { get; } = new();

    private DependencyGraphNode? _dependencyGraph;
    private bool _prepareDependencyGraphReportedErrors;

    public DependencyGraphNode PrepareDependencyGraph()
    {
        var graph = Implementation.DependencyGraph.GetDependencyGraph<DependencyGraphNode>(
            this.Target,
            ( diagnostic, location ) =>
            {
                this._prepareDependencyGraphReportedErrors |= diagnostic.Definition.Severity == Severity.Error;
                this.Builder.Diagnostics.Report( diagnostic, location.ToDiagnosticLocation() );
            } );

        foreach ( var node in graph.DescendantsDepthFirst() )
        {
            this._prepareDependencyGraphReportedErrors |= !node.Initialize( this );
        }

        return graph;
    }

    public DependencyGraphNode DependencyGraph => this._dependencyGraph ??= this.PrepareDependencyGraph();

    public bool PrepareDependencyGraphReportedErrors
    {
        get
        {
            _ = this.DependencyGraph;

            return this._prepareDependencyGraphReportedErrors;
        }
    }

    public IField GetOrCreateLastValueField( DependencyGraphNode node )
    {
        if ( node.LastValueField == null )
        {
            var lastValueFieldName = this.GetAndReserveUnusedMemberName( $"_last{node.ContiguousPropertyPathWithoutDot}" );

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
            var handlerFieldName = this.GetAndReserveUnusedMemberName( $"_on{node.ContiguousPropertyPathWithoutDot}PropertyChangedHandler" );

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

            var subscribeMethodName = this.GetAndReserveUnusedMemberName( $"SubscribeTo{node.Name}" );

            var result = this.Builder.Advice.IntroduceMethod(
                this.Target,
                nameof(ClassicImplementationStrategyBuilder.Subscribe),
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

    private IMethod? GetOnUnmonitoredObservablePropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && m.Attributes.Any( typeof(OnUnmonitoredObservablePropertyChangedMethodAttribute) )
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }, _, _]
                && m.Parameters[1].Type == this.Elements.NullableINotifyPropertyChanged
                && m.Parameters[2].Type == this.Elements.NullableINotifyPropertyChanged );

    /// <summary>
    /// Validates the given <see cref="IFieldOrProperty"/>, reporting diagnostics if applicable. The result is cached
    /// so that diagnostics are not repeated.
    /// </summary>
    /// <returns><see langword="true"/> if valid, or <see langword="false"/> if invalid.</returns>
    public bool ValidateFieldOrProperty( IFieldOrProperty fieldOrProperty )
    {
        if ( !this._validateFieldOrPropertyLookup.TryGetValue( fieldOrProperty, out var result ) )
        {
            result = IsValid( fieldOrProperty );
            this._validateFieldOrPropertyLookup[fieldOrProperty] = result;
        }

        return result;

        bool IsValid( IFieldOrProperty fp )
        {
            var typeImplementsInpc =
                this.InpcInstrumentationKindLookup.Get( fp.Type ) is InpcInstrumentationKind.Explicit or InpcInstrumentationKind.Implicit;

            var isValid = true;

            switch ( fp.Type.IsReferenceType )
            {
                case null:
                    // This might require INPC-type code which is used at runtime only when T implements INPC,
                    // and non-INPC-type code which is used at runtime when T does not implement INPC.

                    this.Builder.Diagnostics.Report(
                        DiagnosticDescriptors.NotifyPropertyChanged.ErrorFieldOrPropertyTypeIsUnconstrainedGeneric.WithArguments(
                            (fp.DeclarationKind, fp, fp.Type) ),
                        fp );

                    isValid = false;

                    break;

                case false:

                    if ( typeImplementsInpc )
                    {
                        this.Builder.Diagnostics.Report(
                            DiagnosticDescriptors.NotifyPropertyChanged.ErrorFieldOrPropertyTypeIsStructImplementingInpc.WithArguments(
                                (fp.DeclarationKind, fp, fp.Type) ),
                            fp );

                        isValid = false;
                    }

                    break;
            }

            if ( fp.IsVirtual )
            {
                this.Builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorVirtualMemberIsNotSupported.WithArguments( (fp.DeclarationKind, fp) ),
                    fp );

                isValid = false;
            }

            if ( fp.IsNew )
            {
                this.Builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorNewMemberIsNotSupported.WithArguments( (fp.DeclarationKind, fp) ),
                    fp );

                isValid = false;
            }

            return isValid;
        }
    }
}