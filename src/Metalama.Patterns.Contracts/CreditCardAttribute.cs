﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentException"/> if the target is assigned a value that
/// is not a valid credit card number. Null strings are accepted and do not
/// throw an exception.
/// </summary>
/// <remarks>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.CreditCardErrorMessage"/>.</para>
/// </remarks>
[Inheritable]
public sealed class CreditCardAttribute : ContractAspect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreditCardAttribute"/> class.
    /// </summary>
    public CreditCardAttribute()
    {
    }

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

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        if ( !CreditCardAttributeHelper.IsValidCreditCardNumber( value ) )
        {
            throw ContractsServices.Default.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                typeof(ArgumentNullException),
                typeof(CreditCardAttribute),
                value,
                meta.Target.GetTargetName(),
                meta.Target.GetTargetKind(),
                meta.Target.ContractDirection,
                ContractLocalizedTextProvider.CreditCardErrorMessage ) );
        }
    }
}