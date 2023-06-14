// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// #ExpectedMessage(COM010/Range_InvalidType_Byte3.cs[14,25-14,33])

using Metalama.Patterns.Contracts;

public class Range_InvalidType_Byte3
{
    private byte field;

    [return: Range(-50, -20)]
    public byte GetField()
    {
        return this.field;
    }
}