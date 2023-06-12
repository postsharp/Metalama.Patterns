// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

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
    
    interface IFoo
    {
        void Bar( [Required] string a );
    }

    class Foo : IFoo
    {
        public void Bar( string a )
        {
            
        }
    }
}