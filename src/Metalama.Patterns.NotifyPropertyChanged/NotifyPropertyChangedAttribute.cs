// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;
using System.Diagnostics;

namespace Metalama.Patterns.NotifyPropertyChanged;

[AttributeUsage( AttributeTargets.Class )]
[Inheritable]
public sealed partial class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
{
    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();
    }

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        Debugger.Break();
        var ctx = new BuildAspectContext( builder );

        try
        {
            // The order of method calls here is significant.

            ExamineBaseAndIntroduceInterfaceIfRequired( ctx );
            
            // Introduce methods like UpdateA1B1()
            IntroduceUpdateMethods( ctx );

            // Override auto properties
            ProcessAutoProperties( ctx );

            AddPropertyPathsForOnChildPropertyChangedMethodAttribute( ctx );

            IntroduceOnPropertyChangedMethod( ctx );
            IntroduceOnChildPropertyChangedMethod( ctx );
            IntroduceOnUnmonitoredInpcPropertyChanged( ctx );
        }
        catch ( DiagnosticErrorReportedException )
        {
            // Diagnostic already raised, do nothing.
        }
    }
    
    private static void AddPropertyPathsForOnChildPropertyChangedMethodAttribute( BuildAspectContext ctx )
    {
        // NB: The selection logic here must be kept in sync with the logic in the OnUnmonitoredInpcPropertyChanged template.

        ctx.PropertyPathsForOnChildPropertyChangedMethodAttribute.AddRange(
            ctx.DependencyGraph.DecendantsDepthFirst()
            .Where( n => n.Data.InpcBaseHandling switch
            {
                InpcBaseHandling.OnUnmonitoredInpcPropertyChanged when ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined => true,
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
            nameof( OnPropertyChanged ),
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
            args: new
            {
                ctx
            } );

        ctx.OnPropertyChangedMethod.Declaration = result.Declaration;

        // Ensure that all required fields are generated in advance of template execution.
        // The node selection logic mirrors that of the template's loops and conditons.

        IEnumerable<DependencyGraph.Node<NodeData>> nodes = Array.Empty<DependencyGraph.Node<NodeData>>();

        if ( ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined )
        {
            foreach ( var node in ctx.DependencyGraph.DecendantsDepthFirst()
                .Where( n => n.Data.InpcBaseHandling == InpcBaseHandling.OnUnmonitoredInpcPropertyChanged ) )
            {
                _ = ctx.GetOrCreateHandlerField( node );
            }
        }

        foreach ( var node in ctx.DependencyGraph.Children
            .Where( node => node.Data.InpcBaseHandling is InpcBaseHandling.OnPropertyChanged && node.HasChildren ) )
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
            nameof( OnChildPropertyChanged ),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                b.AddAttribute(
                    AttributeConstruction.Create(
                        ctx.Type_OnChildPropertyChangedMethodAttribute,
                        new[] {
                                ctx.PropertyPathsForOnChildPropertyChangedMethodAttribute.OrderBy( s => s).ToArray()
                        } ) );

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
            args: new
            {
                ctx
            } );

        ctx.OnChildPropertyChangedMethod.Declaration = result.Declaration;
    }

    private static void IntroduceOnUnmonitoredInpcPropertyChanged( BuildAspectContext ctx )
    {
        if ( !ctx.OnUnmonitoredInpcPropertyChangedMethod.WillBeDefined )
        {
            return;
        }

        var isOverride = ctx.BaseOnUnmonitoredInpcPropertyChangedMethod != null;

        var result = ctx.Builder.Advice.IntroduceMethod(
            ctx.Target,
            nameof( OnUnmonitoredInpcPropertyChanged ),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
                b.AddAttribute(
                    AttributeConstruction.Create(
                        ctx.Type_OnUnmonitoredInpcPropertyChangedMethodAttribute,
                        new[]
                        {
                            ctx.PropertyNamesForOnUnmonitoredInpcPropertyChangedMethodAttribute.OrderBy( s => s ).ToArray()
                        } ) );

                if ( isOverride )
                {
                    b.Name = ctx.BaseOnUnmonitoredInpcPropertyChangedMethod!.Name;
                }

                if ( ctx.Target.IsSealed )
                {
                    b.Accessibility = isOverride ? ctx.BaseOnUnmonitoredInpcPropertyChangedMethod!.Accessibility : Accessibility.Private;
                }
                else
                {
                    b.Accessibility = isOverride ? ctx.BaseOnUnmonitoredInpcPropertyChangedMethod!.Accessibility : Accessibility.Protected;
                    b.IsVirtual = !isOverride;
                }
            },
            args: new
            {
                ctx
            } );

        ctx.OnUnmonitoredInpcPropertyChangedMethod.Declaration = result.Declaration;
    }

    private static void ExamineBaseAndIntroduceInterfaceIfRequired( BuildAspectContext ctx )
    {
        if ( ctx.TargetImplementsInpc )
        {
            if ( ctx.BaseOnPropertyChangedMethod == null )
            {
                ctx.Builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( ctx.Target ) );

                throw new DiagnosticErrorReportedException();
            }
        }
        else
        {
            var result = ctx.Builder.Advice.ImplementInterface( ctx.Target, ctx.Type_INotifyPropertyChanged, OverrideStrategy.Fail );

            if ( result.Outcome == Framework.Advising.AdviceOutcome.Error )
            {
                Debugger.Break();
            }
        }
    }

    private static void ValidateFieldOrProperty(
        BuildAspectContext ctx,
        IFieldOrProperty fieldOrProperty )
    {
        var propertyTypeImplementsInpc = ctx.GetInpcInstrumentationKind( fieldOrProperty.Type ) is InpcInstrumentationKind.Explicit or InpcInstrumentationKind.Implicit;

        switch ( fieldOrProperty.Type.IsReferenceType )
        {
            case null:
                // This might require INPC-type code which is used at runtime only when T implements INPC,
                // and non-INPC-type code which is used at runtime when T does not implement INPC.

                ctx.Builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorFieldOrPropertyTypeIsUnconstrainedGeneric.WithArguments( (fieldOrProperty.DeclarationKind, fieldOrProperty, fieldOrProperty.Type) ),
                    fieldOrProperty );

                throw new DiagnosticErrorReportedException();

            case true:

                if ( propertyTypeImplementsInpc )
                {
                    if ( fieldOrProperty.InitializerExpression != null )
                    {
                        // TODO: Support this pattern by moving initializer to ctor.

                        ctx.Builder.Diagnostics.Report(
                            DiagnosticDescriptors.NotifyPropertyChanged.ErrorFieldOrPropertyHasAnInitializerExpression.WithArguments( (fieldOrProperty.DeclarationKind, fieldOrProperty) ),
                            fieldOrProperty );

                        throw new DiagnosticErrorReportedException();
                    }
                }
                break;

            case false:

                if ( propertyTypeImplementsInpc )
                {
                    ctx.Builder.Diagnostics.Report(
                        DiagnosticDescriptors.NotifyPropertyChanged.ErrorFieldOrPropertyTypeIsStructImplementingINPC.WithArguments( (fieldOrProperty.DeclarationKind, fieldOrProperty, fieldOrProperty.Type) ),
                        fieldOrProperty );

                    throw new DiagnosticErrorReportedException();
                }
                break;
        }
    }

    private static void IntroduceUpdateMethods( BuildAspectContext ctx )
    {
        var allNodesDepthFirst = ctx.DependencyGraph.DecendantsDepthFirst().ToList();
        allNodesDepthFirst.Reverse();

        // Process all nodes in depth-first, leaf-to-root order, creating necessary update methods as we go.
        // The order is important, so that parent nodes test if child node methods were necessary and invoke them.
        
        // NB: We might be able do this with DeferredDeclaration<T> and care less about ordering.

        foreach ( var node in allNodesDepthFirst )
        {
            if ( node.Children.Count == 0 || node.Parent!.IsRoot )
            {
                // Leaf nodes and root properties should never have update methods.
                node.Data.SetUpdateMethod( null );
                continue;
            }

            ValidateFieldOrProperty( ctx, node.FieldOrProperty );

            IMethod? thisUpdateMethod = null;

            // Don't add fields and update methods for properties handled by base, or for root properties of the target type, or for properties of types that don't implement INPC.
            if ( node.Data.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit 
                && !ctx.HasInheritedOnChildPropertyChangedPropertyPath(node.DottedPropertyPath) )
            {
                var lastValueField = ctx.GetOrCreateLastValueField( node );
                var onPropertyChangedHandlerField = ctx.GetOrCreateHandlerField( node );

                var methodName = ctx.GetAndReserveUnusedMemberName( $"Update{node.ContiguousPropertyPath}" );

                var accessChildExprBuilder = new ExpressionBuilder();

#pragma warning disable CA1307 // Specify StringComparison for clarity [Justification: code must remain compatible with netstandard2.0]
                accessChildExprBuilder.AppendVerbatim( node.DottedPropertyPath.Replace( ".", "?." ) );
#pragma warning restore CA1307 // Specify StringComparison for clarity

                var accessChildExpression = accessChildExprBuilder.ToExpression();

                var introduceUpdateChildPropertyMethodResult = ctx.Builder.Advice.IntroduceMethod(
                    ctx.Target,
                    nameof( UpdateChildInpcProperty ),
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

            node.Data.SetUpdateMethod( thisUpdateMethod );
        }
    }

    private static void ProcessAutoProperties( BuildAspectContext ctx )
    {
        var target = ctx.Target;
        var typeOfInpc = (INamedType) TypeFactory.GetType( typeof( INotifyPropertyChanged ) );

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
            if ( p.IsVirtual )
            {
                ctx.Builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorVirtualMemberIsNotSupported.WithArguments( (p.DeclarationKind, p) ),
                    p );

                throw new DiagnosticErrorReportedException();
            }

            if ( p.IsNew )
            {
                ctx.Builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorNewMemberIsNotSupported.WithArguments( (p.DeclarationKind, p) ),
                    p );

                throw new DiagnosticErrorReportedException();
            }

            ValidateFieldOrProperty( ctx, p );

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

                        if ( hasDependentProperties )
                        {
                            handlerField = ctx.GetOrCreateHandlerField( node! );
                            ctx.PropertyPathsForOnChildPropertyChangedMethodAttribute.Add( p.Name );
                        }
                        else
                        {
                            ctx.PropertyNamesForOnUnmonitoredInpcPropertyChangedMethodAttribute.Add( p.Name );
                        }

                        ctx.Builder.Advice.Override( p, nameof( OverrideInpcRefTypeProperty ), tags: new
                        {
                            ctx,
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

                    var comparisonKind = p.Type is INamedType nt && nt.SpecialType != SpecialType.ValueTask_T
                        ? EqualityComparisonKind.EqualityOperator
                        : EqualityComparisonKind.DefaultEqualityComparer;

                    ctx.Builder.Advice.Override( p, nameof( OverrideUninstrumentedTypeProperty ), tags: new
                    {
                        ctx,
                        node,
                        compareUsing = comparisonKind,
                        propertyTypeInstrumentationKind
                    } );
                    break;
            }
        }
    }
}