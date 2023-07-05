using System;
using System.Linq;
using Xunit;
using PostSharp.Patterns.Caching.Serializers;
using System.Runtime.Serialization;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests.Serializers
{
#if NET_DATA_CONTRACT_SERIALIZER
    public class DataContractSerializerTests : SerializerBaseTests
    {
        public DataContractSerializerTests() : base(new Caching.Serializers.DataContractSerializer())
        {

        }

        [Fact]
        public void TestObject()
        {
            MyObject o = new MyObject();
            MyObject roundTripItem = (MyObject) this.RoundTrip(o);
            Assert.Equal(o.Value, roundTripItem.Value);

        }

        [DataContract]
        class MyObject
        {
            public static int NextValue = 10;

            [DataMember]
            public int Value = (NextValue++);

        }
    }
#endif
}
