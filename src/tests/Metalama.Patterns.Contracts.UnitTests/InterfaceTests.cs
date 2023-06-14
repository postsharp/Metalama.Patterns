// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Tests.Helpers;
using Xunit;

namespace Metalama.Patterns.Contracts.Tests;

public class InterfaceTests
{
    // TODO: Review. This test fails.
    [Fact]
    public void TestInterfaceImpl()
    {
        var cut = new Foo();
        var e = TestHelpers.RecordException<ArgumentNullException>( () => cut.Bar( null! ) );
        Assert.NotNull( e );
    }

    private interface IFoo
    {
        void Bar( [Required] string a );
    }

    private class Foo : IFoo
    {
        public void Bar( string a )
        {
        }
    }
}