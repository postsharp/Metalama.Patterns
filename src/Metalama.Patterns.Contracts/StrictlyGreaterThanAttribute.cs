// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts.Numeric;

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
/// <seealso cref="@contract-types"/>
[PublicAPI]
public class StrictlyGreaterThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="long"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( long min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="int"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( int min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="short"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( short min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="sbyte"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( sbyte min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="ulong"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( ulong min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="uint"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( uint min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="ushort"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( ushort min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="byte"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( byte min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="double"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( double min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="decimal"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( decimal min )
        : base( NumericBound.Create( min, false ), null ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrictlyGreaterThanAttribute"/> class specifying a minimum value of type <see cref="float"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public StrictlyGreaterThanAttribute( float min )
        : base( NumericBound.Create( min, false ), null ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnStrictlyGreaterThanContractViolated( value, this.Range.MinValue!.ObjectValue );
    }
}