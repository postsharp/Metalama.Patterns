// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws:
/// <list type="bullet">
///     <item><description>
///         an <see cref="ArgumentNullException"/> if the target is assigned a null value;
///     </description></item>
///     <item><description>
///         an <see cref="ArgumentOutOfRangeException"/> if the target is assigned an empty or white-space string.
///     </description></item>
/// </list>
/// </summary>
/// <remarks>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.RequiredErrorMessage"/>.</para>
/// </remarks>
[Inheritable]
public sealed class RequiredAttribute : ContractAspect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredAttribute"/> class.
    /// </summary>
    public RequiredAttribute()
    {
    }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );
        builder.MustSatisfy(
            f => f.Type.IsReferenceType != false || f.Type.IsNullable != false,
            f => $"the type of {f} must be a nullable" );
    }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );
        builder.MustSatisfy(
            p => p.Type.IsReferenceType != false || p.Type.IsNullable != false,
            p => $"the type of {p} must be a nullable" );
    }

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        var targetKind = meta.Target.GetTargetKind();
        var targetName = meta.Target.GetTargetName();
        var targetType = meta.Target.GetTargetType();

        if ( targetType.SpecialType == SpecialType.String )
        {
            if ( string.IsNullOrWhiteSpace( value ) )
            {
                if ( value == null! )
                {
                    throw ContractsServices.Default.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                        typeof(ArgumentNullException),
                        typeof(RequiredAttribute),
                        value,
                        targetName,
                        targetKind,
                        meta.Target.ContractDirection,
                        ContractLocalizedTextProvider.RequiredErrorMessage ) );
                }
                else
                {
                    throw ContractsServices.Default.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                        typeof(ArgumentOutOfRangeException),
                        typeof(RequiredAttribute),
                        value,
                        targetName,
                        targetKind,
                        meta.Target.ContractDirection,
                        ContractLocalizedTextProvider.RequiredErrorMessage ) );
                }
            }
        }
        else
        {
            if ( value == null! )
            {
                throw ContractsServices.Default.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                    typeof(ArgumentNullException),
                    typeof(RequiredAttribute),
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    ContractLocalizedTextProvider.RequiredErrorMessage ) );
            }
        }
    }
}