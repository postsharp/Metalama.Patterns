// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETFRAMEWORK
using System.Runtime.Serialization;
using Xunit;

namespace Metalama.Patterns.Caching.Tests.Serializers;

public sealed class DataContractSerializerTests : SerializerBaseTests
{
    public DataContractSerializerTests() : base( new Caching.Serializers.DataContractSerializer() ) { }

    [Fact]
    public void TestObject()
    {
        var o = new MyObject();
        var roundTripItem = (MyObject?) this.RoundTrip( o );
        Assert.Equal( o.Value, roundTripItem!.Value );
    }

    [DataContract]
    private sealed class MyObject
    {
        // ReSharper disable once MemberCanBePrivate.Local
#pragma warning disable SA1401
        public static int NextValue = 10;
#pragma warning restore SA1401

        [DataMember]
#pragma warning disable SA1401
        public int Value = NextValue++;
#pragma warning restore SA1401
    }
}

#endif