// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Property )]
public sealed class DependencyPropertyAttribute : DependencyPropertyOptionsAttribute, IAspect<IProperty>
{
    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        // TODO: Aspect tests for eligibility.

        builder.MustNotBeStatic();
        
        // ReSharper disable once RedundantNameQualifier
        builder.MustHaveAccessibility( Framework.Code.Accessibility.Public );
        builder.MustBeReadable();
        builder.MustSatisfy( p => p.IsAutoPropertyOrField == true, p => $"{p} must be an auto-property." );
        builder.DeclaringType().MustBe( typeof(DependencyObject), ConversionKind.Reference );
    }

    void IAspect<IProperty>.BuildAspect( IAspectBuilder<IProperty> builder )
    {
        var aspectBuilder = new DependencyPropertyAspectBuilder( builder );
        aspectBuilder.Build();
    }
}