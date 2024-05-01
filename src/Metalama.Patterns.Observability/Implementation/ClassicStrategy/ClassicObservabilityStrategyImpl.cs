// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;
using Metalama.Patterns.Observability.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

internal sealed partial class ClassicObservabilityStrategyImpl : IObservabilityStrategy
{
    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    private readonly IAspectBuilder<INamedType> _builder;
    private readonly Deferred<ObservabilityTemplateArgs> _templateArgs = new();
    private readonly ObservabilityOptions _commonOptions;
    private readonly ClassicObservabilityStrategyOptions _classicOptions;
    private readonly Dictionary<IFieldOrProperty, bool> _validateFieldOrPropertyResults = new();
    private readonly IMethod? _baseOnPropertyChangedMethod;
    private readonly IMethod? _baseOnChildPropertyChangedMethod;
    private readonly IMethod? _baseOnObservablePropertyChangedMethod;
    private readonly Assets _assets;
    private readonly InpcInstrumentationKindLookup _inpcInstrumentationKindLookup;
    private readonly bool _targetImplementsInpc;

    // Useful to see when debugging:
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly bool _baseImplementsInpc;
    private readonly Deferred<IMethod> _onPropertyChangedMethod = new();
    private readonly Deferred<IMethod?> _onChildPropertyChangedMethod = new();
    private readonly Deferred<IMethod?>? _onObservablePropertyChangedMethod;
    private readonly List<string> _propertyPathsForOnChildPropertyChangedMethod = new();
    private readonly List<string> _propertyNamesForOnObservablePropertyChangedMethod = new();
    private readonly Deferred<ClassicDependencyTypeNode> _dependencyTypeNode = new();

    public ClassicObservabilityStrategyImpl( IAspectBuilder<INamedType> builder )
    {
        var target = builder.Target;

        this._builder = builder;
        this._assets = target.Compilation.Cache.GetOrAdd( _ => new Assets() );
        this._inpcInstrumentationKindLookup = new InpcInstrumentationKindLookup( this._builder.Target, this._assets );
        this._commonOptions = builder.Target.Enhancements().GetOptions<ObservabilityOptions>();
        this._classicOptions = builder.Target.Enhancements().GetOptions<ClassicObservabilityStrategyOptions>();

        // TODO: Consider using BaseType.Definition where possible for better performance.

        this._baseImplementsInpc =
            target.BaseType != null && (
                target.BaseType.Is( this._assets.INotifyPropertyChanged )
                || (target.BaseType is { BelongsToCurrentProject: true }
                    && target.BaseType.Definition.Enhancements().HasAspect( typeof(ObservableAttribute) )));

        this._targetImplementsInpc = this._baseImplementsInpc || target.Is( this._assets.INotifyPropertyChanged );
        this._baseOnPropertyChangedMethod = GetOnPropertyChangedMethod( target );
        this._baseOnChildPropertyChangedMethod = GetOnChildPropertyChangedMethod( target );
        this._baseOnObservablePropertyChangedMethod = GetOnObservablePropertyChangedMethod( target, this._assets );

        var useOnObservablePropertyChangedMethod =
            this._classicOptions.EnableOnObservablePropertyChangedMethod == true &&
            (!target.IsSealed || this._baseOnObservablePropertyChangedMethod != null);

        if ( useOnObservablePropertyChangedMethod )
        {
            this._onObservablePropertyChangedMethod = new Deferred<IMethod?>();
        }
    }

    public void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        if ( builder != this._builder )
        {
            throw new ArgumentOutOfRangeException();
        }

        // Validate, maximising the coverage of diagnostic reporting.

        var v1 = this.ValidateBaseImplementation();
        var v2 = this.BuildAndValidateDependencyGraph();
        var v3 = this.ValidateRootAutoProperties();

        // Transform, if valid. By design, aim to minimise diagnostic reporting that only occurs during
        // the transform phase.
        if ( v1 && v2 && v3 )
        {
            this.IntroduceInterfaceIfRequired();

            // Introduce methods like UpdateA1B1()
            this.IntroduceUpdateMethods();

            this.ProcessAutoPropertiesAndReferencedFields();

            this.AddPropertyPathsForOnChildPropertyChangedMethodAttribute();

            if ( !this.TryIntroduceOnPropertyChangedMethod() )
            {
                return;
            }

            if ( !this.TryIntroduceOnChildPropertyChangedMethod() )
            {
                return;
            }

            if ( !this.TryIntroduceOnObservablePropertyChanged() )
            {
                return;
            }

            this._templateArgs.Value = new ObservabilityTemplateArgs(
                this._commonOptions,
                this._classicOptions,
                this._builder.Target,
                this._assets,
                this._inpcInstrumentationKindLookup,
                this.DependencyTypeNode,
                this._onObservablePropertyChangedMethod?.Value,
                this._onPropertyChangedMethod.Value,
                this._onChildPropertyChangedMethod.Value,
                this._baseOnPropertyChangedMethod,
                this._baseOnChildPropertyChangedMethod,
                this._baseOnObservablePropertyChangedMethod );
        }
    }

    private bool ValidateRootAutoProperties()
    {
        // TODO: Add support for fields.

        // Root auto properties that are referenced by other properties will also be validated
        // when building the dependency graph. Here we make sure to cover unreferenced properties.
        // The ValidateFieldOrProperty method caches results and will not raise duplicate diagnostics,
        // so it's simplest to just run though all the root properties regardless of whether they
        // are covered by the dependency graph.

        var relevantProperties =
            this._builder.Target.Properties
                .Where(
                    p =>
                        p is { IsStatic: false, IsAutoPropertyOrField: true }
                        && !p.Attributes.Any( this._assets.NotObservableAttribute ) );

        var allValid = true;

        foreach ( var p in relevantProperties )
        {
            allValid &= this.ValidateFieldOrPropertyIntrinsicCharacteristics( p );
        }

        return allValid;
    }

    private void AddPropertyPathsForOnChildPropertyChangedMethodAttribute()
    {
        // NB: The selection logic here must be kept in sync with the logic in the OnObservablePropertyChanged template.

        this._propertyPathsForOnChildPropertyChangedMethod.AddRange(
            this.DependencyTypeNode.AllReferences
                .Where(
                    n => n.InpcBaseHandling switch
                    {
                        InpcBaseHandling.OnObservablePropertyChanged when this._onObservablePropertyChangedMethod != null =>
                            true,
                        InpcBaseHandling.OnPropertyChanged when n.HasChildren => true,
                        _ => false
                    } )
                .Select( n => n.DottedPropertyPath ) );
    }

    private bool TryIntroduceOnPropertyChangedMethod()
    {
        var isOverride = this._baseOnPropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( Templates.Provider )
            .IntroduceMethod(
                this._builder.Target,
                nameof(Templates.OnPropertyChanged),
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
                args: new { templateArgs = this._templateArgs } );

        if ( result.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        this._onPropertyChangedMethod.Value = result.Declaration;

        // Ensure that all required fields are generated in advance of template execution.
        // The node selection logic mirrors that of the template's loops and conditions.

        if ( this._onObservablePropertyChangedMethod != null )
        {
            foreach ( var node in this.DependencyTypeNode
                         .AllReferences
                         .Where( n => n.InpcBaseHandling == InpcBaseHandling.OnObservablePropertyChanged ) )
            {
                _ = this.GetOrCreateHandlerField( node );
            }
        }

        foreach ( var node in this.DependencyTypeNode
                     .AllReferences
                     .Where( node => node.InpcBaseHandling is InpcBaseHandling.OnPropertyChanged && node.HasChildren ) )
        {
            _ = this.GetOrCreateHandlerField( node );
            _ = this.GetOrCreateLastValueField( node );
        }

        return true;
    }

    private bool TryIntroduceOnChildPropertyChangedMethod()
    {
        var nodesForOnChildPropertyChanged =
            this.DependencyTypeNode
                .AllReferences
                .Where( n => n.Depth > 0 )
                .Where(
                    node =>
                    {
                        var rootPropertyNode = node.Root;

                        if ( rootPropertyNode.ReferencedFieldOrProperty.DeclaringType == this._builder.Target )
                        {
                            return false;
                        }

                        var firstAncestorWithNotNoneHandling = node.Ancestors().FirstOrDefault( n => n.InpcBaseHandling != InpcBaseHandling.None );

                        if ( firstAncestorWithNotNoneHandling != null )
                        {
                            switch ( firstAncestorWithNotNoneHandling.InpcBaseHandling )
                            {
                                case InpcBaseHandling.OnObservablePropertyChanged when this._onObservablePropertyChangedMethod != null:
                                    return false;

                                case InpcBaseHandling.OnChildPropertyChanged when node.Depth - firstAncestorWithNotNoneHandling.Depth > 1:
                                    return false;

                                case InpcBaseHandling.OnPropertyChanged:
                                    return false;
                            }
                        }

                        return true;
                    } )
                .ToList();

        if ( this._propertyPathsForOnChildPropertyChangedMethod.Count == 0 && nodesForOnChildPropertyChanged.Count == 0 )
        {
            // This method is not necessary.
            this._onChildPropertyChangedMethod.Value = null;

            return true;
        }

        var isOverride = this._baseOnChildPropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( Templates.Provider )
            .IntroduceMethod(
                this._builder.Target,
                nameof(Templates.OnChildPropertyChanged),
                IntroductionScope.Instance,
                isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
                b =>
                {
                    if ( this._propertyPathsForOnChildPropertyChangedMethod.Count > 0 )
                    {
                        b.AddAttribute(
                            AttributeConstruction.Create(
                                this._assets.InvokedForAttribute,
                                this._propertyPathsForOnChildPropertyChangedMethod.OrderBy( s => s ).ToArray() ) );
                    }

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
                args: new { templateArgs = this._templateArgs, nodesForOnChildPropertyChanged } );

        if ( result.Outcome != AdviceOutcome.Error )
        {
            this._onChildPropertyChangedMethod.Value = result.Declaration;

            return true;
        }
        else
        {
            return false;
        }
    }

    private bool TryIntroduceOnObservablePropertyChanged()
    {
        if ( this._onObservablePropertyChangedMethod == null )
        {
            return true;
        }

        var nodesProcessedByOnObservablePropertyChanged = this.DependencyTypeNode.AllReferences
            .Where( n => n.InpcBaseHandling == InpcBaseHandling.OnObservablePropertyChanged )
            .ToList();

        if ( nodesProcessedByOnObservablePropertyChanged.Count == 0 && this._propertyNamesForOnObservablePropertyChangedMethod.Count == 0 )
        {
            // We don't create the method because it would not do anything and it would not be invoked.
            // Note that both conditions need to be true, because we still need the method because of the derived types.
            this._onObservablePropertyChangedMethod.Value = null;

            return true;
        }

        var isOverride = this._baseOnObservablePropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( Templates.Provider )
            .IntroduceMethod(
                this._builder.Target,
                nameof(Templates.OnObservablePropertyChanged),
                IntroductionScope.Instance,
                isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
                b =>
                {
                    if ( this._propertyNamesForOnObservablePropertyChangedMethod.Count > 0 )
                    {
                        b.AddAttribute(
                            AttributeConstruction.Create(
                                this._assets.InvokedForAttribute,
                                this._propertyNamesForOnObservablePropertyChangedMethod.OrderBy( s => s ).ToArray() ) );
                    }

                    if ( isOverride )
                    {
                        b.Name = this._baseOnObservablePropertyChangedMethod!.Name;
                    }

                    if ( this._builder.Target.IsSealed )
                    {
                        b.Accessibility = isOverride ? this._baseOnObservablePropertyChangedMethod!.Accessibility : Accessibility.Private;
                    }
                    else
                    {
                        b.Accessibility = isOverride ? this._baseOnObservablePropertyChangedMethod!.Accessibility : Accessibility.Protected;
                        b.IsVirtual = !isOverride;
                    }
                },
                args: new { templateArgs = this._templateArgs, nodesProcessedByOnObservablePropertyChanged } );

        if ( result.Outcome == AdviceOutcome.Error )
        {
            return false;
        }

        this._onObservablePropertyChangedMethod.Value = result.Declaration;

        return true;
    }

    private bool ValidateBaseImplementation()
    {
        var isValid = true;

        if ( this._targetImplementsInpc && this._baseOnPropertyChangedMethod == null )
        {
            this._builder.Diagnostics.Report(
                DiagnosticDescriptors.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( this._builder.Target ) );

            isValid = false;
        }

        return isValid;
    }

    private void IntroduceInterfaceIfRequired()
    {
        if ( !this._targetImplementsInpc )
        {
            this._builder.Advice.WithTemplateProvider( Templates.Provider ).ImplementInterface( this._builder.Target, this._assets.INotifyPropertyChanged );
        }
    }

    private void IntroduceUpdateMethods()
    {
        var allNodesDepthFirst = this.DependencyTypeNode
            .AllReferences
            .OrderByDescending( x => x.Depth )
            .ThenBy( x => x.DottedPropertyPath )
            .ToList();

        // Process all nodes in depth-first, leaf-to-root order, creating necessary update methods as we go.
        // The order is important, so that parent nodes test if child node methods were necessary and invoke them.

        foreach ( var node in allNodesDepthFirst )
        {
            if ( !node.HasChildren || node.IsRoot )
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

                var methodName = this.GetAndReserveUnusedMemberName( $"Update{node.ContiguousPropertyPath}" );

                var accessChildExprBuilder = new ExpressionBuilder();

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity [Justification: code must remain compatible with netstandard2.0]
#endif
                accessChildExprBuilder.AppendVerbatim( node.DottedPropertyPath.Replace( ".", "?." ) );
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

                var accessChildExpression = accessChildExprBuilder.ToExpression();

                var introduceUpdateChildPropertyMethodResult = this._builder.Advice.WithTemplateProvider( Templates.Provider )
                    .IntroduceMethod(
                        this._builder.Target,
                        nameof(Templates.UpdateChildInpcProperty),
                        IntroductionScope.Instance,
                        OverrideStrategy.Fail,
                        b =>
                        {
                            b.Name = methodName;
                            b.Accessibility = Accessibility.Private;
                        },
                        args: new
                        {
                            templateArgs = this._templateArgs,
                            node,
                            accessChildExpression,
                            lastValueField,
                            onPropertyChangedHandlerField
                        } );

                thisUpdateMethod = introduceUpdateChildPropertyMethodResult.Declaration;

                // This type will raise OnChildPropertyChanged for the current node, let derived types know.
                this._propertyPathsForOnChildPropertyChangedMethod.Add( node.DottedPropertyPath );
            }

            node.UpdateMethod.Value = thisUpdateMethod;
        }
    }

    private void ProcessAutoPropertiesAndReferencedFields()
    {
        var target = this._builder.Target;

        // Override all auto properties, and only those fields that are referenced in the graph.

        var properties =
            target.Properties
                .Where( p => p is { IsStatic: false, IsImplicitlyDeclared: false, Writeability: Writeability.All } )
                .Select( p => (ClassicDependencyPropertyNode) this.DependencyTypeNode.GetOrAddProperty( p ) )
                .Concat(
                    target.Fields
                        .Where( f => f is { IsStatic: false, IsImplicitlyDeclared: false, Writeability: Writeability.All } )
                        .Select( p => (ClassicDependencyPropertyNode) this.DependencyTypeNode.GetOrAddProperty( p ) ) )
                .Where( node => !node.FieldOrProperty.Attributes.Any( this._assets.NotObservableAttribute ) )
                .ToList();

        foreach ( var propertyNode in properties )
        {
            var fieldOrProperty = propertyNode.FieldOrProperty;
            var propertyTypeInstrumentationKind = this._inpcInstrumentationKindLookup.Get( fieldOrProperty.Type );
            var propertyTypeImplementsInpc = propertyTypeInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit;

            switch ( fieldOrProperty.Type.IsReferenceType )
            {
                case null or true:

                    if ( propertyTypeImplementsInpc )
                    {
                        var hasDependentProperties = propertyNode.RootReferenceNode.HasAnyReferencingProperties;

                        IField? handlerField = null;
                        IMethod? subscribeMethod = null;

                        if ( hasDependentProperties )
                        {
                            var rootReference = propertyNode!.RootReferenceNode!;
                            handlerField = this.GetOrCreateHandlerField( rootReference );

                            if ( !this.TryGetOrCreateRootPropertySubscribeMethod( propertyNode, out subscribeMethod ) )
                            {
                                return;
                            }

                            if ( fieldOrProperty.DeclarationKind == DeclarationKind.Property )
                            {
                                this._propertyPathsForOnChildPropertyChangedMethod.Add( fieldOrProperty.Name );
                                this._propertyNamesForOnObservablePropertyChangedMethod.Add( fieldOrProperty.Name );
                            }

                            if ( fieldOrProperty.InitializerExpression != null )
                            {
                                this._builder.Advice.WithTemplateProvider( Templates.Provider )
                                    .AddInitializer(
                                        this._builder.Target,
                                        nameof(Templates.SubscribeInitializer),
                                        InitializerKind.BeforeInstanceConstructor,
                                        args: new { fieldOrProperty, subscribeMethod } );
                            }
                        }
                        else
                        {
                            if ( fieldOrProperty.DeclarationKind == DeclarationKind.Property )
                            {
                                this._propertyNamesForOnObservablePropertyChangedMethod.Add( fieldOrProperty.Name );
                            }
                        }

                        this._builder.Advice.WithTemplateProvider( Templates.Provider )
                            .OverrideAccessors(
                                fieldOrProperty,
                                setTemplate: nameof(Templates.OverrideInpcRefTypePropertySetter),
                                args: new { templateArgs = this._templateArgs, handlerField, node = propertyNode, subscribeMethod } );
                    }
                    else
                    {
                        var comparer = fieldOrProperty.Type.IsReferenceType == null
                            ? EqualityComparisonKind.DefaultEqualityComparer
                            : EqualityComparisonKind.ReferenceEquals;

                        this._builder.Advice.WithTemplateProvider( Templates.Provider )
                            .OverrideAccessors(
                                fieldOrProperty,
                                setTemplate: nameof(Templates.OverrideUninstrumentedTypePropertySetter),
                                args: new
                                {
                                    templateArgs = this._templateArgs, node = propertyNode, compareUsing = comparer, propertyTypeInstrumentationKind
                                } );
                    }

                    break;

                case false:

                    var comparisonKind = fieldOrProperty.Type is INamedType nt && nt.SpecialType != SpecialType.ValueTask_T
                        ? EqualityComparisonKind.EqualityOperator
                        : EqualityComparisonKind.DefaultEqualityComparer;

                    this._builder.Advice.WithTemplateProvider( Templates.Provider )
                        .OverrideAccessors(
                            fieldOrProperty,
                            setTemplate: nameof(Templates.OverrideUninstrumentedTypePropertySetter),
                            args: new
                            {
                                templateArgs = this._templateArgs, node = propertyNode, compareUsing = comparisonKind, propertyTypeInstrumentationKind
                            } );

                    break;
            }
        }
    }

    private HashSet<string>? _inheritedOnChildPropertyChangedPropertyPaths;

    public bool HasInheritedOnChildPropertyChangedPropertyPath( string parentPropertyPath )
    {
        this._inheritedOnChildPropertyChangedPropertyPaths ??=
            BuildPropertyPathLookup( GetPropertyPaths( this._assets.InvokedForAttribute, this._baseOnChildPropertyChangedMethod ) );

        return this._inheritedOnChildPropertyChangedPropertyPaths.Contains( parentPropertyPath );
    }

    private HashSet<string>? _inheritedOnObservablePropertyChangedPropertyNames;

    public bool HasInheritedOnObservablePropertyChangedProperty( string propertyName )
    {
        this._inheritedOnObservablePropertyChangedPropertyNames ??=
            BuildPropertyPathLookup(
                GetPropertyPaths(
                    this._assets.InvokedForAttribute,
                    this._baseOnObservablePropertyChangedMethod ) );

        return this._inheritedOnObservablePropertyChangedPropertyNames.Contains( propertyName );
    }

    /// <summary>
    /// Builds and validates the dependency graph (including validating dependency analysis).
    /// </summary>
    /// <returns>Success. <see langword="true"/> if the graph was built without diagnostic errors, otherwise <see langword="false"/>.</returns>
    private bool BuildAndValidateDependencyGraph()
    {
        var graphBuildingContext = new GraphBuildingContext( this );
        var nodeInitializationContext = new ClassicProcessingNodeInitializationContext( this._builder.Target.Compilation, this, this._builder.Target );

        var graphBuilder = new ClassicDependencyGraphBuilder( nodeInitializationContext );

        var typeNode = (ClassicDependencyTypeNode) graphBuilder.GetDependencyGraph(
            this._builder.Target,
            graphBuildingContext,
            cancellationToken: this._builder.CancellationToken );

        var hasErrors = graphBuildingContext.HasReportedErrors;

        foreach ( var property in typeNode.Properties )
        {
            hasErrors |= !this.ValidateFieldOrPropertyIntrinsicCharacteristics( property.FieldOrProperty );
        }

        this._dependencyTypeNode.Value = typeNode;

        return !hasErrors;
    }

    private ClassicDependencyTypeNode DependencyTypeNode => this._dependencyTypeNode.Value;

    private IField GetOrCreateLastValueField( ClassicDependencyReferenceNode node )
    {
        if ( !node.LastValueField.HasBeenSet )
        {
            var lastValueFieldName = this.GetAndReserveUnusedMemberName( $"_last{node.ContiguousPropertyPath}" );

            var introduceLastValueFieldResult = this._builder.Advice.IntroduceField(
                this._builder.Target,
                lastValueFieldName,
                node.ReferencedFieldOrProperty.Type.ToNullableType(),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b => b.Accessibility = Accessibility.Private );

            node.LastValueField.Value = introduceLastValueFieldResult.Declaration;
        }

        return node.LastValueField.Value;
    }

    private IField GetOrCreateHandlerField( ClassicDependencyReferenceNode node )
    {
        if ( !node.HandlerField.HasBeenSet )
        {
            var handlerFieldName = this.GetAndReserveUnusedMemberName( $"_handle{node.ContiguousPropertyPath}PropertyChanged" );

            var introduceHandlerFieldResult = this._builder.Advice.WithTemplateProvider( Templates.Provider )
                .IntroduceField(
                    this._builder.Target,
                    handlerFieldName,
                    this._assets.NullablePropertyChangedEventHandler,
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b => b.Accessibility = Accessibility.Private );

            node.HandlerField.Value = introduceHandlerFieldResult.Declaration;
        }

        return node.HandlerField.Value;
    }

    private bool TryGetOrCreateRootPropertySubscribeMethod( ClassicDependencyPropertyNode propertyNode, out IMethod? subscribeMethod )
    {
        var referenceNode = propertyNode.RootReferenceNode;

        if ( !referenceNode.SubscribeMethod.HasBeenSet )
        {
            var handlerField = this.GetOrCreateHandlerField( referenceNode );

            var subscribeMethodName = this.GetAndReserveUnusedMemberName( $"SubscribeTo{referenceNode.ContiguousPropertyPath}" );

            var result = this._builder.Advice.WithTemplateProvider( Templates.Provider )
                .IntroduceMethod(
                    this._builder.Target,
                    nameof(Templates.SubscribeTo),
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b =>
                    {
                        b.Name = subscribeMethodName;
                        b.Accessibility = Accessibility.Private;
                    },
                    args: new { TValue = propertyNode.FieldOrProperty.Type, templateArgs = this._templateArgs, node = propertyNode, handlerField } );

            if ( result.Outcome != AdviceOutcome.Error )
            {
                referenceNode.SubscribeMethod.Value = result.Declaration;
            }
            else
            {
                subscribeMethod = null;

                return false;
            }
        }

        subscribeMethod = referenceNode.SubscribeMethod.Value;

        return true;
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
            for ( var i = 1; /* Intentionally empty */; i++ )
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

    internal static IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }]
                && _onPropertyChangedMethodNames.Contains( m.Name ) );

    internal static IMethod? GetOnChildPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }, { Type.SpecialType: SpecialType.String }] );

    internal static IMethod? GetOnObservablePropertyChangedMethod( INamedType type, Assets assets )
        => type.AllMethods.FirstOrDefault(
            m =>
                !m.IsStatic
                && (type.IsSealed || ((m.IsVirtual || m.IsOverride) && m.Accessibility is Accessibility.Public or Accessibility.Protected))
                && m.ReturnType.SpecialType == SpecialType.Void
                && m.Parameters is [{ Type.SpecialType: SpecialType.String }, _, _]
                && m.Parameters[1].Type == assets.NullableINotifyPropertyChanged
                && m.Parameters[2].Type == assets.NullableINotifyPropertyChanged );

    /// <summary>
    /// Validates the the intrinsic characteristics of the given <see cref="IFieldOrProperty"/>, reporting diagnostics if applicable. 
    /// The result is cached so that diagnostics are not repeated.
    /// </summary>
    /// <returns><see langword="true"/> if valid, or <see langword="false"/> if invalid.</returns>
    private bool ValidateFieldOrPropertyIntrinsicCharacteristics( IFieldOrProperty fieldOrProperty )
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

            if ( fp.Type.IsReferenceType == false && typeImplementsInpc )
            {
                this._builder.Diagnostics.Report(
                    DiagnosticDescriptors.ErrorFieldOrPropertyTypeIsStructImplementingInpc.WithArguments( (fp.DeclarationKind, fp, fp.Type) ),
                    fp );

                isValid = false;
            }

            if ( fp.IsVirtual )
            {
                this._builder.Diagnostics.Report(
                    DiagnosticDescriptors.ErrorVirtualMemberIsNotSupported.WithArguments( (fp.DeclarationKind, fp) ),
                    fp );

                isValid = false;
            }

            if ( fp.IsNew )
            {
                this._builder.Diagnostics.Report(
                    DiagnosticDescriptors.ErrorNewMemberIsNotSupported.WithArguments( (fp.DeclarationKind, fp) ),
                    fp );

                isValid = false;
            }

            return isValid;
        }
    }

    public InpcInstrumentationKind GetInpcInstrumentationKind( IType fieldOrPropertyType ) => this._inpcInstrumentationKindLookup.Get( fieldOrPropertyType );
}