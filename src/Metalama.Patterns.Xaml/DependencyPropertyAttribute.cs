// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Clean up unused references in project file.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Framework.Project;
using System.Windows;

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Property )]
public sealed class DependencyPropertyAttribute : Attribute, IAspect<IProperty>
{
    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        builder.MustNotBeStatic();
        builder.MustHaveAccessibility( Framework.Code.Accessibility.Public );
        builder.MustBeReadable();
        builder.MustSatisfy( p => p.IsAutoPropertyOrField == true, p => $"{p} must be an auto-property." );
    }

    void IAspect<IProperty>.BuildAspect( IAspectBuilder<IProperty> builder )
    {
        var propertyType = builder.Target.Type;
        var declaringType = builder.Target.DeclaringType;

        var introduceFieldResult = builder.WithTarget( declaringType ).Advice.IntroduceField(
            declaringType,
            $"{builder.Target.Name}Property",
            typeof( DependencyProperty ),
            IntroductionScope.Static,
            OverrideStrategy.Fail,
            b =>
            {
                b.Accessibility = Framework.Code.Accessibility.Public;
                b.Writeability = Writeability.ConstructorOnly;
            } );

        if ( !MetalamaExecutionContext.Current.ExecutionScenario.CapturesNonObservableTransformations )
        {
            return;
        }

        if ( introduceFieldResult.Outcome == AdviceOutcome.Default )
        {
            builder.WithTarget( declaringType ).Advice.AddInitializer(
                declaringType,
                builder.Target.Writeability == Writeability.All
                    ? nameof( InitializeProperty )
                    : nameof( InitializeReadOnlyDependencyProperty ),
                InitializerKind.BeforeTypeConstructor,
                args: new
                {
                    field = introduceFieldResult.Declaration,
                    name = builder.Target.Name,
                    propertyType,
                    declaringType
                } );
        }
    }

    [Template]
    private static void InitializeProperty(
        [CompileTime] IField field,
        [CompileTime] string name, 
        [CompileTime] INamedType propertyType,
        [CompileTime] INamedType declaringType )
    {
        // Simple for now, we do not yet support default value, callbacks or validators.
        field.Value = DependencyProperty.Register( name, propertyType.ToTypeOfExpression().Value, declaringType.ToTypeOfExpression().Value );
    }

    [Template]
    private static void InitializeReadOnlyDependencyProperty(
        [CompileTime] IField field,
        [CompileTime] string name,
        [CompileTime] INamedType propertyType,
        [CompileTime] INamedType declaringType )
    {
        // Simple for now, we do not yet support default value, callbacks or validators.
        field.Value = DependencyProperty.RegisterReadOnly( name, propertyType.ToTypeOfExpression().Value, declaringType.ToTypeOfExpression().Value, new FrameworkPropertyMetadata() ).DependencyProperty;
    }
}