using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Collections;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Engine.CodeModel;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

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
            AnalyzeBaseAndIntroduceInterfaceIfRequired( ctx );
            
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
                    b.Name = ctx.BaseOnPropertyChangedMethod.Name;
                }

                if ( ctx.Target.IsSealed )
                {
                    b.Accessibility = Accessibility.Private;
                }
                else
                {
                    b.Accessibility = Accessibility.Protected;
                    b.IsVirtual = true;
                }
            },
            args: new
            {
                ctx
            } );

        ctx.OnPropertyChangedMethod.Declaration = result.Declaration;
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
                    b.Accessibility = Accessibility.Private;
                }
                else
                {
                    b.Accessibility = Accessibility.Protected;
                    b.IsVirtual = true;
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
                    b.Name = ctx.BaseOnUnmonitoredInpcPropertyChangedMethod.Name;
                }

                if ( ctx.Target.IsSealed )
                {
                    b.Accessibility = Accessibility.Private;
                }
                else
                {
                    b.Accessibility = Accessibility.Protected;
                    b.IsVirtual = true;
                }
            },
            args: new
            {
                ctx
            } );

        ctx.OnUnmonitoredInpcPropertyChangedMethod.Declaration = result.Declaration;
    }

    private static void AnalyzeBaseAndIntroduceInterfaceIfRequired( BuildAspectContext ctx )
    {
        var builder = ctx.Builder;
        var target = builder.Target;

        ctx.BaseOnPropertyChangedMethod = GetOnPropertyChangedMethod( target );
        ctx.BaseOnChildPropertyChangedMethod = GetOnChildPropertyChangedMethod( target );
        ctx.BaseOnUnmonitoredInpcPropertyChangedMethod = GetOnUnmonitoredInpcPropertyChangedMethod( ctx, target );

        if ( ctx.TargetImplementsInpc )
        {
            if ( ctx.BaseOnPropertyChangedMethod == null )
            {
                builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( target ) );

                throw new DiagnosticErrorReportedException();
            }
        }
        else
        {
            builder.Advice.ImplementInterface( target, ctx.Type_INotifyPropertyChanged, OverrideStrategy.Fail );
        }

        ctx.InheritedOnChildPropertyChangedPropertyPaths = 
            BuildPropertyPathLookup( GetPropertyPaths( ctx.Type_OnChildPropertyChangedMethodAttribute, ctx.BaseOnChildPropertyChangedMethod ) );

        ctx.InheritedOnUnmonitoredInpcPropertyChangedPropertyPaths = 
            BuildPropertyNameLookup( GetPropertyPaths( ctx.Type_OnUnmonitoredInpcPropertyChangedMethodAttribute, ctx.BaseOnUnmonitoredInpcPropertyChangedMethod ) );

        static HashSet<string>? BuildPropertyNameLookup( IEnumerable<string>? propertyNames )
        {
            if ( propertyNames == null )
            {
                return null;
            }

            var h = new HashSet<string>();

            foreach ( var s in propertyNames )
            {
                if ( s.IndexOf('.') > -1 )
                {
                    throw new ArgumentException( "A property name must not contain period characters.", nameof( propertyNames ) );
                }
                h.Add( s );
            }

            return h;
        }

        // NOTE: This hashset of path stems approach is a simple way to allow path stems to be checked, but
        // a tree-based structure might scale better if required. Keeping it simple for now.
        static HashSet<string>? BuildPropertyPathLookup( IEnumerable<string>? propertyPaths )
        {
            if ( propertyPaths == null )
            {
                return null;
            }

            var h = new HashSet<string>();
            
            foreach ( var s in propertyPaths )
            {
                AddPropertyPathAndAllAncestorStems( h, s );
            }
            
            return h;
        }

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

                IMethod? thisUpdateMethod = null;

                // Don't add fields and update methods for properties handled by base, or for root properties of the target type, or for properties of types that don't implement INPC.
                if ( !nodeToProcess.Parent!.IsRoot 
                    && nodeToProcess.Data.PropertyTypeInpcInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit 
                    && !ctx.HasInheritedOnChildPropertyChangedPropertyPath(nodeToProcess.Data.DottedPropertyPath) )
                {
                    var pathForMemberNames = nodeToProcess.Data.ContiguousPropertyPath;
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

                    accessChildExprBuilder.AppendVerbatim( nodeToProcess.Data.DottedPropertyPath.Replace( ".", "?." ) );

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

    private static IMethod? GetOnUnmonitoredInpcPropertyChangedMethod( BuildAspectContext ctx, INamedType type )
        => type.AllMethods.FirstOrDefault( m =>
            !m.IsStatic
            && m.Attributes.Any( typeof( OnUnmonitoredInpcPropertyChangedMethodAttribute ) )
            && (type.IsSealed || m.Accessibility is Accessibility.Public or Accessibility.Protected)
            && m.ReturnType.SpecialType == SpecialType.Void
            && m.Parameters.Count == 3
            && m.Parameters[0].Type.SpecialType == SpecialType.String
            && m.Parameters[1].Type == ctx.Type_Nullable_INotifyPropertyChanged
            && m.Parameters[2].Type == ctx.Type_Nullable_INotifyPropertyChanged );

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
}