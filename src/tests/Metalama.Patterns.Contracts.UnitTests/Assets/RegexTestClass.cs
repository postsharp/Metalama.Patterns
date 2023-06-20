// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.UnitTests;

public class RegexTestClass
{
    [RegularExpression( ".+@.+" )]
    public string Email;

    [EmailAddress]
    public string Email2;

    [Phone]
    public string PhoneField;

    [Url]
    public string UrlField;

    // Incorrect warning at build time, but no squiggly.
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8603  // Possible null reference return.
    public string SetEmail( [RegularExpression( ".+@.+" )] string email ) => email;
#pragma warning restore CS8603  // Possible null reference return.
#pragma warning restore IDE0079 // Remove unnecessary suppression

    [RegularExpression( @"^[a-z]{4}$" )]
    public string PatternEscaping1;

    [RegularExpression( @"^\{[a-z]{4}}$" )]
    public string PatternEscaping2;
}