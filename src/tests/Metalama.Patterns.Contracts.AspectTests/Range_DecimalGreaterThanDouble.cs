﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts;

public class Range_DecimalGreaterThanDouble
{
    public void MethodWithDecimalGreaterThanDouble( [GreaterThan( (double) decimal.MaxValue + ((double) decimal.MaxValue * 1e-6) )] decimal? a )
    {
    }
}