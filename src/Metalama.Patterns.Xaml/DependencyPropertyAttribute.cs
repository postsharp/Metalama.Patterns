﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Clean up unused references in project file.

// TODO: Investigating compile time exception inside Metalma. Remove conditions when resolved.
#define ENABLE_LOCALFUNCTIONS_WORKAROUND_WA1_WA2

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Options;
using System.Windows;

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Property )]
public sealed partial class DependencyPropertyAttribute : DependencyPropertyOptionsAttribute, IAspect<IProperty>
{
    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        builder.MustNotBeStatic();
        builder.MustHaveAccessibility( Framework.Code.Accessibility.Public );
        builder.MustBeReadable();
        builder.MustSatisfy( p => p.IsAutoPropertyOrField == true, p => $"{p} must be an auto-property." );
        builder.DeclaringType().MustBe( typeof( DependencyObject ), ConversionKind.Reference );
    }

    void IAspect<IProperty>.BuildAspect(Metalama.Framework.Aspects.IAspectBuilder<Metalama.Framework.Code.IProperty> builder)
    {
        var aspectBuilder = new DependencyPropertyAspectBuilder( builder );
        aspectBuilder.Build();
    }
}