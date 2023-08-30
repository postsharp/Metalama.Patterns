// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;

#pragma warning disable IDE0004 // Remove Unnecessary Cast: in this problem domain, explicit casts add clarity.

// Resharper disable RedundantCast

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is smaller than a given value.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// </remarks>
[PublicAPI]
public class GreaterThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( long min )
        : base(
            min,
            long.MaxValue,
            min,
            long.MaxValue,
            min < 0 ? 0 : (ulong) min,
            ulong.MaxValue,
            min,
            double.MaxValue,
            min,
            decimal.MaxValue,
            GetInvalidTypes( min, long.MaxValue ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( ulong min )
        : base(
            min,
            ulong.MaxValue,
            min > (ulong) long.MaxValue ? long.MaxValue : (long) min,
            long.MaxValue,
            min,
            ulong.MaxValue,
            min,
            double.MaxValue,
            min,
            decimal.MaxValue,
            GetInvalidTypes( min ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( double min )
        : base(
            min,
            double.MaxValue,
            DoubleMinimum.ToInt64( min ),
            long.MaxValue,
            DoubleMinimum.ToUInt64( min ),
            ulong.MaxValue,
            min,
            double.MaxValue,
            DoubleMinimum.ToDecimal( min ),
            decimal.MaxValue,
            GetInvalidTypes( min, double.MaxValue ) ) { }

    private static class DoubleMinimum
    {
        public static long ToInt64( double min )
        {
            if ( min < (double) long.MinValue )
            {
                return long.MinValue;
            }

            if ( min > (double) long.MaxValue )
            {
                return long.MaxValue;
            }

            return (long) min;
        }

        public static ulong ToUInt64( double min )
        {
            if ( min < 0 )
            {
                return 0;
            }

            if ( min > (double) ulong.MaxValue )
            {
                return ulong.MaxValue;
            }

            return (ulong) min;
        }

        public static decimal ToDecimal( double min )
        {
            if ( min > (double) decimal.MaxValue )
            {
                return decimal.MaxValue;
            }

            if ( min < (double) decimal.MinValue )
            {
                return decimal.MinValue;
            }

            return (decimal) min;
        }
    }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.Project.ContractOptions().Templates.OnGreaterThanContractViolated( value, this.DisplayMinValue );
    }

    private static readonly DiagnosticDefinition<(IDeclaration, string)> _rangeCannotBeApplied =
        CreateCannotBeAppliedDiagnosticDefinition( "LAMA5001", nameof(GreaterThanAttribute) );

    /// <inheritdoc/>
    protected override DiagnosticDefinition<(IDeclaration Declaration, string TargetBasicType)> GetCannotBeAppliedDiagnosticDefinition()
        => _rangeCannotBeApplied;
}