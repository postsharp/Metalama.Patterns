// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Linq;
using Xunit;
using Metalama.Patterns.Caching.Serializers;
using Metalama.Patterns.Common.Tests.Helpers;
using Metalama.Serialization;

namespace Metalama.Patterns.Caching.Tests.Serializers
{
    public class PortableSerializerTests : SerializerBaseTests
    {
        public PortableSerializerTests() : base( new PortableSerializer() ) { }

        [Fact]
        public void TestObject()
        {
            var o = new MyObject();
            var roundTripItem = (MyObject) this.RoundTrip( o );
            Assert.Equal( o.Value, roundTripItem.Value );
        }

        [PSerializable]
        private class MyObject
        {
            public static int NextValue = 10;
            public int Value = (NextValue++);
        }
    }
}