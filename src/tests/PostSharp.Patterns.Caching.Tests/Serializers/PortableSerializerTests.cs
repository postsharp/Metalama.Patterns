using System;
using System.Linq;
using Xunit;
using PostSharp.Patterns.Caching.Serializers;
using PostSharp.Patterns.Common.Tests.Helpers;
using PostSharp.Serialization;

namespace PostSharp.Patterns.Caching.Tests.Serializers
{
    public class PortableSerializerTests : SerializerBaseTests
    {
        public PortableSerializerTests() : base(new PortableSerializer())
        {
            
        }



        [Fact]
        public void TestObject()
        {
            MyObject o = new MyObject();
            MyObject roundTripItem = (MyObject) this.RoundTrip(o);
            Assert.Equal(o.Value, roundTripItem.Value);

        }

        [PSerializable]
        class MyObject
        {
            public static int NextValue = 10;
            public int Value = (NextValue++);

        }
    }
}
