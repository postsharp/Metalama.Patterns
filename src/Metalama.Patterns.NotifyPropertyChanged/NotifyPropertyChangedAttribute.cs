using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;
using System.Diagnostics;
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

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        Debugger.Break();
        var ctx = new BuildAspectContext( builder );

        try
        {
            FindOrIntroducePropertyChangedMethod( ctx );
            IntroduceUpdateMethods( ctx );
            ProcessAutoProperties( ctx );
        }
        catch ( DiagnosticErrorReportedException )
        {
            // Diagnostic already raised, do nothing.
        }
    }

    private static void FindOrIntroducePropertyChangedMethod( BuildAspectContext ctx )
    {
        var builder = ctx.Builder;
        var target = builder.Target;

        IMethod? onPropertyChangedMethod = null;

        if ( ctx.TargetImplementsInpc )
        {
            onPropertyChangedMethod = GetOnPropertyChangedMethod( target );

            if ( onPropertyChangedMethod == null )
            {
                builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( target ) );

                throw new DiagnosticErrorReportedException();
            }
        }
        else
        {
            var implementInterfaceResult = builder.Advice.ImplementInterface( target, ctx.Type_INotifyPropertyChanged, OverrideStrategy.Fail );

            var introduceOnPropertyChangedMethodResult = builder.Advice.IntroduceMethod(
                builder.Target,
                nameof( OnPropertyChanged ),
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
    }

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

    /// <summary>
    /// Introduces discrete change methods.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="onChangedMethodName"></param>
    /// <param name="onChildChangedMethodName">
    /// The name to use for the <c>OnChildChangedMethod</c>, or <see langword="null"/> if the
    /// <c>OnChildChangedMethod</c> should not be introduced.
    /// </param>
    /// <param name="node">
    /// The graph node, when defined. Nodes are not defined for root properties of the target type which have no references.
    /// </param>
    /// <param name="propertyName">
    /// If <paramref name="node"/> is not specified, this implies that the change methods are being introduced for a root
    /// property of the target type, in which case <paramref name="propertyName"/> must be the name of the property.
    /// </param>
    /// <param name="propertyPathForMetadataAttributesOrNullIfIsOverride">
    /// The property path to use with introduced <see cref="Metadata.OnChangedAttribute"/> and <see cref="Metadata.OnChildChangedAttribute"/>;
    /// or <see langword="null"/> if metadata attributes should not be introduced because the method(s) will be derived overrides.
    /// </param>
    /// <returns></returns>
    [Obsolete("To be removed.", true)]
    private static (IMethod OnChangedMethod, IMethod? OnChildChangedMethod) IntroduceDiscreteChangeMethods( 
        BuildAspectContext ctx,         
        string onChangedMethodName, 
        string? onChildChangedMethodName,
        DependencyGraph.Node<NodeData>? node,
        string? propertyName,
        string? propertyPathForMetadataAttributesOrNullIfIsOverride )
    {
        if ( node == null && propertyName == null )
        {
            throw new ArgumentException( $"At least one of {nameof( node )} and {nameof( propertyName )} must be specified." );
        }

        var introduceOnChangedMethodResult = ctx.Builder.Advice.IntroduceMethod(
            ctx.Target,
            nameof( GenerateOnChangedBody ),
            IntroductionScope.Instance,
            OverrideStrategy.Override,
            b =>
            {
                b.Name = onChangedMethodName;
                b.Accessibility = Accessibility.Protected;
                b.IsVirtual = !ctx.Target.IsSealed;

                if ( propertyPathForMetadataAttributesOrNullIfIsOverride != null )
                {
                    b.AddAttribute( AttributeConstruction.Create( ctx.Type_OnChangedAttribute, new[] { propertyPathForMetadataAttributesOrNullIfIsOverride } ) );
                }
            },
            args: new
            {
                ctx,
                node,
                propertyName,
                disableNotifySelfChanged = propertyPathForMetadataAttributesOrNullIfIsOverride == null,
                proceedAtEnd = true
            } );
        
        IMethod? onChildChangedMethod = null;

        if ( onChildChangedMethodName != null )
        {
            var introduceOnChildChangedMethodResult = ctx.Builder.Advice.IntroduceMethod(
                ctx.Target,
                nameof( GenerateOnChildChangedOverride ),
                IntroductionScope.Instance,
                OverrideStrategy.Override,
                b =>
                {
                    b.Name = onChildChangedMethodName;
                    b.Accessibility = Accessibility.Protected;
                    b.IsVirtual = !ctx.Target.IsSealed;

                    if ( propertyPathForMetadataAttributesOrNullIfIsOverride != null )
                    {
                        b.AddAttribute( AttributeConstruction.Create( ctx.Type_OnChildChangedAttribute, new[] { propertyPathForMetadataAttributesOrNullIfIsOverride } ) );
                    }
                },
                args: new
                {
                    ctx,
                    node
                } );

            onChildChangedMethod = introduceOnChildChangedMethodResult.Declaration;
        }

        return (introduceOnChangedMethodResult.Declaration, onChildChangedMethod);
    }

    private static void IntroduceUpdateMethods( BuildAspectContext ctx )
    {
        var allNodesDepthFirst = ctx.DependencyGraph.DecendantsDepthFirst().ToList();
        allNodesDepthFirst.Reverse();

        /* Iterate all nodes (except root), depth-first, in leaf-to-root order (this is important).
         * For each node that is directly referenced, we then make sure to build the
         * update method for the *parent* - this is because the parent object will
         * notify changes to the child that we are considering.
         */

        // TODO: I suspect/know some scenarios are not handled yet. Don't assume the logic here is correct or complete.

        foreach ( var node in allNodesDepthFirst )
        {
            var nodeToProcess = node.Parent!;

            if ( nodeToProcess.IsRoot )
            {
                // *node* is a root property of the target type. Always consider processing these, in case
                // they have not been processed already.
                nodeToProcess = node;
            }

            /*
            if ( nodeToProcess.Parent!.IsRoot )
            {
                // *parent* is a root property of the target type. Do we need to do anything for it?
                continue;
            }
            */

            if ( !nodeToProcess.Data.MethodsHaveBeenSet )
            {
                var propertyDetails = ValidateFieldOrProperty( ctx, nodeToProcess.Data.FieldOrProperty );

                // eg, for node X.Y.Z, parentElementNames is [X,Y]
                var parentElementNames = nodeToProcess.AncestorsAndSelf().Reverse().Select( n => n.Symbol.Name ).ToList();

                var pathForMemberNames = string.Join( "", parentElementNames );
                var pathForMetadataLookup = string.Join( ".", parentElementNames );

                IMethod? thisUpdateMethod = null;

                // Don't add fields and update methods for properties handled by base, or for root properties of the target type, or for properties of types that don't implement INPC.
                if ( !nodeToProcess.Parent!.IsRoot 
                    && nodeToProcess.Data.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit )
                {
                    var lastValueFieldName = ctx.GetAndReserveUnusedMemberName( $"_last{pathForMemberNames}" );

                    var introduceLastValueFieldResult = ctx.Builder.Advice.IntroduceField(
                        ctx.Target,
                        lastValueFieldName,
                        nodeToProcess.Data.FieldOrProperty.Type.ToNullableType(),
                        IntroductionScope.Instance,
                        OverrideStrategy.Fail,
                        b => b.Accessibility = Accessibility.Private );

                    var lastValueField = introduceLastValueFieldResult.Declaration;

                    var onPropertyChangedHandlerFieldName = ctx.GetAndReserveUnusedMemberName( $"_on{pathForMemberNames}ChangedHandler" );

                    var introduceOnPropertyChangedHandlerFieldName = ctx.Builder.Advice.IntroduceField(
                        ctx.Target,
                        onPropertyChangedHandlerFieldName,
                        ctx.Type_Nullable_PropertyChangedEventHandler,
                        IntroductionScope.Instance,
                        OverrideStrategy.Fail,
                        b => b.Accessibility = Accessibility.Private );

                    var onPropertyChangedHandlerField = introduceOnPropertyChangedHandlerFieldName.Declaration;

                    var methodName = ctx.GetAndReserveUnusedMemberName( $"Update{pathForMemberNames}" );

                    var accessChildExprBuilder = new ExpressionBuilder();

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
                            b.Accessibility = Accessibility.Private;
                        },
                        args: new
                        {
                            ctx,
                            node = nodeToProcess,
                            accessChildExpression,
                            lastValueField,
                            onPropertyChangedHandlerField
                        } );

                    thisUpdateMethod = introduceUpdateChildPropertyMethodResult.Declaration;
                }

                nodeToProcess.Data.SetMethods( thisUpdateMethod );
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
                            var handlerFieldName = ctx.GetAndReserveUnusedMemberName( $"_on{p.Name}PropertyChangedHandler" );

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

    private static IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault( m =>
            !m.IsStatic
            && (type.IsSealed || m.Accessibility is Accessibility.Public or Accessibility.Protected)
            && m.ReturnType.SpecialType == SpecialType.Void
            && m.Parameters.Count == 1
            && m.Parameters[0].Type.SpecialType == SpecialType.String
            && _onPropertyChangedMethodNames.Contains( m.Name ) );
}