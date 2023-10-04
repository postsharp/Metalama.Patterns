// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.AspectTests;

public sealed class RangeOnlyTestsRequiredBounds
{
    public void LessThan( [LessThan( 42 )] int x ) { }

    public void GreatherThan( [GreaterThan( 42 )] int x ) { }

    public void Negative( [Negative] int x ) { }

    public void Positive( [Positive] int x ) { }

    public void Range( [Range( 10, 20 )] int x ) { }

    public void StrictlyGreaterThan( [StrictlyGreaterThan( 42 )] int x ) { }
    
    public void StrictlyLessThan( [StrictlyLessThan( 42 )] int x ) { }

    public void StrictlyNegative( [StrictlyNegative] int x ) { }

    public void StrictlyPositive( [StrictlyPositive] int x ) { }

    public void StrictRange( [StrictRange( 10, 20 )] int x ) { }
}