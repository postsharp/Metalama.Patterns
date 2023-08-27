// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

// Resharper disable RedundantCast

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is smaller than or equal to a given value.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>
///     Floating-point values are tested to be greater than or equal to the minimum value
///     plus a tolerance value. The tolerance value is equal to the distance
///     of the value closest to the minimum value according to the precision
///     of the respective floating-point numerical data type.
/// </para>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.StrictlyGreaterThanErrorMessage"/>.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the minimum value used.</para>
/// </remarks>
[PublicAPI]
public partial class StrictlyGreaterThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( long min )
        : base(
            min,
            long.MaxValue,
            Int64Minimum.ToInt64( min ),
            long.MaxValue,
            Int64Minimum.ToUInt64( min ),
            ulong.MaxValue,
            Int64Minimum.ToDouble( min ),
            double.MaxValue,
            Int64Minimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( Int64Minimum.ToInt64( min ), long.MaxValue ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( ulong min )
        : base(
            min,
            ulong.MaxValue,
            UInt64Minimum.ToInt64( min ),
            long.MaxValue,
            UInt64Minimum.ToUInt64( min ),
            ulong.MaxValue,
            UInt64Minimum.ToDouble( min ),
            double.MaxValue,
            UInt64Minimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( UInt64Minimum.ToUInt64( min ) ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( double min )
        : base(
            min,
            double.MaxValue,
            DoubleMinimum.ToInt64( min ),
            long.MaxValue,
            DoubleMinimum.ToUInt64( min ),
            ulong.MaxValue,
            DoubleMinimum.ToDouble( min ),
            double.MaxValue,
            DoubleMinimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( min < double.MaxValue - 1 ? min + 1 : double.MaxValue, double.MaxValue ) ) { }

    /// <inheritdoc/>
    protected override ExceptionInfo GetExceptionInfo()
        => new(
            CompileTimeHelpers.GetContractLocalizedTextProviderField(
                nameof(ContractLocalizedTextProvider
                           .StrictlyGreaterThanErrorMessage) ),
            true,
            false );

    private static readonly DiagnosticDefinition<(IDeclaration, string)> _rangeCannotBeApplied =
        CreateCannotBeAppliedDiagnosticDefinition( "LAMA5005", nameof(StrictlyGreaterThanAttribute) );

    /// <inheritdoc/>
    protected override DiagnosticDefinition<(IDeclaration Declaration, string TargetBasicType)> GetCannotBeAppliedDiagnosticDefinition()
        => _rangeCannotBeApplied;
}