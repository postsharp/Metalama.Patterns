﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Implementation.DesignTimeStrategy;
using Metalama.Patterns.Observability.Options;
using System.ComponentModel;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

internal sealed class ClassicDesignTimeImplementationStrategyBuilder : DesignTimeImplementationStrategyBuilder
{
    private readonly IMethod? _baseOnPropertyChangedMethod;
    private readonly IMethod? _baseOnChildPropertyChangedMethod;
    private readonly IMethod? _baseOnObservablePropertyChangedMethod;

    public ClassicDesignTimeImplementationStrategyBuilder( IAspectBuilder<INamedType> builder ) : base( builder )
    {
        var target = builder.Target;
        var elements = builder.Target.Compilation.Cache.GetOrAdd( _ => new Assets() );
        this._baseOnPropertyChangedMethod = ClassicImplementationStrategyBuilder.GetOnPropertyChangedMethod( target );
        this._baseOnChildPropertyChangedMethod = ClassicImplementationStrategyBuilder.GetOnChildPropertyChangedMethod( target );

        this._baseOnObservablePropertyChangedMethod =
            ClassicImplementationStrategyBuilder.GetOnObservablePropertyChangedMethod( target, elements );
    }

    protected override void BuildAspect()
    {
        base.BuildAspect();

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
        if ( !this.Builder.Target.Enhancements().GetOptions<ClassicImplementationStrategyOptions>().EnableOnObservablePropertyChangedMethod
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