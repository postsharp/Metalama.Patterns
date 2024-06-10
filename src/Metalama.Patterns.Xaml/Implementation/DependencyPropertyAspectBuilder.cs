﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Project;
using Metalama.Patterns.Xaml.Configuration;
using Metalama.Patterns.Xaml.Implementation.DependencyPropertyNamingConvention;
using Metalama.Patterns.Xaml.Implementation.NamingConvention;
using System.Windows;
using MetalamaAccessibility = Metalama.Framework.Code.Accessibility;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal sealed partial class DependencyPropertyAspectBuilder
{
    internal const string RegistrationFieldCategory = "registration field";
    internal const string PropertyChangedMethodCategory = "property-changed method";
    internal const string ValidateMethodCategory = "validate method";

    private readonly IAspectBuilder<IProperty> _builder;
    private readonly DependencyPropertyAttribute _attribute;
    private readonly DependencyPropertyOptions _options;

    public DependencyPropertyAspectBuilder( IAspectBuilder<IProperty> builder, DependencyPropertyAttribute attribute )
    {
        this._builder = builder;
        this._attribute = attribute;
        this._options = builder.Target.Enhancements().GetOptions<DependencyPropertyOptions>();
    }

    public void Build()
    {
        var builder = this._builder;
        var target = builder.Target;
        var propertyType = builder.Target.Type;
        var declaringType = target.DeclaringType;
        var options = target.Enhancements().GetOptions<DependencyPropertyOptions>();

        var hasExplicitNaming = this._attribute.PropertyChangedMethod != null
                                || this._attribute.ValidateMethod != null;

        var namingConventions = hasExplicitNaming
            ?
            [
                new ExplicitDependencyPropertyNamingConvention(
                    this._attribute.RegistrationField,
                    this._attribute.PropertyChangedMethod,
                    this._attribute.ValidateMethod )
            ]
            : options.GetSortedNamingConventions();

        var diagnosticReporter = new DiagnosticReporter( builder );

        if ( !NamingConventionEvaluator.TryEvaluate( namingConventions, target, diagnosticReporter, out var match ) )
        {
            // No match.
            return;
        }

        // NB: WPF convention requires a specific field name, so we don't try to find an unused name.

        IIntroductionAdviceResult<IField>? introduceRegistrationFieldResult = null;

        if ( match?.RegistrationFieldConflictMatch.Outcome == MemberMatchOutcome.Success )
        {
            introduceRegistrationFieldResult = this._builder.Advice.IntroduceField(
                declaringType,
                match.RegistrationFieldName!,
                typeof(DependencyProperty),
                IntroductionScope.Static,
                OverrideStrategy.Fail,
                b =>
                {
                    // ReSharper disable once RedundantNameQualifier
                    b.Accessibility = MetalamaAccessibility.Public;
                    b.Writeability = Writeability.ConstructorOnly;
                } );
        }

        var onChangedMethod = match?.PropertyChangedMatch.Member;
        var validateMethod = match?.ValidateMatch.Member;

        if ( !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            if ( onChangedMethod != null )
            {
                this._builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, onChangedMethod );
            }

            if ( validateMethod != null )
            {
                this._builder.Diagnostics.Suppress( Suppressions.SuppressRemoveUnusedPrivateMembersIDE0051, validateMethod );
            }

            return;
        }

        if ( match == null || introduceRegistrationFieldResult is not { Outcome: AdviceOutcome.Default } )
        {
            // We cannot proceed with other transformations if there was no naming convention match or
            // we could not introduce the DependencyProperty field.

            return;
        }

        // TODO: #34041 - Replace with target.Enhancements().HasAspect<ContractAspect>() once HasAspect supports base types.

        var hasContracts = target.Enhancements().GetAspectInstances().Any( a => typeof(ContractAspect).IsAssignableFrom( a.AspectClass.Type ) );

        IMethod? applyContractsMethod = null;

        if ( hasContracts )
        {
            var name = this.GetAndReserveUnusedMemberName( $"Apply{builder.Target.Name}Contracts" );

            var result = builder.Advice.WithTemplateProvider( Templates.Provider )
                .IntroduceMethod(
                    declaringType,
                    nameof(Templates.ApplyContracts),
                    IntroductionScope.Static,
                    OverrideStrategy.Fail,
                    b =>
                    {
                        b.Name = name;
                        b.Accessibility = MetalamaAccessibility.Private;
                    },
                    new { T = propertyType } );

            if ( result.Outcome != AdviceOutcome.Default )
            {
                return;
            }

            applyContractsMethod = result.Declaration;

            ContractAspect.RedirectContracts( this._builder, target, applyContractsMethod.Parameters[0] );
        }

        /* Regarding setting the initial value, the PostSharp implementation takes care to:
         *
         * - Only set the initial value when definitely supplied via an initializer and not when supplied via the DefaultValue property of the aspect.
         * - Set the initial value using DependencyObject.SetCurrentValue ("otherwise we override value coming from templates").
         * - Suspend enforcement of contracts and explicit validation around that call.
         *
         * See also https://tp.postsharp.net/entity/26136-dependencyproperty-values-are-not-set-when
         *
         * TG: My understanding from a Metalama perspective is that we have easy access and control of the initializer expression, and it can be used
         * to provide the PropertyMetadata.DefaultValue. PostSharp could not do this, so it provided the DefaultValue attribute property as a
         * compromised alternative (only constant default values can be provided that way). So in ML, we should not need the Start/StopIgnoringContracts
         * concept. By default (according to DependencyPropertyOptions), we will use the initializer to set PropertyMetadata.DefaultValue
         * (controlled by DependencyPropertyOptions.InitializerProvidesDefaultValue), and we will *not* call DependencyObject.SetValue (controlled by
         * DependencyPropertyOptions.InitializerProvidesInitialValue).
         */

        this._builder.Advice.WithTemplateProvider( Templates.Provider )
            .AddInitializer(
                declaringType,
                nameof(Templates.InitializeDependencyProperty),
                InitializerKind.BeforeTypeConstructor,
                args: new
                {
                    dependencyPropertyField = introduceRegistrationFieldResult.Declaration,
                    options = this._options,
                    propertyName = match.DependencyPropertyName,
                    propertyType,
                    declaringType,
                    defaultValueExpr = this._builder.Target.InitializerExpression,
                    onChangedMethod,
                    onChangedSignatureKind = match.PropertyChangedMatch.Kind,
                    validateMethod,
                    validateSignatureKind = match.ValidateMatch.Kind,
                    applyContractsMethod
                } );

        this._builder.Advice.WithTemplateProvider( Templates.Provider )
            .OverrideAccessors(
                this._builder.Target,
                new GetterTemplateSelector( nameof(Templates.OverrideGetter) ),
                args: new { propertyType, dependencyPropertyField = introduceRegistrationFieldResult.Declaration } );

        if ( this._builder.Target.Writeability != Writeability.None )
        {
            this._builder.Advice.WithTemplateProvider( Templates.Provider )
                .OverrideAccessors(
                    this._builder.Target,
                    setTemplate: nameof(Templates.OverrideSetter),
                    args: new { dependencyPropertyField = introduceRegistrationFieldResult.Declaration } );
        }

        // Here we avoid the temptation to generate a static field to store the result of the initializer expression
        // and use the same result for the default value and as the initial value of all instances of the declaring type. This
        // pattern does not have the same semantics as a regular property initializer, which would be invoked for each instance
        // of the declaring type, while the default value is evaluated just once as this is a statically scoped value.
        // Therefore we evaluate the initializer expression once for the default value and a second time for each object instance.

        if ( this._builder.Target.InitializerExpression != null )
        {
            this._builder.Advice.WithTemplateProvider( Templates.Provider )
                .AddInitializer(
                    declaringType,
                    nameof(Templates.Assign),
                    InitializerKind.BeforeInstanceConstructor,
                    args: new { left = this._builder.Target, right = this._builder.Target.InitializerExpression } );
        }
    }

    private HashSet<string>? _existingMemberNames;

    /// <summary>
    /// Gets an unused member name for the target type by adding an numeric suffix until an unused name is found.
    /// </summary>
    /// <param name="desiredName"></param>
    /// <returns></returns>
    private string GetAndReserveUnusedMemberName( string desiredName )
    {
        this._existingMemberNames ??= new HashSet<string>(
            ((IEnumerable<INamedDeclaration>) this._builder.Target.DeclaringType.AllMembers()).Concat( this._builder.Target.DeclaringType.Types )
            .Select( m => m.Name ) );

        if ( this._existingMemberNames.Add( desiredName ) )
        {
            return desiredName;
        }
        else
        {
            // ReSharper disable once BadSemicolonSpaces
            for ( var i = 1; /* Intentionally empty */; i++ )
            {
                var adjustedName = $"{desiredName}_{i}";

                if ( this._existingMemberNames.Add( adjustedName ) )
                {
                    return adjustedName;
                }
            }
        }
    }
}