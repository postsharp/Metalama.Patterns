// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts.Implementation;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentOutOfRangeException"/> if the target is assigned a value that
/// is not strictly within the given range.
/// </summary>
/// <remarks>
///     <para>Null values are accepted and do not throw an exception.
/// </para>
/// <para>
///     Floating-point values are tested to be strictly within the given bounds
///     with a tolerance value. The tolerance value is equal to the distance
///     of the value closest to the bounds according to the precision
///     of the respective floating-point numerical data type.
/// </para>
/// </remarks>
[PublicAPI]
public class StrictRangeAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public StrictRangeAttribute( long min, long max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( int min, int max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( short min, short max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( sbyte min, sbyte max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying unsigned integer bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public StrictRangeAttribute( ulong min, ulong max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( uint min, uint max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( ushort min, ushort max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( byte min, byte max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying floating-point bounds.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    /// <param name="max">The upper bound.</param>
    public StrictRangeAttribute( double min, double max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( float min, float max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    public StrictRangeAttribute( decimal min, decimal max )
        : base( RangeBound.Create( min, true ), RangeBound.Create( max, false ) ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnStrictRangeContractViolated( value, this.Range );
    }
}