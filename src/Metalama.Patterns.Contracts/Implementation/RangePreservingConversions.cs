// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts.Implementation;

/// <summary>
/// Converts a number of a type into a number of a different type, respecting the range of each time, i.e.
/// if the input value is out of the range of the target type, it is changed to the bound of the range.
/// </summary>
[RunTimeOrCompileTime]
internal static class RangePreservingConversions
{
    public static TypedRangeBound<decimal> ToDecimal( double value, bool isStrict )
        => value switch
        {
            < (double) decimal.MinValue => new TypedRangeBound<decimal>( decimal.MinValue, false ),
            > (double) decimal.MaxValue => new TypedRangeBound<decimal>( decimal.MaxValue, false ),
            _ => new TypedRangeBound<decimal>( (decimal) value, isStrict )
        };

    public static TypedRangeBound<long> ToInt64( double value, bool isStrict )
        => value switch
        {
            < long.MinValue => new TypedRangeBound<long>( long.MinValue, false ),
            > long.MaxValue => new TypedRangeBound<long>( long.MaxValue, false ),
            _ => new TypedRangeBound<long>( (long) value, isStrict )
        };

    public static TypedRangeBound<long> ToInt64( decimal value, bool isStrict )
        => value switch
        {
            < long.MinValue => new TypedRangeBound<long>( long.MinValue, false ),
            > long.MaxValue => new TypedRangeBound<long>( long.MaxValue, false ),
            _ => new TypedRangeBound<long>( (long) value, isStrict )
        };

    public static TypedRangeBound<long> ToInt64( ulong value, bool isStrict )
        => value switch
        {
            > long.MaxValue => new TypedRangeBound<long>( long.MaxValue, false ),
            _ => new TypedRangeBound<long>( (long) value, isStrict )
        };

    public static TypedRangeBound<ulong> ToUInt64( double value, bool isStrict )
        => value switch
        {
            < ulong.MinValue => new TypedRangeBound<ulong>( ulong.MinValue, false ),
            > ulong.MaxValue => new TypedRangeBound<ulong>( ulong.MaxValue, false ),
            _ => new TypedRangeBound<ulong>( (ulong) value, isStrict )
        };

    public static TypedRangeBound<ulong> ToUInt64( decimal value, bool isStrict )
        => value switch
        {
            < ulong.MinValue => new TypedRangeBound<ulong>( ulong.MinValue, false ),
            > ulong.MaxValue => new TypedRangeBound<ulong>( ulong.MaxValue, false ),
            _ => new TypedRangeBound<ulong>( (ulong) value, isStrict )
        };

    public static TypedRangeBound<ulong> ToUInt64( long value, bool isStrict )
        => value switch
        {
            < 0 => new TypedRangeBound<ulong>( ulong.MinValue, false ),
            _ => new TypedRangeBound<ulong>( (ulong) value, isStrict )
        };
}