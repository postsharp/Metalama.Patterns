// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.Tests;

public class StringLengthTestClass
{
    [StringLength( 5, 10 )]
    public string StringField;

    public string StringMethod( [StringLength( 10 )] string parameter ) => parameter!;
}