// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Engine.CodeModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

internal sealed partial class NaturalAspect : IAspect<INamedType>
{
    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();
    }

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var ctx = new BuildAspectContext( builder );

        // Validate, maximising the coverage of diagnostic reporting.

        var v1 = ValidateBaseImplementation( ctx );
        var v2 = ValidateDependencyAnalysis( ctx );
        var v3 = ValidateRootAutoProperties( ctx );

        // Transform, if valid. By design, aim to minimise diagnostic reporting that only occurs during
        // the transform phase.
        if ( v1 && v2 && v3 )
        {
            IntroduceInterfaceIfRequired( ctx );

            // Introduce methods like UpdateA1B1()
            IntroduceUpdateMethods( ctx );

            // Override auto properties
            ProcessAutoProperties( ctx );

            AddPropertyPathsForOnChildPropertyChangedMethodAttribute( ctx );

            IntroduceOnPropertyChangedMethod( ctx );
            IntroduceOnChildPropertyChangedMethod( ctx );
            IntroduceOnUnmonitoredObservablePropertyChanged( ctx );
        }
    }

    private static bool ValidateDependencyAnalysis( BuildAspectContext ctx )
    {
        return !ctx.PrepareDependencyGraphReportedErrors;
    }

    private static bool ValidateRootAutoProperties( BuildAspectContext ctx )
    {
        // TODO: Add support for fields.

        var relevantProperties =
            ctx.Target.Properties
                .Where(
                    p =>
                        p is { IsStatic: false, IsAutoPropertyOrField: true }
                        && !p.Attributes.Any( ctx.Elements.IgnoreAutoChangeNotificationAttribute ) );

        var allValid = true;

        foreach ( var p in relevantProperties )
        {
            allValid &= ctx.ValidateFieldOrProperty( p );
        }

        return allValid;
    }

    private static void AddPropertyPathsForOnChildPropertyChangedMethodAttribute( BuildAspectContext ctx )
    {
        // NB: The selection logic here must be kept in sync with the logic in the OnUnmonitoredObservablePropertyChanged template.

        ctx.PropertyPathsForOnChildPropertyChangedMethodAttribute.AddRange(
            ctx.DependencyGraph.DescendantsDepthFirst()
                .Where(
                    n => n.InpcBaseHandling switch
                    {
                        InpcBaseHandling.OnUnmonitoredObservablePropertyChanged when ctx.OnUnmonitoredObservablePropertyChangedMethod.WillBeDefined => true,
                        InpcBaseHandling.OnPropertyChanged when n.HasChildren => true,
                        _ => false
                    } )
                .Select( n => n.DottedPropertyPath ) );
    }

    private static void IntroduceOnPropertyChangedMethod( BuildAspectContext ctx )
    {
        var isOverride = ctx.BaseOnPropertyChangedMethod != null;

        var result = ctx.Builder.Advice.IntroduceMethod(
            ctx.Target,
            nameof(OnPropertyChanged),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                if ( isOverride )
                {
                    b.Name = ctx.BaseOnPropertyChangedMethod!.Name;
                }

                if ( ctx.Target.IsSealed )
                {
                    b.Accessibility = isOverride ? ctx.BaseOnPropertyChangedMethod!.Accessibility : Accessibility.Private;
                }
                else
                {
                    b.Accessibility = isOverride ? ctx.BaseOnPropertyChangedMethod!.Accessibility : Accessibility.Protected;
                    b.IsVirtual = !isOverride;
                }
            },
            args: new { ctx } );

        ctx.OnPropertyChangedMethod.Declaration = result.Declaration;

        // Ensure that all required fields are generated in advance of template execution.
        // The node selection logic mirrors that of the template's loops and conditions.

        if ( ctx.OnUnmonitoredObservablePropertyChangedMethod.WillBeDefined )
        {
            foreach ( var node in ctx.DependencyGraph.DescendantsDepthFirst()
                         .Where( n => n.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredObservablePropertyChanged ) )
            {
                _ = ctx.GetOrCreateHandlerField( node );
            }
        }

        foreach ( var node in ctx.DependencyGraph.Children
                     .Where( node => node.InpcBaseHandling is InpcBaseHandling.OnPropertyChanged && node.HasChildren ) )
        {
            _ = ctx.GetOrCreateHandlerField( node );
            _ = ctx.GetOrCreateLastValueField( node );
        }
    }

    private static void IntroduceOnChildPropertyChangedMethod( BuildAspectContext ctx )
    {
        var isOverride = ctx.BaseOnChildPropertyChangedMethod != null;

        var result = ctx.Builder.Advice.IntroduceMethod(
            ctx.Target,
            nameof(OnChildPropertyChanged),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                b.AddAttribute(
                    AttributeConstruction.Create(
                        ctx.Elements.OnChildPropertyChangedMethodAttribute,
                        new[] { ctx.PropertyPathsForOnChildPropertyChangedMethodAttribute.OrderBy( s => s ).ToArray() } ) );

                if ( isOverride )
                {
                    b.Name = ctx.BaseOnChildPropertyChangedMethod!.Name;
                }

                if ( ctx.Target.IsSealed )
                {
                    b.Accessibility = isOverride ? ctx.BaseOnChildPropertyChangedMethod!.Accessibility : Accessibility.Private;
                }
                else
                {
                    b.Accessibility = isOverride ? ctx.BaseOnChildPropertyChangedMethod!.Accessibility : Accessibility.Protected;
                    b.IsVirtual = !isOverride;
                }
            },
            args: new { ctx } );

        ctx.OnChildPropertyChangedMethod.Declaration = result.Declaration;
    }

    private static void IntroduceOnUnmonitoredObservablePropertyChanged( BuildAspectContext ctx )
    {
        if ( !ctx.OnUnmonitoredObservablePropertyChangedMethod.WillBeDefined )
        {
            return;
        }

        var isOverride = ctx.BaseOnUnmonitoredObservablePropertyChangedMethod != null;

        var result = ctx.Builder.Advice.IntroduceMethod(
            ctx.Target,
            nameof(OnUnmonitoredObservablePropertyChanged),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                b.AddAttribute(
                    AttributeConstruction.Create(
                        ctx.Elements.OnUnmonitoredObservablePropertyChangedMethodAttribute,
                        new[] { ctx.PropertyNamesForOnUnmonitoredObservablePropertyChangedMethodAttribute.OrderBy( s => s ).ToArray() } ) );

                if ( isOverride )
                {
                    b.Name = ctx.BaseOnUnmonitoredObservablePropertyChangedMethod!.Name;
                }

                if ( ctx.Target.IsSealed )
                {
                    b.Accessibility = isOverride ? ctx.BaseOnUnmonitoredObservablePropertyChangedMethod!.Accessibility : Accessibility.Private;
                }
                else
                {
                    b.Accessibility = isOverride ? ctx.BaseOnUnmonitoredObservablePropertyChangedMethod!.Accessibility : Accessibility.Protected;
                    b.IsVirtual = !isOverride;
                }
            },
            args: new { ctx } );

        ctx.OnUnmonitoredObservablePropertyChangedMethod.Declaration = result.Declaration;
    }

    private static bool ValidateBaseImplementation( BuildAspectContext ctx )
    {
        var isValid = true;

        if ( ctx is { TargetImplementsInpc: true, BaseOnPropertyChangedMethod: null } )
        {
            ctx.Builder.Diagnostics.Report(
                DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( ctx.Target ) );

            isValid = false;
        }

        return isValid;
    }

    private static void IntroduceInterfaceIfRequired( BuildAspectContext ctx )
    {
        if ( !ctx.TargetImplementsInpc )
        {
            ctx.Builder.Advice.ImplementInterface( ctx.Target, ctx.Elements.INotifyPropertyChanged );
        }
    }

    private static void IntroduceUpdateMethods( BuildAspectContext ctx )
    {
        var allNodesDepthFirst = ctx.DependencyGraph.DescendantsDepthFirst().ToList();
        allNodesDepthFirst.Reverse();

        // Process all nodes in depth-first, leaf-to-root order, creating necessary update methods as we go.
        // The order is important, so that parent nodes test if child node methods were necessary and invoke them.

        // NB: We might be able do this with DeferredDeclaration<T> and care less about ordering.

        foreach ( var node in allNodesDepthFirst )
        {
            if ( node.Children.Count == 0 || node.Parent!.IsRoot )
            {
                // Leaf nodes and root properties should never have update methods.
                node.UpdateMethod.Declaration = null;

                continue;
            }

            IMethod? thisUpdateMethod = null;

            // Don't add fields and update methods for properties handled by base, or for root properties of the target type, or for properties of types that don't implement INPC.
            if ( node.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit
                 && !ctx.HasInheritedOnChildPropertyChangedPropertyPath( node.DottedPropertyPath ) )
            {
                var lastValueField = ctx.GetOrCreateLastValueField( node );
                var onPropertyChangedHandlerField = ctx.GetOrCreateHandlerField( node );

                var methodName = ctx.GetAndReserveUnusedMemberName( $"Update{node.ContiguousPropertyPathWithoutDot}" );

                var accessChildExprBuilder = new ExpressionBuilder();

#if NETCOREAPP
#pragma warning disable CA1307 // Specify StringComparison for clarity [Justification: code must remain compatible with netstandard2.0]
#endif
                accessChildExprBuilder.AppendVerbatim( node.DottedPropertyPath.Replace( ".", "?." ) );
#if NETCOREAPP
#pragma warning restore CA1307 // Specify StringComparison for clarity
#endif

                var accessChildExpression = accessChildExprBuilder.ToExpression();

                var introduceUpdateChildPropertyMethodResult = ctx.Builder.Advice.IntroduceMethod(
                    ctx.Target,
                    nameof(UpdateChildInpcProperty),
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b =>
                    {
                        b.Name = methodName;
                        b.Accessibility = Accessibility.Private;
                    },
                    args: new
                    {
                        ctx,
                        node,
                        accessChildExpression,
                        lastValueField,
                        onPropertyChangedHandlerField
                    } );

                thisUpdateMethod = introduceUpdateChildPropertyMethodResult.Declaration;

                // This type will raise OnChildPropertyChanged for the current node, let derived types know.
                ctx.PropertyPathsForOnChildPropertyChangedMethodAttribute.Add( node.DottedPropertyPath );
            }

            node.UpdateMethod.Declaration = thisUpdateMethod;
        }
    }

    private static void ProcessAutoProperties( BuildAspectContext ctx )
    {
        var target = ctx.Target;

        // PS appears to consider all instance properties regardless of accessibility.
        var autoProperties =
            target.Properties
                .Where(
                    p =>
                        p is { IsStatic: false, IsAutoPropertyOrField: true } 
                        && !p.Attributes.Any( ctx.Elements.IgnoreAutoChangeNotificationAttribute ) )
                .ToList();

        foreach ( var p in autoProperties )
        {
            var propertyTypeInstrumentationKind = ctx.GetInpcInstrumentationKind( p.Type );
            var propertyTypeImplementsInpc = propertyTypeInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit;
            var node = ctx.DependencyGraph.GetChild( p.GetSymbol() );

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
                            handlerField = ctx.GetOrCreateHandlerField( node! );
                            subscribeMethod = ctx.GetOrCreateRootPropertySubscribeMethod( node! );
                            ctx.PropertyPathsForOnChildPropertyChangedMethodAttribute.Add( p.Name );

                            if ( p.InitializerExpression != null )
                            {
                                ctx.Builder.Advice.AddInitializer(
                                    ctx.Target,
                                    nameof(SubscribeInitializer),
                                    InitializerKind.BeforeInstanceConstructor,
                                    args: new { fieldOrProperty = p, subscribeMethod } );
                            }
                        }
                        else
                        {
                            ctx.PropertyNamesForOnUnmonitoredObservablePropertyChangedMethodAttribute.Add( p.Name );
                        }
                        
                        ctx.Builder.Advice.Override( p, nameof(OverrideInpcRefTypeProperty), tags: new { ctx, handlerField, node, subscribeMethod } );
                    }
                    else
                    {
                        ctx.Builder.Advice.Override(
                            p,
                            nameof(OverrideUninstrumentedTypeProperty),
                            tags: new { ctx, node, compareUsing = EqualityComparisonKind.ReferenceEquals, propertyTypeInstrumentationKind } );
                    }

                    break;

                case false:

                    var comparisonKind = p.Type is INamedType nt && nt.SpecialType != SpecialType.ValueTask_T
                        ? EqualityComparisonKind.EqualityOperator
                        : EqualityComparisonKind.DefaultEqualityComparer;

                    ctx.Builder.Advice.Override(
                        p,
                        nameof(OverrideUninstrumentedTypeProperty),
                        tags: new { ctx, node, compareUsing = comparisonKind, propertyTypeInstrumentationKind } );

                    break;
            }
        }
    }
}