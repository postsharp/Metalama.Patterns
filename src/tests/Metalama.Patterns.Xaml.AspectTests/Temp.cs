// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Contracts;
using System.Diagnostics;

[assembly: AspectOrder( "Metalama.Patterns.AspectTests.Temp.TrimAttribute:" + ContractAspect.Layer1Build, "Metalama.Patterns.AspectTests.Temp.ATestAspectAttribute", "Metalama.Patterns.AspectTests.Temp.TrimAttribute:" + ContractAspect.Layer0Apply )]
[assembly: AspectOrder( "Metalama.Patterns.Contracts.PositiveAttribute:" + ContractAspect.Layer1Build, "Metalama.Patterns.AspectTests.Temp.ATestAspectAttribute", "Metalama.Patterns.Contracts.PositiveAttribute:" + ContractAspect.Layer0Apply )]

namespace Metalama.Patterns.Xaml.AspectTests.Temp;

// <target>
public partial class TestTarget
{    
    [ATestAspect]
    [Positive]
    public int Foo { get; set; }

    [ATestAspect]
    [Trim]
    public string? Bar { get; set; }
}

[AttributeUsage( AttributeTargets.Property )]
public sealed class ATestAspectAttribute : Attribute, IAspect<IProperty>
{
    void IAspect<IProperty>.BuildAspect( IAspectBuilder<IProperty> builder )
    {
        // TODO: #34041 Use HasAspect<ContractAspect> once base types are supported.
        var instances = builder.Target.Enhancements().GetAspectInstances();

        Debugger.Break();

        if ( builder.Target.Enhancements().GetAspectInstances().Any( instance => typeof( ContractAspect ).IsAssignableFrom( instance.AspectClass.Type ) ) )
        {
            var result = builder.Advice.IntroduceMethod(
                builder.Target.DeclaringType,
                nameof( ValidationProxy ),
                IntroductionScope.Instance,
                OverrideStrategy.Fail,
                b => b.Name = $"Validate{builder.Target.Name}Proxy",
                args: new
                {
                    T = builder.Target.Type
                } );

            if ( result.Outcome == AdviceOutcome.Error )
            {
                return;
            }

            builder.Advice.AddAnnotation( builder.Target, new ContractAspect.RedirectToProxyParameterAnnotation( result.Declaration.Parameters[0] ) );
        }
    }

    [Template]
    private static T ValidationProxy<[CompileTime] T>( T value )
    {
        return value;
    }

    void IEligible<IProperty>.BuildEligibility( IEligibilityBuilder<IProperty> builder )
    {
    }
}

[Layers( Layer0Apply, Layer1Build )]
public sealed class TrimAttribute : ContractAspect
{
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        builder.Type().MustBe( typeof( string ) );
        base.BuildEligibility( builder );
    }

    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        builder.Type().MustBe( typeof( string ) );
        base.BuildEligibility( builder );
    }

    public override void Validate( dynamic? value )
    {
#pragma warning disable IDE0059 // Unnecessary assignment of a value
        value = value?.Trim();
#pragma warning restore IDE0059 // Unnecessary assignment of a value
    }
}