// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// #ExpectedMessage(COM010/Range_InvalidType_Byte2.cs[12,25-12,30])

using Metalama.Patterns.Contracts;

public class Range_InvalidType_Byte2
{
    [StrictlyGreaterThan(255)]
    public byte Field { get; set; }
}