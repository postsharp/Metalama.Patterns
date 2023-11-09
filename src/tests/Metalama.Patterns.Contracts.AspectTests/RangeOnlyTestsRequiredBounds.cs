// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.AspectTests;

public sealed class RangeOnlyTestsRequiredBounds
{
    public void Negative( [Negative] int x ) { }

    public void Positive( [Positive] int x ) { }

    public void StrictlyNegative( [StrictlyNegative] int x ) { }

    public void StrictlyPositive( [StrictlyPositive] int x ) { }

    public void LessThanInt( [LessThan( 42 )] int x ) { }

    public void GreaterThanInt( [GreaterThan( 42 )] int x ) { }

    public void RangeInt( [Range( 10, 20 )] int x ) { }

    public void StrictlyGreaterThanInt( [StrictlyGreaterThan( 42 )] int x ) { }

    public void StrictlyLessThanInt( [StrictlyLessThan( 42 )] int x ) { }

    public void StrictRangeInt( [StrictRange( 10.0, 20.0 )] int x ) { }

    public void LessThanDouble( [LessThan( 42.0 )] int x ) { }

    public void GreaterThanDouble( [GreaterThan( 42.0 )] int x ) { }

    public void RangeDouble( [Range( 10.0, 20.0 )] int x ) { }

    public void StrictlyGreaterThanDouble( [StrictlyGreaterThan( 42.0 )] int x ) { }

    public void StrictlyLessThanDouble( [StrictlyLessThan( 42.0 )] int x ) { }

    public void StrictRangeDouble( [StrictRange( 10.0, 20.0 )] int x ) { }

    public void LessThanUnsigned( [LessThan( 42ul )] int x ) { }

    public void GreaterThanUnsigned( [GreaterThan( 42ul )] int x ) { }

    public void RangeUnsigned( [Range( 10ul, 20ul )] int x ) { }

    public void StrictlyGreaterThanUnsigned( [StrictlyGreaterThan( 42ul )] int x ) { }

    public void StrictlyLessThanUnsigned( [StrictlyLessThan( 42ul )] int x ) { }

    public void StrictRangeUnsigned( [StrictRange( 10ul, 20ul )] int x ) { }
}