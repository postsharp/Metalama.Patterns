// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts.Numeric;

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
/// <seealso href="@contract-types"/>
[PublicAPI]
public class GreaterThanAttribute : RangeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="long"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( long min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="int"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( int min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="short"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( short min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="sbyte"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( sbyte min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="ulong"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( ulong min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="uint"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( uint min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="ushort"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( ushort min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="byte"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( byte min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="double"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( double min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="float"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( float min )
        : base( NumericBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a minimum value of type <see cref="decimal"/>.
    /// </summary>
    /// <param name="min">The minimum allowed value.</param>
    public GreaterThanAttribute( decimal min )
        : base( NumericBound.Create( min ), default ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnGreaterThanContractViolated( value, this.Range.MinValue!.ObjectValue );
    }
}