using System;
using System.Linq;
using Xunit;
using Metalama.Patterns.Caching.Serializers;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests.Serializers
{
    public class BinarySerializerTests : SerializerBaseTests
    {
        public BinarySerializerTests() : base(new BinarySerializer())
        {

        }



        [Fact]
        public void TestObject()
        {
            MyObject o = new MyObject();
            object roundTripItem = this.RoundTrip(o);
            Assert.IsType<MyObject>( roundTripItem );
            Assert.Equal(o.Value, ((MyObject)roundTripItem).Value);

        }

        [Serializable]
        class MyObject
        {
            public static int NextValue = 10;
            public int Value = (NextValue++);

        }
    }
}
