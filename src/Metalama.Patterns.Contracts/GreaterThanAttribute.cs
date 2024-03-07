// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts.Implementation;

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
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( int min )
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( short min )
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( sbyte min )
        : base( RangeBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying an unsigned integer bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( ulong min )
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( uint min )
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( ushort min )
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( byte min )
        : base( RangeBound.Create( min ), default ) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class specifying a floating-point bound.
    /// </summary>
    /// <param name="min">The lower bound.</param>
    public GreaterThanAttribute( double min )
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( float min )
        : base( RangeBound.Create( min ), default ) { }

    public GreaterThanAttribute( decimal min )
        : base( RangeBound.Create( min ), default ) { }

    protected override void OnContractViolated( dynamic? value )
    {
        meta.Target.GetContractOptions().Templates!.OnGreaterThanContractViolated( value, this.Range.MinValue.ObjectValue );
    }
}