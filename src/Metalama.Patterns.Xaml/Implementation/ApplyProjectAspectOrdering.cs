// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
using Metalama.Framework.Eligibility;
using Metalama.Patterns.Xaml.Implementation;

[assembly: ApplyProjectAspectOrdering]
[assembly:
    AspectOrder(
        "Metalama.Patterns.Contracts.ContractAspect:" + ContractAspect.BuildLayer,
        "Metalama.Patterns.Xaml.DependencyPropertyAttribute",
        "Metalama.Patterns.Contracts.ContractAspect" )]

namespace Metalama.Patterns.Xaml.Implementation;

/// <summary>
/// Adds <see cref="AspectOrderAttribute"/> for all concrete types deriving <see cref="ContractAspect"/> in assembly <c>Metalama.Patterns.Contracts</c>.
/// </summary>
[AttributeUsage( AttributeTargets.Assembly )]
internal sealed class ApplyProjectAspectOrdering : Attribute, IAspect<ICompilation>
{
    public void BuildAspect( IAspectBuilder<ICompilation> builder )
    {
        var compilation = builder.Target;

        var contractAspectType = TypeFactory.GetType( typeof(ContractAspect) );
        var aspectOrderAttributeType = (INamedType) TypeFactory.GetType( typeof(AspectOrderAttribute) );

        var mpcAssy = compilation.ReferencedAssemblies.OfName( "Metalama.Patterns.Contracts" ).First();

        foreach ( var contractType in mpcAssy.AllTypes.Where( t => !t.IsAbstract && t.ForCompilation( compilation ).Is( contractAspectType ) ) )
        {
            var fullName = contractType.FullName;

            var attributeConstruction = AttributeConstruction.Create(
                aspectOrderAttributeType,
                new object[] { $"{fullName}:{ContractAspect.BuildLayer}", "Metalama.Patterns.Xaml.DependencyPropertyAttribute", fullName } );

            builder.Advice.IntroduceAttribute( compilation, attributeConstruction, OverrideStrategy.New );
        }
    }

    public void BuildEligibility( IEligibilityBuilder<ICompilation> builder ) { }
}