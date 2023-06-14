// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.Tests;

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

    public string SetEmail( [RegularExpression( ".+@.+" )] string email ) => email!;

    [RegularExpression( @"^[a-z]{4}$" )]
    public string PatternEscaping1;

    [RegularExpression( @"^\{[a-z]{4}}$" )]
    public string PatternEscaping2;
}