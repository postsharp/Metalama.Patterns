// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Xaml.Implementation;

// TODO: #34040 - Replace all code in this file with the following once [AspectOrder] supports ordering by base types, and remove the project reference to Metalama.Patterns.Contracts.
// [assembly: AspectOrder( "Metalama.Framework.Aspects.ContractAspect:" + ContractAspect.Layer1Build, "Metalama.Patterns.Xaml.DependencyPropertyAttribute:*", "Metalama.Framework.Aspects.ContractAspect:" + ContractAspect.Layer0Apply )]

// TODO: Pending "LAMA0021: A cycle was found..." fix, uncomment the following line:
// [assembly: ApplyProjectAspectOrdering]
[assembly: AspectOrder( "Metalama.Patterns.Contracts.TrimAttribute:" + ContractAspect.Layer1Build, "Metalama.Patterns.Xaml.DependencyPropertyAttribute", "Metalama.Patterns.Contracts.TrimAttribute:" + ContractAspect.Layer0Apply )]

// TODO: Pending "LAMA0021: A cycle was found..." fix, remove the following line:
[assembly: AspectOrder( new string[] { "Metalama.Patterns.Contracts.NotNullAttribute:Layer1Build", "Metalama.Patterns.Xaml.DependencyPropertyAttribute", "Metalama.Patterns.Contracts.NotNullAttribute:Layer0Apply" } )]

namespace Metalama.Patterns.Xaml.Implementation;

/// <summary>
/// Adds <see cref="AspectOrderAttribute"/> for all concrete types deriving <see cref="ContractAspect"/> in assembly <c>Metalama.Patterns.Contracts</c>.
/// </summary>
[AttributeUsage( AttributeTargets.Assembly )]
internal class ApplyProjectAspectOrdering : Attribute, IAspect<ICompilation>
{
    public void BuildAspect( IAspectBuilder<ICompilation> builder )
    {
        var compilation = builder.Target;

        var contractAspectType = TypeFactory.GetType( typeof( ContractAspect ) );
        var aspectOrderAttributeType = (INamedType) TypeFactory.GetType( typeof( AspectOrderAttribute ) );

        var mpcAssy = compilation.ReferencedAssemblies.OfName( "Metalama.Patterns.Contracts" ).First();
        
        foreach ( var contractType in mpcAssy.AllTypes.Where( t => !t.IsAbstract && t.ForCompilation( compilation ).Is( contractAspectType ) ) )
        {
            var fullName = contractType.FullName;

            var attributeConstruction = AttributeConstruction.Create(
                aspectOrderAttributeType,
                new object[]
                {
                    $"{fullName}:{ContractAspect.Layer1Build}",
                    "Metalama.Patterns.Xaml.DependencyPropertyAttribute",
                    $"{fullName}:{ContractAspect.Layer0Apply}"
                } );

            builder.Advice.IntroduceAttribute( compilation, attributeConstruction, OverrideStrategy.New );
        }
    }

    public void BuildEligibility( IEligibilityBuilder<ICompilation> builder )
    {
    }
}