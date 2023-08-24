// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.UnitTests.Assets;

public class TestException : Exception
{
    public TestException( string message ) : base( message ) { }
}