// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// #ExpectedMessage(COM010)

using Metalama.Patterns.Contracts;

public class Range_UlongStrictlyLessThanUlong
{
    private void MethodWithUlongStrictlyLessThanUlong( [StrictlyLessThan( ulong.MinValue )] ulong? a )
    {
    }
}