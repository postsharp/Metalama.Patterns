// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Configuration;
using System.ComponentModel;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

internal sealed class ClassicDesignTimeObservabilityStrategyImpl : DesignTimeObservabilityStrategy
{
    private readonly IMethod? _baseOnPropertyChangedMethod;
    private readonly IMethod? _baseOnChildPropertyChangedMethod;
    private readonly IMethod? _baseOnObservablePropertyChangedMethod;

    private IAspectBuilder<INamedType> Builder { get; }

    public ClassicDesignTimeObservabilityStrategyImpl( IAspectBuilder<INamedType> builder )
    {
        this.Builder = builder;

        var target = builder.Target;
        var elements = builder.Target.Compilation.Cache.GetOrAdd( _ => new Assets() );
        this._baseOnPropertyChangedMethod = ClassicObservabilityStrategyImpl.GetOnPropertyChangedMethod( target );
        this._baseOnChildPropertyChangedMethod = ClassicObservabilityStrategyImpl.GetOnChildPropertyChangedMethod( target );

        this._baseOnObservablePropertyChangedMethod =
            ClassicObservabilityStrategyImpl.GetOnObservablePropertyChangedMethod( target, elements );
    }

    public override void BuildAspect( IAspectBuilder<INamedType> builder )
    {
        base.BuildAspect( builder );

        // Also introduce any methods which might be introduced by the full builder which might reasonably be observed by other code.

        this.IntroduceOnPropertyChangedMethod();
        this.IntroduceOnChildPropertyChangedMethod();
        this.IntroduceOnObservablePropertyChanged();
    }

    private void IntroduceOnPropertyChangedMethod()
    {
        var isOverride = this._baseOnPropertyChangedMethod != null;

        this.Builder.Advice.WithTemplateProvider( this )
            .IntroduceMethod(
                this.Builder.Target,
                nameof(OnPropertyChanged),
                IntroductionScope.Instance,
                isOverride ? OverrideStrategy.Override : OverrideStrategy.Ignore,
                b =>
                {
                    if ( isOverride )
                    {
                        b.Name = this._baseOnPropertyChangedMethod!.Name;
                    }

                    if ( this.Builder.Target.IsSealed )
                    {
                        b.Accessibility = isOverride ? this._baseOnPropertyChangedMethod!.Accessibility : Accessibility.Private;
                    }
                    else
                    {
                        b.Accessibility = isOverride ? this._baseOnPropertyChangedMethod!.Accessibility : Accessibility.Protected;
                        b.IsVirtual = !isOverride;
                    }
                } );
    }

    private void IntroduceOnChildPropertyChangedMethod()
    {
        var isOverride = this._baseOnChildPropertyChangedMethod != null;

        this.Builder.Advice.WithTemplateProvider( this )
            .IntroduceMethod(
                this.Builder.Target,
                nameof(OnChildPropertyChanged),
                IntroductionScope.Instance,
                isOverride ? OverrideStrategy.Override : OverrideStrategy.Ignore,
                b =>
                {
                    if ( isOverride )
                    {
                        b.Name = this._baseOnChildPropertyChangedMethod!.Name;
                    }

                    if ( this.Builder.Target.IsSealed )
                    {
                        b.Accessibility = isOverride ? this._baseOnChildPropertyChangedMethod!.Accessibility : Accessibility.Private;
                    }
                    else
                    {
                        b.Accessibility = isOverride ? this._baseOnChildPropertyChangedMethod!.Accessibility : Accessibility.Protected;
                        b.IsVirtual = !isOverride;
                    }
                } );
    }

    private void IntroduceOnObservablePropertyChanged()
    {
        if ( !this.Builder.Target.Enhancements().GetOptions<ClassicObservabilityStrategyOptions>().EnableOnObservablePropertyChangedMethod
             == true )
        {
            return;
        }

        var isOverride = this._baseOnObservablePropertyChangedMethod != null;

        this.Builder.Advice.WithTemplateProvider( this )
            .IntroduceMethod(
                this.Builder.Target,
                nameof(OnObservablePropertyChanged),
                IntroductionScope.Instance,
                isOverride ? OverrideStrategy.Override : OverrideStrategy.Ignore,
                b =>
                {
                    if ( isOverride )
                    {
                        b.Name = this._baseOnObservablePropertyChangedMethod!.Name;
                    }

                    if ( this.Builder.Target.IsSealed )
                    {
                        b.Accessibility = isOverride ? this._baseOnObservablePropertyChangedMethod!.Accessibility : Accessibility.Private;
                    }
                    else
                    {
                        b.Accessibility = isOverride ? this._baseOnObservablePropertyChangedMethod!.Accessibility : Accessibility.Protected;
                        b.IsVirtual = !isOverride;
                    }
                } );
    }

    [UsedImplicitly]
    [Template]
    private static void OnPropertyChanged( string propertyName ) { }

    [UsedImplicitly]
    [Template]
    internal static void OnChildPropertyChanged( string parentPropertyPath, string propertyName ) { }

    [UsedImplicitly]
    [Template]
    internal static void OnObservablePropertyChanged( string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue ) { }
}