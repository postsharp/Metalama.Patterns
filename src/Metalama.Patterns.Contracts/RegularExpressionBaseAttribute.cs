// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// The base class of contracts that are based on custom attributes.
/// </summary>
[PublicAPI]
[Inheritable]
public abstract class RegularExpressionBaseAttribute : ContractAspect
{
    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );
        builder.Type().MustBe<string>();
    }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );
        builder.Type().MustBe<string>();
    }

    protected abstract IExpression GetRegex();

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        var regex = (Regex) this.GetRegex().Value!;

        if ( value != null && !regex.IsMatch( (string) value! ) )
        {
            this.OnContractViolated( value, regex );
        }
    }

    [Template]
    protected virtual void OnContractViolated( dynamic? value, dynamic regex )
    {
        meta.Target.GetContractOptions().Templates!.OnRegularExpressionContractViolated( value, regex );
    }
}