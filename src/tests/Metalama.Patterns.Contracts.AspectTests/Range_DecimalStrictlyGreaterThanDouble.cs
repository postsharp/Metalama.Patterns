// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// #ExpectedMessage(COM010)

using Metalama.Patterns.Contracts;

public class Range_DecimalStrictlyGreaterThanDouble
{
    private void MethodWithDecimalStrictlyGreaterThanDouble(
        [StrictlyGreaterThan( (double) decimal.MaxValue + (double) decimal.MaxValue*1e-6 )] decimal? a )
    {
    }
}