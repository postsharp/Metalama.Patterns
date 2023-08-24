using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Invokers;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using System.ComponentModel;
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
public sealed class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
{
    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();

#if false // Moved/will move to BuildAspect as per guidance at the end of this article: https://doc.metalama.net/conceptual/aspects/eligibility              
        builder.MustSatisfy(
            t => !t.Is( typeof( INotifyPropertyChanged ) ) || GetOnPropertyChangedMethod( t ) != null,
            _ => $"the class implements INotifyPropertyChanged but does not define an OnPropertyChanged method with the following signature: void OnPropertyChanged(string propertyName). The method name can also be NotifyOfPropertyChange or RaisePropertyChanged" );

        builder.MustSatisfy(
            t => t.Is( typeof( INotifyPropertyChanged ) ) || HasCompatibleEvent(t) != false,
            _ => $"the class has a defines a PropertyChanged event that is not compatible with INotifyPropertyChanged.PropertyChanged." );

        builder.MustSatisfy(
            t => t.Is( typeof( INotifyPropertyChanged ) ) || HasCompatibleEvent(t) != true || GetOnPropertyChangedMethod(t) != null,
            _ => $"the class has a defines a PropertyChanged event that is compatible with INotifyPropertyChanged.PropertyChanged, but does not define an OnPropertyChanged method with the following signature: void OnPropertyChanged(string propertyName). The method name can also be NotifyOfPropertyChange or RaisePropertyChanged." );

        builder.MustSatisfy(
            t => t.Is( typeof( INotifyPropertyChanged ) ) || HasCompatibleEvent( t ) != null || GetOnPropertyChangedMethod( t ) == null,
            _ => $"the class has a defines an OnPropertyChanged method with the expected signature but does not define a PropertyChanged event that is compatible with INotifyPropertyChanged.PropertyChanged." );

        /* TODO: Additional elibility checks:
         * - If class does not implement INPC, but does have an event named PropertyChanged:
         *     1. the event must be an auto event otherwise we can't raise it via IEvent.Raise. However, it's not possible
         *        to check this in ML at present, see TP #33665.
         */
#endif
    }

    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    private static IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault( m =>
            !m.IsStatic
            && m.Accessibility is Accessibility.Public or Accessibility.Protected
            && m.ReturnType.SpecialType == SpecialType.Void
            && m.Parameters.Count == 1
            && m.Parameters[0].Type.SpecialType == SpecialType.String
            && _onPropertyChangedMethodNames.Contains( m.Name ) );

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var target = builder.Target;
        var inpcType = (INamedType) TypeFactory.GetType( typeof( INotifyPropertyChanged ) );

        var implementsInpc = target.Is( inpcType );

        IMethod? onPropertyChangedMethod = null;

        if ( implementsInpc )
        {
            onPropertyChangedMethod = GetOnPropertyChangedMethod( target );

            if ( onPropertyChangedMethod == null )
            {
                builder.Diagnostics.Report(
                    DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassImplementsInpcButDoesNotDefineOnPropertyChanged
                    .WithArguments( target ) );

                return;
            }
        }
        else
        {
            var implementInterfaceResult = builder.Advice.ImplementInterface( target, inpcType, OverrideStrategy.Fail );

            var introduceOnPropertyChangedMethodResult = builder.Advice.IntroduceMethod(
                builder.Target,
                nameof( OnPropertyChanged ),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b => b.Accessibility = Accessibility.Protected,
                args: new
                {
                    propertyChangedEvent = implementInterfaceResult.InterfaceMembers.First().TargetMember
                } );

            onPropertyChangedMethod = introduceOnPropertyChangedMethodResult.Declaration;            
        }

        ProcessAutoProperties( builder, onPropertyChangedMethod );
    }

    private static void ProcessAutoProperties( IAspectBuilder<INamedType> builder, IMethod onPropertyChangedMethod )
    {
        var target = builder.Target;

        var onPropertyChangedMethodHasCallerMemberNameAttribute = onPropertyChangedMethod.Parameters[0].Attributes.Any( typeof( CallerMemberNameAttribute ) );

        // PS appears to consider all instance properties regardless of accessibility.
        var autoProperties = target.Properties.Where( p => !p.IsStatic && p.IsAutoPropertyOrField == true ).ToList();

        foreach ( var p in autoProperties )
        {
            switch ( p.Type.IsReferenceType )
            {
                case null:
                    throw new NotImplementedException( "Not implemented: unconstrained generic properties" );

                case true:
                    throw new NotImplementedException( "Not implemented: reference-type properties" );

                case false:
                    builder.Advice.Override( p, nameof( OverrideValueTypeProperty ), tags: new
                    {
                        onPropertyChangedMethodHasCallerMemberNameAttribute,
                        onPropertyChangedMethod
                    } );
                    break;
            }
            
        }
    }

    [Template]
    private static dynamic? OverrideValueTypeProperty
    {
        set
        {
            if ( value != meta.Target.FieldOrProperty.Value )
            {
                meta.Target.FieldOrProperty.Value = value;

                var onPropertyChangedMethod = (IMethod) meta.Tags["onPropertyChangedMethod"]!;

                if ( (bool)meta.Tags["onPropertyChangedMethodHasCallerMemberNameAttribute"]! )
                {
                    // Emit human idomatic "OnPropertyChanged()" where the property name will be injected by the C# compiler thanks to [CallerMemberName] on
                    // the introduced or existing OnPropertyChanged method.
                    onPropertyChangedMethod.Invoke();
                }
                else
                {
                    onPropertyChangedMethod.Invoke( meta.Target.Member.Name );
                }
            }
        }
    }

    // TODO: ML BUG? https://doc.metalama.net/conceptual/aspects/advising/implementing-interfaces says "The accessibility of introduced members is inconsequential.",
    //       but the aspect fails if this member is private.
    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;

    [Template]
    private static void OnPropertyChanged( [CompileTime] IEvent propertyChangedEvent, [CallerMemberName] string? propertyName = null )
    {
        propertyChangedEvent.With( InvokerOptions.NullConditional ).Raise( meta.This, new PropertyChangedEventArgs( propertyName ) );
    }
}

#if false // This code from BuildAspect imagined supporting adding the interface using exsting members for impl, which is not supported by ML (?)
    /// <summary>
    /// Called for types that don't implement INPC.
    /// </summary>
    /// <returns><see langword="true"/> if the type has a compatible event, <see langword="null"/> if the type has no
    /// event with the expected name, or <see langword="false"/> if the type has an incompatible event of the
    /// expected name.</returns>
    private static bool? HasCompatibleEvent( INamedType type )
    {
        // See also comment above re TP #33665.
        var ev = type.AllEvents.FirstOrDefault( e => e.Name == nameof( INotifyPropertyChanged.PropertyChanged ) );

        return ev?.Type.Is( typeof( PropertyChangedEventHandler ) );
    }

    Snippet from BuildAspect:

        {
            var propertyChangedEvent = target.AllEvents.FirstOrDefault( e => e.Name == nameof( INotifyPropertyChanged.PropertyChanged ) );

            if ( propertyChangedEvent != null )
            {
                if ( propertyChangedEvent.Accessibility != Accessibility.Public || !propertyChangedEvent.Type.Is( typeof( PropertyChangedEventHandler ) ) )
                {
                    builder.Diagnostics.Report(
                        DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassDefinesIncompatiblePropertyChangedEvent
                        .WithArguments( target ),
                        propertyChangedEvent );

                    return;
                }

                onPropertyChangedMethod = GetOnPropertyChangedMethod( target );

                if ( onPropertyChangedMethod == null )
                {
                    builder.Diagnostics.Report(
                        DiagnosticDescriptors.NotifyPropertyChanged.ErrorClassDefinesEventButDoesNotDefineOnPropertyChanged
                        .WithArguments( target ) );

                    return;
                }
            }
            else
            {
                var introducePropertyChangedEventResult = builder.Advice.IntroduceEvent(
                    builder.Target,
                    nameof( PropertyChanged ),
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b => b.Accessibility = Accessibility.Public );

                propertyChangedEvent = introducePropertyChangedEventResult.Declaration;

                var introduceOnPropertyChangedMethodResult = builder.Advice.IntroduceMethod(
                    builder.Target,
                    nameof( OnPropertyChanged ),
                    IntroductionScope.Instance,
                    OverrideStrategy.Fail,
                    b => b.Accessibility = Accessibility.Protected );

                onPropertyChangedMethod = introduceOnPropertyChangedMethodResult.Declaration;
            }        
        }    
#endif