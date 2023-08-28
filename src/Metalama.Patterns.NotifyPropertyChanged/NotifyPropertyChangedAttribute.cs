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
[Inheritable]
public sealed class NotifyPropertyChangedAttribute : Attribute, IAspect<INamedType>
{
    private static readonly string[] _onPropertyChangedMethodNames = { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged" };

    void IEligible<INamedType>.BuildEligibility( IEligibilityBuilder<INamedType> builder )
    {
        builder.MustNotBeStatic();
    }

    private enum InpcInstrumentationKind
    {
        None,
        Implicit,
        Explicit
    }

    private sealed class BuildAspectContext
    {
        private readonly Dictionary<IType, InpcInstrumentationKind> _inpcInstrumentationKindLookup = new();

        public BuildAspectContext( IAspectBuilder<INamedType> builder )
        {
            this.Builder = builder;
            this.Type_INotifyPropertyChanged = (INamedType) TypeFactory.GetType( typeof( INotifyPropertyChanged ) );
            this.Event_INotifyPropertyChanged_PropertyChanged = this.Type_INotifyPropertyChanged.Events.First();
            this.Type_PropertyChangedEventHandler = (INamedType) TypeFactory.GetType( typeof( PropertyChangedEventHandler ) );
            this.Type_IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof( IgnoreAutoChangeNotificationAttribute ) );
        }

        public IAspectBuilder<INamedType> Builder { get; }

        public INamedType Target => this.Builder.Target;

        public INamedType Type_INotifyPropertyChanged { get; }

        public IEvent Event_INotifyPropertyChanged_PropertyChanged { get; }

        public INamedType Type_PropertyChangedEventHandler { get; }

        public INamedType Type_IgnoreAutoChangeNotificationAttribute { get; }

        public IMethod OnPropertyChangedMethod { get; set; } = null!;

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

            InpcInstrumentationKind Check( IType type )
            {
                switch ( type )
                {
                    case INamedType namedType:
                        if ( namedType.Equals( this.Type_INotifyPropertyChanged ) )
                        {
                            return InpcInstrumentationKind.Implicit;
                        }
                        else if ( namedType.Is( this.Type_INotifyPropertyChanged ) )
                        {
                            if ( namedType.TryFindImplementationForInterfaceMember( this.Event_INotifyPropertyChanged_PropertyChanged, out var result ) )
                            {
                                return result.IsExplicitInterfaceImplementation ? InpcInstrumentationKind.Explicit : InpcInstrumentationKind.Implicit;
                            }

                            throw new InvalidOperationException();
                        }
                        else if ( namedType.Enhancements().HasAspect<NotifyPropertyChangedAttribute>() )
                        {
                            // For now, the aspect always introduces implicit implementation.
                            return InpcInstrumentationKind.Implicit;
                        }
                        else
                        {
                            return InpcInstrumentationKind.None;
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
    }

    void IAspect<INamedType>.BuildAspect( IAspectBuilder<INamedType> builder )
    {
        var ctx = new BuildAspectContext( builder );

        var target = builder.Target;

        var implementsInpc = target.Is( ctx.Type_INotifyPropertyChanged );

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
                },
                args: new
                {
                    propertyChangedEvent = implementInterfaceResult.InterfaceMembers.First().TargetMember
                } );

            onPropertyChangedMethod = introduceOnPropertyChangedMethodResult.Declaration;
        }

        ctx.OnPropertyChangedMethod = onPropertyChangedMethod;

        ProcessAutoProperties( ctx );
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
            switch ( p.Type.IsReferenceType )
            {
                case null:
                    throw new NotImplementedException( "Not implemented: unconstrained generic properties" );

                case true:

                    var propertyTypeInstrumentationKind = ctx.GetInpcInstrumentationKind( p.Type );

                    if ( propertyTypeInstrumentationKind is InpcInstrumentationKind.Implicit or InpcInstrumentationKind.Explicit )
                    {
                        // TODO: Requires dependency analysis.
                        // Do any properties depend on a child of this property?
                        var hasDependentProperties = true;

                        IField? handlerField = null;

                        if ( hasDependentProperties )
                        {
                            var handlerFieldName = GetAvailableFieldName( ctx.Target, $"_on{p.Name}PropertyChangedHandler" );

                            var introduceHandlerFieldResult = ctx.Builder.Advice.IntroduceField(
                                ctx.Target,
                                handlerFieldName,
                                ctx.Type_PropertyChangedEventHandler,
                                whenExists: OverrideStrategy.Fail );

                            handlerField = introduceHandlerFieldResult.Declaration;
                        }

                        ctx.Builder.Advice.Override( p, nameof( OverrideInpcRefTypeProperty ), tags: new
                        {
                            onPropertyChangedMethodHasCallerMemberNameAttribute,
                            onPropertyChangedMethod = ctx.OnPropertyChangedMethod,
                            handlerField,
                            ctx
                        } );
                    }
                    else
                    {
                        throw new NotImplementedException( "Not implemented: uninstrumented reference-type properties" );
                    }
                    break;

                case false:
                    ctx.Builder.Advice.Override( p, nameof( OverrideValueTypeProperty ), tags: new
                    {
                        onPropertyChangedMethodHasCallerMemberNameAttribute,
                        onPropertyChangedMethod = ctx.OnPropertyChangedMethod,
                    } );
                    break;
            }
        }
    }

    private static IMethod? GetOnPropertyChangedMethod( INamedType type )
        => type.AllMethods.FirstOrDefault( m =>
            !m.IsStatic
            && (type.IsSealed || m.Accessibility is Accessibility.Public or Accessibility.Protected)
            && m.ReturnType.SpecialType == SpecialType.Void
            && m.Parameters.Count == 1
            && m.Parameters[0].Type.SpecialType == SpecialType.String
            && _onPropertyChangedMethodNames.Contains( m.Name ) );

    private static string GetAvailableFieldName( INamedType type, string desiredFieldName )
    {
        string result;

        if ( !type.Fields.OfName( desiredFieldName ).Any() )
        {
            result = desiredFieldName;
        }
        else
        {
            for ( var i = 2; true; i++ )
            {
                result = $"{desiredFieldName}{i}";

                if ( !type.Fields.OfName( result ).Any() )
                {
                    break;
                }
            }
        }

        return result;
    }

    [Template]
    private static void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
    {
        // TODO: Supply a compile-time map of (relevant e.PropertyName) => (list of properties which depend on that e.PropertyName)
        meta.InsertComment( "Not implemented yet. Will a swich on e.PropertyName where cases call OnPropertyChanged for the affected properties." );
    }

    [Template]
    private static dynamic? OverrideInpcRefTypeProperty
    {
        set
        {
            var handlerField = (IField?) meta.Tags["handlerField"];
            var ctx = (BuildAspectContext) meta.Tags["ctx"]!;
            var eventRequiresCast = ctx.GetInpcInstrumentationKind( meta.Target.Property.Type ) is InpcInstrumentationKind.Explicit;

            if ( !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ) )
            {
                var oldValue = meta.Target.FieldOrProperty.Value;

                if ( handlerField != null )
                {
                    if ( oldValue != null )
                    {
                        if ( eventRequiresCast )
                        {
                            meta.Cast( ctx.Type_INotifyPropertyChanged, oldValue ).PropertyChanged -= handlerField.Value;
                        }
                        else
                        {
                            oldValue.PropertyChanged -= handlerField.Value;
                        }
                    }
                }

                meta.Target.FieldOrProperty.Value = value;

                var onPropertyChangedMethod = (IMethod) meta.Tags["onPropertyChangedMethod"]!;

                meta.Target.FieldOrProperty.Value = value;
                onPropertyChangedMethod.Invoke( meta.Target.Property.Name );

                if ( handlerField != null )
                {
                    if ( value != null )
                    {
                        handlerField.Value ??= (PropertyChangedEventHandler) OnSpecificPropertyChanged;

                        if ( eventRequiresCast )
                        {
                            meta.Cast( ctx.Type_INotifyPropertyChanged, value ).PropertyChanged += handlerField.Value;
                        }
                        else
                        {
                            value.PropertyChanged += handlerField.Value;
                        }
                    }
                }
            }

            // This must be implemented as a local function because Metalama does not currently support delegates in any other way.
            void OnSpecificPropertyChanged( object sender, PropertyChangedEventArgs e )
            {
                // TODO: Supply a compile-time map of (relevant e.PropertyName) => (list of properties which depend on that e.PropertyName)
                meta.InsertComment( "Not implemented yet. Will a swich on e.PropertyName where cases call OnPropertyChanged for the affected properties." );
            }
        }
    }

    [Template]
    private static dynamic? OverrideNonInpcRefTypeProperty
    {
        set
        {
            if ( !ReferenceEquals( value, meta.Target.FieldOrProperty.Value ) )
            {
                var onPropertyChangedMethod = (IMethod) meta.Tags["onPropertyChangedMethod"]!;

                meta.Target.FieldOrProperty.Value = value;
                onPropertyChangedMethod.Invoke( meta.Target.Property.Name );
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

                // TODO: IMethod.Invoke is not aware of/does not support default args, so we can't currently emit this idomatic expression.
                //       If/when supported, update other invocations, this is the only commented example.
#if true
                onPropertyChangedMethod.Invoke( meta.Target.Property.Name );
#else
                if ( false || (bool)meta.Tags["onPropertyChangedMethodHasCallerMemberNameAttribute"]! )
                {
                    // Emit human idomatic "OnPropertyChanged()" where the property name will be injected by the C# compiler thanks to [CallerMemberName] on
                    // the introduced or existing OnPropertyChanged method.
                    onPropertyChangedMethod.Invoke();
                }
                else
                {
                    onPropertyChangedMethod.Invoke( meta.Target.Member.Name );
                }
#endif
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