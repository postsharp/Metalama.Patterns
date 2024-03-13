// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.AspectTests.ObjectRange;

internal class C
{
    [return: StrictlyPositive]
    public object M( [Positive] object a, [Range( 0, 100 )] object b, [LessThan( 100 )] out object c )
    {
        c = a;

        return b;
    }
}