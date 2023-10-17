// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Xaml.Implementation;
using Metalama.Patterns.Xaml.Options;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml;

[AttributeUsage( AttributeTargets.Property )]
public sealed class CommandAttribute : CommandOptionsAttribute, IAspect<IProperty>
{
    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
        builder.MustNotBeStatic();
        builder.MustHaveAccessibility( Framework.Code.Accessibility.Public );
        builder.MustBeReadable();
        builder.MustSatisfy( p => p.IsAutoPropertyOrField == true != (p.Writeability == Writeability.None), p => $"{p} must be an auto-property with only a getter, or a non-auto property with both a getter and setter." );
        builder.Type().MustBe( typeof( ICommand ) );
    }

    void IAspect<IProperty>.BuildAspect( IAspectBuilder<IProperty> builder )
    {
        var aspectBuilder = new CommandAspectBuilder( builder );
        aspectBuilder.Build();
    }
}