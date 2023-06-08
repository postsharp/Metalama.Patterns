// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Patterns.Common.Tests;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;

using PostSharp.Patterns.Contracts;

namespace PostSharp.Patterns.Contracts.Tests
{
    public class InterfaceTests
    {
        [Fact]
        public void TestInterfaceImpl()
        {
            AssertEx.Throws<ArgumentNullException>( () =>
             {
                 Foo f = new Foo();
                 f.Bar( null );
             } );
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
}
