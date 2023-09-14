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

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        Debugger.Break();
        var ctx = new BuildAspectContext( builder );

        try
        {
            ExamineBaseAndIntroduceInterfaceIfRequired( ctx );
            
            // Introduce methods like UpdateA1B1()
            IntroduceUpdateMethods( ctx );

            // Override auto properties
            ProcessAutoProperties( ctx );

            IntroduceOnPropertyChangedMethod( ctx );
            IntroduceOnChildPropertyChangedMethod( ctx );
            IntroduceOnUnmonitoredInpcPropertyChanged( ctx );
        }
        catch ( DiagnosticErrorReportedException )
        {
            // Diagnostic already raised, do nothing.
        }
    }
    
    // TODO: Work in progress!
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
                    b.Name = ctx.BaseOnChildPropertyChangedMethod.Name;
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
            ctx.Builder.Advice.ImplementInterface( ctx.Target, ctx.Type_INotifyPropertyChanged, OverrideStrategy.Fail );
        }
    }

    // TODO: Return value is currently unused.
    private static (bool OnChangedMethodIsApplicable, bool OnChildChangedMethodIsApplicable) ValidateFieldOrProperty(
        BuildAspectContext ctx,
        IFieldOrProperty fieldOrProperty )
    {
        var propertyTypeImplementsInpc = ctx.GetInpcInstrumentationKind( fieldOrProperty.Type ) is InpcInstrumentationKind.Explicit or InpcInstrumentationKind.Implicit;

        var onChangedMethodIsApplicable = false;
        var onChildChangedMethodIsApplicable = false;

        switch ( fieldOrProperty.Type.IsReferenceType )
        {
            case null:
                // This might require INPC-type code which is used at runtime only when T implements INPC,
                // and non-INPC-type code which is used at runtime when T does not implement INPC.                    
                throw new NotSupportedException( "Unconstrained generic properties are not supported." );

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

                    onChangedMethodIsApplicable = true;
                    onChildChangedMethodIsApplicable = true;
                }
                else
                {
                    onChangedMethodIsApplicable = true;
                }
                break;

            case false:

                if ( propertyTypeImplementsInpc )
                {
                    // TODO: Proper error reporting.
                    throw new NotSupportedException( "structs implementing INotifyPropertyChanged are not supported." );
                }
                onChangedMethodIsApplicable = true;
                break;
        }

        return (onChangedMethodIsApplicable, onChildChangedMethodIsApplicable);
    }

    private static void IntroduceUpdateMethods( BuildAspectContext ctx )
    {
        var allNodesDepthFirst = ctx.DependencyGraph.DecendantsDepthFirst().ToList();
        allNodesDepthFirst.Reverse();

        // Process all nodes in depth-first, leaf-to-root order, creating necessary update methods as we go.
        // The order is important, so that parent nodes test if child node methods were necessary and invoke them.
        
        // NB: We might be able do this with DeferredDeclaration<T> and care less about ordering.

        // TODO: I suspect/know some scenarios are not handled yet. Don't assume the logic here is correct or complete.

        foreach ( var node in allNodesDepthFirst )
        {
            if ( node.Children.Count == 0 || node.Parent!.IsRoot )
            {
                // Leaf nodes and root properties should never have update methods.
                node.Data.SetUpdateMethod( null );
                continue;
            }

            // TODO: Remove this check, just confirming something.
            if ( node.Data.UpdateMethodHasBeenSet )
            {
                throw new InvalidOperationException( "Why???" );
            }

            _ = ValidateFieldOrProperty( ctx, node.Data.FieldOrProperty );

            IMethod? thisUpdateMethod = null;

            // Don't add fields and update methods for properties handled by base, or for root properties of the target type, or for properties of types that don't implement INPC.
            if ( //!node.Parent!.IsRoot &&
                node.Data.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit 
                && !ctx.HasInheritedOnChildPropertyChangedPropertyPath(node.Data.DottedPropertyPath) )
            {
                var lastValueField = ctx.GetOrCreateLastValueField( node );
                var onPropertyChangedHandlerField = ctx.GetOrCreateHandlerField( node );

                var methodName = ctx.GetAndReserveUnusedMemberName( $"Update{node.Data.ContiguousPropertyPath}" );

                var accessChildExprBuilder = new ExpressionBuilder();

                accessChildExprBuilder.AppendVerbatim( node.Data.DottedPropertyPath.Replace( ".", "?." ) );

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
                        node = node,
                        accessChildExpression,
                        lastValueField,
                        onPropertyChangedHandlerField
                    } );

                thisUpdateMethod = introduceUpdateChildPropertyMethodResult.Declaration;
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
                // TODO: Proper error reporting.
                throw new NotSupportedException( "Virtual properties are not supported." );
            }

            if ( p.IsNew )
            {
                // TODO: Proper error reporting.
                throw new NotSupportedException( "'new' properties are not supported." );
            }

            var propertyDetails = ValidateFieldOrProperty( ctx, p );

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

                            // TODO: Is this the right place to do this, or should it be in GetOrCreateHandlerField?
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

    /// <summary>
    /// Introduces a 
    /// </summary>
    /// <param name="ctx"></param>
    private static void IntroduceInitializerMethod( BuildAspectContext ctx )
    {

    }
}