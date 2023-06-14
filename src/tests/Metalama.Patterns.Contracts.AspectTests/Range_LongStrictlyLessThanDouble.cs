// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// #ExpectedMessage(COM010)

using Metalama.Patterns.Contracts;

public class Range_LongStrictlyLessThanDouble
{
    private void MethodWithLongStrictlyLessThanDouble( [StrictlyLessThan( (double) long.MinValue + (double) long.MinValue*1e-6 )] long? a )
    {
    }
}