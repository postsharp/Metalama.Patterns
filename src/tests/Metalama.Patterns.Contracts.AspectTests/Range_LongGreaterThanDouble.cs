// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// #ExpectedMessage(COM010)

using Metalama.Patterns.Contracts;

public class Range_LongGreaterThanDouble
{
    private void MethodWithLongGreaterThanDouble( [GreaterThan( (double) long.MaxValue + (double) long.MaxValue*1e-6 )] long? a )
    {
    }
}