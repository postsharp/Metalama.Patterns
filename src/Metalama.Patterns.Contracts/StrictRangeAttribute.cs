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
/// <seealso cref="@contract-types"/>
[PublicAPI]
public class StrictRangeAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="long"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( long min, long max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="int"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( int min, int max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="short"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( short min, short max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="sbyte"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( sbyte min, sbyte max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="ulong"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( ulong min, ulong max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="uint"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( uint min, uint max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="ushort"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( ushort min, ushort max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="byte"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( byte min, byte max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="double"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( double min, double max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="float"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( float min, float max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictRangeAttribute"/> class specifying bounds of type <see cref="decimal"/>.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public StrictRangeAttribute( decimal min, decimal max )
        : base( RangeBound.Create( min, false ), RangeBound.Create( max, false ) ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnStrictRangeContractViolated( value, this.Range );
    }
}