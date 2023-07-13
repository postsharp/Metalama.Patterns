// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Serializers;
using Xunit;

namespace Metalama.Patterns.Caching.Tests.Serializers
{
    public sealed class BinarySerializerTests : SerializerBaseTests
    {
        public BinarySerializerTests() : base( new BinarySerializer() ) { }

        [Fact]
        public void TestObject()
        {
            var o = new MyObject();
            var roundTripItem = this.RoundTrip( o );
            Assert.IsType<MyObject>( roundTripItem );
            Assert.Equal( o.Value, ((MyObject) roundTripItem).Value );
        }

        [Serializable]
        private class MyObject
        {
#pragma warning disable SA1401
            public static int NextValue = 10;
            public int Value = NextValue++;
#pragma warning restore SA1401
        }
    }
}