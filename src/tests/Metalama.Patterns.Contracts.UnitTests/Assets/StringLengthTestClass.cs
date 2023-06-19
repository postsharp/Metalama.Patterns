// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.UnitTests;

public class StringLengthTestClass
{
    [StringLength( 5, 10 )]
    public string StringField;

    // Incorrect warning at build time, but no squiggly.
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8603 // Possible null reference return.
    public string StringMethod( [StringLength( 10 )] string parameter ) => parameter;
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore IDE0079 // Remove unnecessary suppression
}