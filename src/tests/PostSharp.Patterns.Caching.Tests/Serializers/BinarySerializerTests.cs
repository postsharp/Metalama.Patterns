using System;
using System.Linq;
using Xunit;
using PostSharp.Patterns.Caching.Serializers;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests.Serializers
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
