// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests.TestHelpersTests
{
    public sealed class CachingClassTests
    {
        [Fact]
        public void TestReset()
        {
            CachingClass cachingClass = new CachingClass();

            bool called = cachingClass.Reset();
            Assert.False( called, "It is indicated the method has been called on a fresh instance." );

            cachingClass.GetValue();
            called = cachingClass.Reset();
            Assert.True( called, "It is indicated the method has not been called after we have called it." );

            called = cachingClass.Reset();
            Assert.False( called, "It is indicated the method has been called after the flag was reset." );
        }

        [Fact]
        public async Task TestAsyncReset()
        {
            CachingClass cachingClass = new CachingClass();

            Task<CachedValueClass> valueTask = cachingClass.GetValueAsync();
            bool called = cachingClass.Reset();
            Assert.False( called, "The caching method was called before awaiting the first value." );
            await valueTask;
            called = cachingClass.Reset();
            Assert.True( called, "The method was not called when awaiting the first value." );

            called = cachingClass.Reset();
            Assert.False( called, "It is indicated the method has been called after the flag was reset." );
        }

        [Fact]
        public void TestCounter()
        {
            CachingClass cachingClass = new CachingClass();

            CachedValueClass value0 = cachingClass.GetValue();
            cachingClass.Reset();
            CachedValueClass value1 = cachingClass.GetValue();

            AssertEx.NotEqual( value0, value1, "The method returned the same objects twice." );
        }

        [Fact]
        public async Task TestAsyncCounter()
        {
            CachingClass cachingClass = new CachingClass();

            Task<CachedValueClass> valueTask = cachingClass.GetValueAsync();
            CachedValueClass value0 = await valueTask;
            cachingClass.Reset();

            valueTask = cachingClass.GetValueAsync();
            CachedValueClass value1 = await valueTask;

            AssertEx.NotEqual( value0, value1, "The method returned the same objects twice." );
        }
    }
}