// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.ClassicStrategy;

internal class ClassicDesignTimeImplementationStrategyBuilder : DesignTimeStrategy.DesignTimeImplementationStrategyBuilder
{
    private readonly Elements _elements;
    private readonly IMethod? _baseOnPropertyChangedMethod;
    private readonly IMethod? _baseOnChildPropertyChangedMethod;
    private readonly IMethod? _baseOnUnmonitoredObservablePropertyChangedMethod;

    public ClassicDesignTimeImplementationStrategyBuilder( IAspectBuilder<INamedType> builder ) : base( builder )
    {
        var target = builder.Target;
        this._elements = new Elements( target );
        this._baseOnPropertyChangedMethod = ClassicImplementationStrategyBuilder.GetOnPropertyChangedMethod( target );
        this._baseOnChildPropertyChangedMethod = ClassicImplementationStrategyBuilder.GetOnChildPropertyChangedMethod( target );
        this._baseOnUnmonitoredObservablePropertyChangedMethod = ClassicImplementationStrategyBuilder.GetOnUnmonitoredObservablePropertyChangedMethod( target, this._elements );
    }

    protected override void BuildAspect()
    {
        base.BuildAspect();

        // Also introduce any methods which might be introduced by the full builder which might reasonably be observed by other code.

        this.IntroduceOnPropertyChangedMethod();
        this.IntroduceOnChildPropertyChangedMethod();
        this.IntroduceOnUnmonitoredObservablePropertyChanged();
    }

    private void IntroduceOnPropertyChangedMethod()
    {
        var isOverride = this._baseOnPropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( this ).IntroduceMethod(
            this._builder.Target,
            nameof( OnPropertyChanged ),
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
            } );
    }

    private void IntroduceOnChildPropertyChangedMethod()
    {
        var isOverride = this._baseOnChildPropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( this ).IntroduceMethod(
            this._builder.Target,
            nameof( OnChildPropertyChanged ),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
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
            } );
    }

    private void IntroduceOnUnmonitoredObservablePropertyChanged()
    {
        if ( !this._builder.Target.Enhancements().GetOptions<Options.ClassicImplementationStrategyOptions>().EnableOnUnmonitoredObservablePropertyChangedMethod == true )
        {
            return;
        }

        var isOverride = this._baseOnUnmonitoredObservablePropertyChangedMethod != null;

        var result = this._builder.Advice.WithTemplateProvider( this ).IntroduceMethod(
            this._builder.Target,
            nameof( OnUnmonitoredObservablePropertyChanged ),
            IntroductionScope.Instance,
            isOverride ? OverrideStrategy.Override : OverrideStrategy.Fail,
            b =>
            {
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
            } );
    }

    [Template]
    private static void OnPropertyChanged( string propertyName ) { }

    [Template]
    internal static void OnChildPropertyChanged( string parentPropertyPath, string propertyName ) { }

    [Template]
    internal static void OnUnmonitoredObservablePropertyChanged( string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue ) { }
}