// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentNullException"/> if the target is assigned a <see langword="null"/> value.
/// </summary>
/// <remarks>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.NotNullErrorMessage"/>.</para>
/// </remarks>
[PublicAPI]
[Inheritable]
public sealed class NotNullAttribute : ContractAspect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotNullAttribute"/> class.
    /// </summary>
    public NotNullAttribute() { }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );

        builder.MustSatisfy(
            f => f.Type.IsReferenceType != false || f.Type.IsNullable != false,
            f => $"the type of {f} must be a nullable type" );
    }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );

        builder.MustSatisfy(
            p => p.Type.IsReferenceType != false || p.Type.IsNullable != false,
            p => $"the type of {p} must be a nullable type" );
    }

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        if ( value == null! )
        {
            throw ContractsServices.Default.ExceptionFactory.CreateException(
                ContractExceptionInfo.Create(
                    typeof(ArgumentNullException),
                    typeof(NotNullAttribute),
                    value,
                    meta.Target.GetTargetName(),
                    meta.Target.GetTargetKind(),
                    meta.Target.ContractDirection,
                    ContractLocalizedTextProvider.NotNullErrorMessage ) );
        }
    }
}