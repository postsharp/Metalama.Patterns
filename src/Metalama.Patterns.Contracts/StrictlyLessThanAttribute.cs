﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

#pragma warning disable IDE0004 // Remove Unnecessary Cast: in this problem domain, explicit casts add clarity.

// Resharper disable RedundantCast

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is greater than or equal to a given value.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>
///     Floating-point values are tested to be greater than or equal to the maximum value
///     plus a tolerance value. The tolerance value is equal to the distance
///     of the value closest to the maximum value according to the precision
///     of the respective floating-point numerical data type.
/// </para>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.StrictlyLessThanErrorMessage"/>.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the minimum value used.</para>
/// </remarks>
[PublicAPI]
public partial class StrictlyLessThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( long max )
        : base(
            long.MinValue,
            max,
            long.MinValue,
            Int64Maximum.ToInt64( max ),
            ulong.MinValue,
            Int64Maximum.ToUInt64( max ),
            double.MinValue,
            Int64Maximum.ToDouble( max ),
            decimal.MinValue,
            Int64Maximum.ToDecimal( max ),
            GetInvalidTypes( long.MinValue, Int64Maximum.ToInt64( max ) ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( ulong max )
        : base(
            ulong.MinValue,
            max,
            long.MinValue,
            UInt64Maximum.ToInt64( max ),
            ulong.MinValue,
            UInt64Maximum.ToUInt64( max ),
            double.MinValue,
            UInt64Maximum.ToDouble( max ),
            decimal.MinValue,
            UInt64Maximum.ToDecimal( max ),
            GetInvalidTypes( 0 ) | (max == 0 ? TypeFlag.UInt16 | TypeFlag.UInt32 | TypeFlag.UInt64 : TypeFlag.None) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyLessThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="max">The upper bound.</param>
    public StrictlyLessThanAttribute( double max )
        : base(
            double.MinValue,
            max,
            long.MinValue,
            DoubleMaximum.ToInt64( max ),
            ulong.MinValue,
            DoubleMaximum.ToUInt64( max ),
            double.MinValue,
            DoubleMaximum.ToDouble( max ),
            decimal.MinValue,
            DoubleMaximum.ToDecimal( max ),
            GetInvalidTypes( double.MinValue, max > double.MinValue + 1 ? max + 1 : double.MinValue ) ) { }

    /// <inheritdoc/>
    protected override ExceptionInfo GetExceptionInfo()
        => new(
            CompileTimeHelpers.GetContractLocalizedTextProviderField(
                nameof(ContractLocalizedTextProvider
                           .StrictlyLessThanErrorMessage) ),
            false,
            true );

    private static readonly DiagnosticDefinition<(IDeclaration, string)> _rangeCannotBeApplied =
        CreateCannotBeAppliedDiagnosticDefinition( "LAMA5006", nameof(StrictlyLessThanAttribute) );

    /// <inheritdoc/>
    protected override DiagnosticDefinition<(IDeclaration Declaration, string TargetBasicType)> GetCannotBeAppliedDiagnosticDefinition()
        => _rangeCannotBeApplied;
}