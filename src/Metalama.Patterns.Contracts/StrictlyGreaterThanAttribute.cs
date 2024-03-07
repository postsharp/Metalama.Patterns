// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts.Implementation;

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
/// </remarks>
[PublicAPI]
public class StrictlyGreaterThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( long min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( int min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( short min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( sbyte min )
        : base( RangeBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( ulong min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( uint min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( ushort min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( byte min )
        : base( RangeBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public StrictlyGreaterThanAttribute( double min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( decimal min )
        : base( RangeBound.Create( min, false ), null ) { }

    public StrictlyGreaterThanAttribute( float min )
        : base( RangeBound.Create( min, false ), null ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnStrictlyGreaterThanContractViolated( value, this.Range.MinValue!.ObjectValue );
    }
}