// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Describes a numeric range in the representation of various different numeric types.
/// </summary>
/// <seealso cref="RangeAttribute"/>
[PublicAPI]
internal readonly struct RangeValues
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RangeValues"/> struct.
    /// </summary>
    public RangeValues(
        long minInt64,
        long maxInt64,
        ulong minUInt64,
        ulong maxUInt64,
        double minDouble,
        double maxDouble,
        decimal minDecimal,
        decimal maxDecimal )
    {
        this.MinInt64 = minInt64;
        this.MaxInt64 = maxInt64;
        this.MinUInt64 = minUInt64;
        this.MaxUInt64 = maxUInt64;
        this.MinDouble = minDouble;
        this.MaxDouble = maxDouble;
        this.MinDecimal = minDecimal;
        this.MaxDecimal = maxDecimal;
    }

    public readonly long MinInt64;
    public readonly long MaxInt64;

    public readonly ulong MinUInt64;
    public readonly ulong MaxUInt64;

    public readonly double MinDouble;
    public readonly double MaxDouble;

    public readonly decimal MinDecimal;
    public readonly decimal MaxDecimal;
}