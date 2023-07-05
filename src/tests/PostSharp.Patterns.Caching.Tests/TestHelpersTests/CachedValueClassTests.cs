// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Xunit;
using PostSharp.Patterns.Caching.TestHelpers;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests.TestHelpersTests
{
    /// <summary>
    /// Summary description for CachedValueClassTests
    /// </summary>
    public sealed class CachedValueClassTests
    {
        [Fact]
        public void TestEquality()
        {
            CachedValueClass value0A = new CachedValueClass( 0 );
            CachedValueClass value0B = new CachedValueClass( 0 );
            CachedValueClass value1 = new CachedValueClass( 1 );

            AssertEx.Equal( value0A.GetHashCode(), value0B.GetHashCode(), "Two different instances with the same IDs have different hash codes." );
            AssertEx.Equal( value0A, value0B, "Two different instances with the same IDs are not considered equal." );
            AssertEx.NotEqual( value0A, value1, "Two different instances with different IDs are considered equal." );
        }

        [Fact]
        public void TestInequality()
        {
            CachedValueClass value0 = new CachedValueClass( 0 );
            CachedValueClass value1 = new CachedValueClass( 1 );

            AssertEx.NotEqual( value0, value1, "Two different instances with different IDs are considered equal." );
        }

        [Fact]
        public void TestEqualHashCodes()
        {
            CachedValueClass value0A = new CachedValueClass( 0 );
            CachedValueClass value0B = new CachedValueClass( 0 );

            AssertEx.Equal( value0A.GetHashCode(), value0B.GetHashCode(), "Two different instances with the same IDs have different hash codes." );
        }

        [Fact]
        public void TestUnequalHashCodes()
        {
            CachedValueClass value0 = new CachedValueClass( 0 );
            CachedValueClass value1 = new CachedValueClass( 1 );

            AssertEx.NotEqual( value0.GetHashCode(), value1.GetHashCode(), "Two different instances with different IDs have the same hash codes." );
        }
    }
}