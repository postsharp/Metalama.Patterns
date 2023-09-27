// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Diagnostics;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Framework.Engine.Diagnostics;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using Metalama.Patterns.NotifyPropertyChanged.Options;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

[CompileTime]
internal interface IImplementationStrategyBuilder : IDisposable
{
    /// <summary>
    /// Build the aspect. This method must be called at most once for a given instance of <see cref="IImplementationStrategyBuilder"/>.
    /// </summary>
    public void BuildAspect();
}

internal sealed partial class ClassicImplementationStrategyBuilder : IImplementationStrategyBuilder
{
    private readonly DeferredYesNo<IMethod> _onUnmonitoredObservablePropertyChangedMethod;

    void IDisposable.Dispose()
    {
        throw new NotImplementedException();
    }

    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    private readonly IAspectBuilder<INamedType> _builder;
    private readonly Deferred<TemplateExecutionContext> _deferredTemplateExecutionContext = new();
    private readonly NotifyPropertyChangedOptions _commonOptions;
    private readonly ClassicImplementationStrategyOptions _classicOptions;
    private readonly Dictionary<IFieldOrProperty, bool> _validateFieldOrPropertyResults = new();
    private readonly IMethod? _baseOnPropertyChangedMethod;
    private readonly IMethod? _baseOnChildPropertyChangedMethod;
    private readonly IMethod? _baseOnUnmonitoredObservablePropertyChangedMethod;
    private readonly Elements _elements;
    private readonly InpcInstrumentationKindLookup _inpcInstrumentationKindLookup;
    private readonly bool _targetImplementsInpc;
    private readonly bool _baseImplementsInpc;
    private readonly Deferred<IMethod> _onPropertyChangedMethod = new();
    private readonly Deferred<IMethod> _onChildPropertyChangedMethod = new();
    private readonly List<string> _propertyPathsForOnChildPropertyChangedMethodAttribute = new();
    private readonly List<string> _propertyNamesForOnUnmonitoredObservablePropertyChangedMethodAttribute = new();

    public ClassicImplementationStrategyBuilder( IAspectBuilder<INamedType> builder )
    {
        this._builder = builder;
        this._elements = new Elements( builder.Target );
        this._inpcInstrumentationKindLookup = new( this._elements );
        this._commonOptions = builder.GetOptions<NotifyPropertyChangedOptions>();
        this._classicOptions = builder.GetOptions<ClassicImplementationStrategyOptions>();

        this._onUnmonitoredObservablePropertyChangedMethod = new( willBeDefined: this._classicOptions.EnableOnUnmonitoredObservablePropertyChangedMethodOrDefault );
        var target = builder.Target;

        // TODO: Consider using BaseType.Definition where possible for better performance.

        this._baseImplementsInpc =
            target.BaseType != null && (
                target.BaseType.Is( this._elements.INotifyPropertyChanged )
                || (target.BaseType is { BelongsToCurrentProject: true }
                    && target.BaseType.Definition.Enhancements().HasAspect( typeof( NotifyPropertyChangedAttribute ) )));

        this._targetImplementsInpc = this._baseImplementsInpc || target.Is( this._elements.INotifyPropertyChanged );
        this._baseOnPropertyChangedMethod = GetOnPropertyChangedMethod( target );
        this._baseOnChildPropertyChangedMethod = GetOnChildPropertyChangedMethod(target);
        this._baseOnUnmonitoredObservablePropertyChangedMethod = this.GetOnUnmonitoredObservablePropertyChangedMethod(target);
    }

    public void BuildAspect()
    {
        // Validate, maximising the coverage of diagnostic reporting.

        var v1 = this.ValidateBaseImplementation();
        var v2 = this.ValidateDependencyAnalysis();
        var v3 = this.ValidateRootAutoProperties();

        // Transform, if valid. By design, aim to minimise diagnostic reporting that only occurs during
        // the transform phase.
        if ( v1 && v2 && v3 )
        {
            this.IntroduceInterfaceIfRequired();

            // Introduce methods like UpdateA1B1()
            this.IntroduceUpdateMethods();

            // Override auto properties
            this.ProcessAutoProperties();

            this.AddPropertyPathsForOnChildPropertyChangedMethodAttribute();

            this.IntroduceOnPropertyChangedMethod();
            this.IntroduceOnChildPropertyChangedMethod();
            this.IntroduceOnUnmonitoredObservablePropertyChanged();

            this._deferredTemplateExecutionContext.Value = new(
                this._commonOptions,
                this._classicOptions,
                this._elements,
                this._inpcInstrumentationKindLookup,
                this.DependencyGraph,
                this._onUnmonitoredObservablePropertyChangedMethod.Value,
                this._onPropertyChangedMethod.Value,
                this._onChildPropertyChangedMethod.Value,
                this._baseOnPropertyChangedMethod,
                this._baseOnChildPropertyChangedMethod,
                this._baseOnUnmonitoredObservablePropertyChangedMethod );
        }
    }

    private bool ValidateDependencyAnalysis()
    {
        return !this.PrepareDependencyGraphReportedErrors;
    }

    private bool ValidateRootAutoProperties()
    {
        // TODO: Add support for fields.

        var relevantProperties =
            this._builder.Target.Properties
                .Where(
                    p =>
                        p is { IsStatic: false, IsAutoPropertyOrField: true }
                        && !p.Attributes.Any( this._elements.IgnoreAutoChangeNotificationAttribute ) );

        var allValid = true;

        foreach ( var p in relevantProperties )
        {
            allValid &= this.ValidateFieldOrProperty( p );
        }

        return allValid;
    }

    private void AddPropertyPathsForOnChildPropertyChangedMethodAttribute()
    {
        // NB: The selection logic here must be kept in sync with the logic in the OnUnmonitoredObservablePropertyChanged template.

        this._propertyPathsForOnChildPropertyChangedMethodAttribute.AddRange(
            this.DependencyGraph.DescendantsDepthFirst()
                .Where(
                    n => n.InpcBaseHandling switch
                    {
                        InpcBaseHandling.OnUnmonitoredObservablePropertyChanged when this._onUnmonitoredObservablePropertyChangedMethod.WillBeDefined => true,
                        InpcBaseHandling.OnPropertyChanged when n.HasChildren => true,
                        _ => false
                    } )
                .Select( n => n.DottedPropertyPath ) );
    }

    private void IntroduceOnPropertyChangedMethod()
    {
        var isOverride = this._baseOnPropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( Templates.Provider ).IntroduceMethod(
            this._builder.Target,
            nameof( Templates.OnPropertyChanged ),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                if ( isOverride )
                {
                    b.Name = this._baseOnPropertyChangedMethod!.Name;
                }

                if ( this._builder.Target.IsSealed )
                {
                    b.Accessibility = isOverride ? this._baseOnPropertyChangedMethod!.Accessibility : Accessibility.Private;
                }
                else
                {
                    b.Accessibility = isOverride ? this._baseOnPropertyChangedMethod!.Accessibility : Accessibility.Protected;
                    b.IsVirtual = !isOverride;
                }
            },
            args: new { 
                deferredExecutionContext = this._deferredTemplateExecutionContext 
            } );

        this._onPropertyChangedMethod.Value = result.Declaration;

        // Ensure that all required fields are generated in advance of template execution.
        // The node selection logic mirrors that of the template's loops and conditions.

        if ( this._onUnmonitoredObservablePropertyChangedMethod.WillBeDefined )
        {
            foreach ( var node in this.DependencyGraph.DescendantsDepthFirst()
                         .Where( n => n.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredObservablePropertyChanged ) )
            {
                _ = this.GetOrCreateHandlerField( node );
            }
        }

        foreach ( var node in this.DependencyGraph.Children
                     .Where( node => node.InpcBaseHandling is InpcBaseHandling.OnPropertyChanged && node.HasChildren ) )
        {
            _ = this.GetOrCreateHandlerField( node );
            _ = this.GetOrCreateLastValueField( node );
        }
    }

    private void IntroduceOnChildPropertyChangedMethod()
    {
        var isOverride = this._baseOnChildPropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( Templates.Provider ).IntroduceMethod(
            this._builder.Target,
            nameof( Templates.OnChildPropertyChanged ),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                b.AddAttribute(
                    AttributeConstruction.Create(
                        this._elements.OnChildPropertyChangedMethodAttribute,
                        new[] { this._propertyPathsForOnChildPropertyChangedMethodAttribute.OrderBy( s => s ).ToArray() } ) );

                if ( isOverride )
                {
                    b.Name = this._baseOnChildPropertyChangedMethod!.Name;
                }

                if ( this._builder.Target.IsSealed )
                {
                    b.Accessibility = isOverride ? this._baseOnChildPropertyChangedMethod!.Accessibility : Accessibility.Private;
                }
                else
                {
                    b.Accessibility = isOverride ? this._baseOnChildPropertyChangedMethod!.Accessibility : Accessibility.Protected;
                    b.IsVirtual = !isOverride;
                }
            },
            args: new {
                deferredExecutionContext = this._deferredTemplateExecutionContext
            } );

        this._onChildPropertyChangedMethod.Value = result.Declaration;
    }

    private void IntroduceOnUnmonitoredObservablePropertyChanged()
    {
        if ( !this._onUnmonitoredObservablePropertyChangedMethod.WillBeDefined )
        {
            return;
        }

        var isOverride = this._baseOnUnmonitoredObservablePropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( Templates.Provider ).IntroduceMethod(
            this._builder.Target,
            nameof( Templates.OnUnmonitoredObservablePropertyChanged ),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                b.AddAttribute(
                    AttributeConstruction.Create(
                        this._elements.OnUnmonitoredObservablePropertyChangedMethodAttribute,
                        new[] { this._propertyNamesForOnUnmonitoredObservablePropertyChangedMethodAttribute.OrderBy( s => s ).ToArray() } ) );

                if ( isOverride )
                {
                    b.Name = this._baseOnUnmonitoredObservablePropertyChangedMethod!.Name;
                }

                if ( this._builder.Target.IsSealed )
                {
                    b.Accessibility = isOverride ? this._baseOnUnmonitoredObservablePropertyChangedMethod!.Accessibility : Accessibility.Private;
                }
                else
                {
                    b.Accessibility = isOverride ? this._baseOnUnmonitoredObservablePropertyChangedMethod!.Accessibility : Accessibility.Protected;
                    b.IsVirtual = !isOverride;
                }
            },
            args: new {
                deferredExecutionContext = this._deferredTemplateExecutionContext
            } );

        this._onUnmonitoredObservablePropertyChangedMethod.Value = result.Declaration;
    }

    private bool ValidateBaseImplementation()
    {
        var isValid = true;

        if ( this._targetImplementsInpc && this._baseOnPropertyChangedMethod == null )
        {
            this._builder.Diagnostics.Report(
                DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( this._builder.Target ) );

            isValid = false;
        }

        return isValid;
    }

    private void IntroduceInterfaceIfRequired()
    {
        if ( !this._targetImplementsInpc )
        {
            this._builder.Advice.WithTemplateProvider( Templates.Provider ).ImplementInterface( this._builder.Target, this._elements.INotifyPropertyChanged );
        }
    }

    private void IntroduceUpdateMethods()
    {
        var allNodesDepthFirst = this.DependencyGraph.DescendantsDepthFirst().ToList();
        allNodesDepthFirst.Reverse();

        // Process all nodes in depth-first, leaf-to-root order, creating necessary update methods as we go.
        // The order is important, so that parent nodes test if child node methods were necessary and invoke them.

        // NB: We might be able do this with DeferredDeclaration<T> and care less about ordering.

        foreach ( var node in allNodesDepthFirst )
        {
            if ( node.Children.Count == 0 || node.Parent!.IsRoot )
            {
                // Leaf nodes and root properties should never have update methods.
                node.UpdateMethod.Value = null;

                continue;
            }

            IMethod? thisUpdateMethod = null;

            // Don't add fields and update methods for properties handled by base, or for root properties of the target type, or for properties of types that don't implement INPC.
            if ( node.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit
                 && !this.HasInheritedOnChildPropertyChangedPropertyPath( node.DottedPropertyPath ) )
            {
                var lastValueField = this.GetOrCreateLastValueField( node );
                var onPropertyChangedHandlerField = this.GetOrCreateHandlerField( node );

                var methodName = this.GetAndReserveUnusedMemberName( $"Update{node.ContiguousPropertyPathWithoutDot}" );

                var accessChildExprBuilder = new ExpressionBuilder();

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity [Justification: code must remain compatible with netstandard2.0]
#endif
                accessChildExprBuilder.AppendVerbatim( node.DottedPropertyPath.Replace( ".", "?." ) );
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

                var accessChildExpression = accessChildExprBuilder.ToExpression();

                var introduceUpdateChildPropertyMethodResult = this._builder.Advice.WithTemplateProvider( Templates.Provider ).IntroduceMethod(
                    this._builder.Target,
                    nameof( Templates.UpdateChildInpcProperty ),
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b =>
                    {
                        b.Name = methodName;
                        b.Accessibility = Accessibility.Private;
                    },
                    args: new
                    {
                        deferredExecutionContext = this._deferredTemplateExecutionContext,
                        node,
                        accessChildExpression,
                        lastValueField,
                        onPropertyChangedHandlerField
                    } );

                thisUpdateMethod = introduceUpdateChildPropertyMethodResult.Declaration;

                // This type will raise OnChildPropertyChanged for the current node, let derived types know.
                this._propertyPathsForOnChildPropertyChangedMethodAttribute.Add( node.DottedPropertyPath );
            }

            node.UpdateMethod.Value = thisUpdateMethod;
        }
    }

    private void ProcessAutoProperties()
    {
        var target = this._builder.Target;

        // PS appears to consider all instance properties regardless of accessibility.
        var autoProperties =
            target.Properties
                .Where(
                    p =>
                        p is { IsStatic: false, IsAutoPropertyOrField: true }
                        && !p.Attributes.Any( this._elements.IgnoreAutoChangeNotificationAttribute ) )
                .ToList();

        foreach ( var p in autoProperties )
        {
            var propertyTypeInstrumentationKind = this._inpcInstrumentationKindLookup.Get( p.Type );
            var propertyTypeImplementsInpc = propertyTypeInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit;
            var node = this.DependencyGraph.GetChild( p.GetSymbol() );

            switch ( p.Type.IsReferenceType )
            {
                case true:

                    if ( propertyTypeImplementsInpc )
                    {
                        var hasDependentProperties = node != null;

                        IField? handlerField = null;
                        IMethod? subscribeMethod = null;

                        if ( hasDependentProperties )
                        {
                            handlerField = this.GetOrCreateHandlerField( node! );
                            subscribeMethod = this.GetOrCreateRootPropertySubscribeMethod( node! );
                            this._propertyPathsForOnChildPropertyChangedMethodAttribute.Add( p.Name );

                            if ( p.InitializerExpression != null )
                            {
                                this._builder.Advice.WithTemplateProvider( Templates.Provider ).AddInitializer(
                                    this._builder.Target,
                                    nameof( Templates.SubscribeInitializer ),
                                    InitializerKind.BeforeInstanceConstructor,
                                    args: new { fieldOrProperty = p, subscribeMethod } );
                            }
                        }
                        else
                        {
                            this._propertyNamesForOnUnmonitoredObservablePropertyChangedMethodAttribute.Add( p.Name );
                        }

                        this._builder.Advice.WithTemplateProvider( Templates.Provider ).OverrideAccessors(
                            p,
                            setTemplate: nameof( Templates.OverrideInpcRefTypePropertySetter ),
                            args: new {
                                deferredExecutionContext = this._deferredTemplateExecutionContext, 
                                handlerField, 
                                node, 
                                subscribeMethod 
                            } );
                    }
                    else
                    {
                        this._builder.Advice.WithTemplateProvider( Templates.Provider ).OverrideAccessors(
                            p,
                            setTemplate: nameof( Templates.OverrideUninstrumentedTypePropertySetter ),
                            args: new { 
                                deferredExecutionContext = this._deferredTemplateExecutionContext, 
                                node, 
                                compareUsing = EqualityComparisonKind.ReferenceEquals, 
                                propertyTypeInstrumentationKind 
                            } );
                    }

                    break;

                case false:

                    var comparisonKind = p.Type is INamedType nt && nt.SpecialType != SpecialType.ValueTask_T
                        ? EqualityComparisonKind.EqualityOperator
                        : EqualityComparisonKind.DefaultEqualityComparer;

                    this._builder.Advice.WithTemplateProvider( Templates.Provider ).OverrideAccessors(
                        p,
                        setTemplate: nameof( Templates.OverrideUninstrumentedTypePropertySetter ),
                        args: new {
                            deferredExecutionContext = this._deferredTemplateExecutionContext, 
                            node, 
                            compareUsing = comparisonKind, 
                            propertyTypeInstrumentationKind 
                        } );

                    break;
            }
        }
    }

    private HashSet<string>? _inheritedOnChildPropertyChangedPropertyPaths;

    private bool HasInheritedOnChildPropertyChangedPropertyPath( string parentPropertyPath )
    {
        this._inheritedOnChildPropertyChangedPropertyPaths ??=
            BuildPropertyPathLookup( GetPropertyPaths( this._elements.OnChildPropertyChangedMethodAttribute, this._baseOnChildPropertyChangedMethod ) );

        return this._inheritedOnChildPropertyChangedPropertyPaths.Contains( parentPropertyPath );
    }

    private HashSet<string>? _inheritedOnUnmonitoredObservablePropertyChangedPropertyNames;

    private bool HasInheritedOnUnmonitoredObservablePropertyChangedProperty( string propertyName )
    {
        this._inheritedOnUnmonitoredObservablePropertyChangedPropertyNames ??=
            BuildPropertyPathLookup(
                GetPropertyPaths( this._elements.OnUnmonitoredObservablePropertyChangedMethodAttribute, this._baseOnUnmonitoredObservablePropertyChangedMethod ) );

        return this._inheritedOnUnmonitoredObservablePropertyChangedPropertyNames.Contains( propertyName );
    }

    private DependencyGraphNode? _dependencyGraph;
    private bool _prepareDependencyGraphReportedErrors;

    private DependencyGraphNode PrepareDependencyGraph()
    {
        var graph = Implementation.DependencyGraph.GetDependencyGraph<DependencyGraphNode>(
            this._builder.Target,
            ( diagnostic, location ) =>
            {
                this._prepareDependencyGraphReportedErrors |= diagnostic.Definition.Severity == Severity.Error;
                this._builder.Diagnostics.Report( diagnostic, location.ToDiagnosticLocation() );
            } );

        foreach ( var node in graph.DescendantsDepthFirst() )
        {
            this._prepareDependencyGraphReportedErrors |= !node.Initialize( this );
        }

        return graph;
    }

    private DependencyGraphNode DependencyGraph => this._dependencyGraph ??= this.PrepareDependencyGraph();

    private bool PrepareDependencyGraphReportedErrors
    {
        get
        {
            _ = this.DependencyGraph;

            return this._prepareDependencyGraphReportedErrors;
        }
    }

    private IField GetOrCreateLastValueField( DependencyGraphNode node )
    {
        if ( node.LastValueField == null )
        {
            var lastValueFieldName = this.GetAndReserveUnusedMemberName( $"_last{node.ContiguousPropertyPathWithoutDot}" );

            var introduceLastValueFieldResult = this._builder.Advice.IntroduceField(
                this._builder.Target,
                lastValueFieldName,
                node.FieldOrProperty.Type.ToNullableType(),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b => b.Accessibility = Accessibility.Private );

            node.SetLastValueField( introduceLastValueFieldResult.Declaration );
        }

        return node.LastValueField!;
    }

    private IField GetOrCreateHandlerField( DependencyGraphNode node )
    {
        if ( node.HandlerField == null )
        {
            var handlerFieldName = this.GetAndReserveUnusedMemberName( $"_on{node.ContiguousPropertyPathWithoutDot}PropertyChangedHandler" );

            var introduceHandlerFieldResult = this._builder.Advice.WithTemplateProvider( Templates.Provider ).IntroduceField(
                this._builder.Target,
                handlerFieldName,
                this._elements.NullablePropertyChangedEventHandler,
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b => b.Accessibility = Accessibility.Private );

            node.SetHandlerField( introduceHandlerFieldResult.Declaration );
        }

        return node.HandlerField!;
    }

    private IMethod GetOrCreateRootPropertySubscribeMethod( DependencyGraphNode node )
    {
        if ( node.Depth != 1 )
        {
            throw new ArgumentException( "Must be a root property node (depth must be 1).", nameof( node ) );
        }

        if ( !node.SubscribeMethod.ValueIsSet )
        {
            var handlerField = this.GetOrCreateHandlerField( node );

            var subscribeMethodName = this.GetAndReserveUnusedMemberName( $"SubscribeTo{node.Name}" );

            var result = this._builder.Advice.WithTemplateProvider( Templates.Provider ).IntroduceMethod(
                this._builder.Target,
                nameof( Templates.Subscribe ),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b =>
                {
                    b.Name = subscribeMethodName;
                    b.Accessibility = Accessibility.Private;
                },
                args: new { TValue = node.FieldOrProperty.Type, ctx = this, node, handlerField } );

            node.SubscribeMethod.Value = result.Declaration;
        }

        return node.SubscribeMethod.Value;
    }

    private HashSet<string>? _existingMemberNames;

    /// <summary>
    /// Gets an unused member name for the target type by adding an numeric suffix until an unused name is found.
    /// </summary>
    /// <param name="desiredName"></param>
    /// <returns></returns>
    private string GetAndReserveUnusedMemberName( string desiredName )
    {
        this._existingMemberNames ??= new HashSet<string>(
            ((IEnumerable<INamedDeclaration>) this._builder.Target.AllMembers()).Concat( this._builder.Target.NestedTypes ).Select( m => m.Name ) );

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

    [return: NotNullIfNotNull( nameof( method ) )]
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
                && m.Attributes.Any( typeof( OnChildPropertyChangedMethodAttribute ) )
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }, { Type.SpecialType: SpecialType.String }] );

    private IMethod? GetOnUnmonitoredObservablePropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && m.Attributes.Any( typeof( OnUnmonitoredObservablePropertyChangedMethodAttribute ) )
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }, _, _]
                && m.Parameters[1].Type == this._elements.NullableINotifyPropertyChanged
                && m.Parameters[2].Type == this._elements.NullableINotifyPropertyChanged );

    /// <summary>
    /// Validates the given <see cref="IFieldOrProperty"/>, reporting diagnostics if applicable. The result is cached
    /// so that diagnostics are not repeated.
    /// </summary>
    /// <returns><see langword="true"/> if valid, or <see langword="false"/> if invalid.</returns>
    private bool ValidateFieldOrProperty( IFieldOrProperty fieldOrProperty )
    {
        if ( !this._validateFieldOrPropertyResults.TryGetValue( fieldOrProperty, out var result ) )
        {
            result = IsValid( fieldOrProperty );
            this._validateFieldOrPropertyResults[fieldOrProperty] = result;
        }

        return result;

        bool IsValid( IFieldOrProperty fp )
        {
            var typeImplementsInpc =
                this._inpcInstrumentationKindLookup.Get( fp.Type ) is InpcInstrumentationKind.Explicit or InpcInstrumentationKind.Implicit;

            var isValid = true;

            switch ( fp.Type.IsReferenceType )
            {
                case null:
                    // This might require INPC-type code which is used at runtime only when T implements INPC,
                    // and non-INPC-type code which is used at runtime when T does not implement INPC.

                    this._builder.Diagnostics.Report(
                        DiagnosticDescriptors.NotifyPropertyChanged.ErrorFieldOrPropertyTypeIsUnconstrainedGeneric.WithArguments(
                            (fp.DeclarationKind, fp, fp.Type) ),
                        fp );

                    isValid = false;

                    break;

                case false:

                    if ( typeImplementsInpc )
                    {
                        this._builder.Diagnostics.Report(
                            DiagnosticDescriptors.NotifyPropertyChanged.ErrorFieldOrPropertyTypeIsStructImplementingInpc.WithArguments(
                                (fp.DeclarationKind, fp, fp.Type) ),
                            fp );

                        isValid = false;
                    }

                    break;
            }

            if ( fp.IsVirtual )
            {
                this._builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorVirtualMemberIsNotSupported.WithArguments( (fp.DeclarationKind, fp) ),
                    fp );

                isValid = false;
            }

            if ( fp.IsNew )
            {
                this._builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorNewMemberIsNotSupported.WithArguments( (fp.DeclarationKind, fp) ),
                    fp );

                isValid = false;
            }

            return isValid;
        }
    }
}