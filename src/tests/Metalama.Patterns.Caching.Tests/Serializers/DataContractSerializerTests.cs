// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Linq;
using Xunit;
using Metalama.Patterns.Caching.Serializers;
using System.Runtime.Serialization;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests.Serializers
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