// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Serializers;
using System.Runtime.Serialization;
using Xunit;

namespace Metalama.Patterns.Caching.Tests.Serializers
{
    public abstract class SerializerBaseTests
    {
        private readonly ICachingSerializer _serializer;

        protected SerializerBaseTests( ICachingSerializer serializer )
        {
            this._serializer = serializer;
        }

        protected object? RoundTrip( object? cacheItem )
        {
            var serialization = this._serializer.Serialize( cacheItem );
            var newCacheItem = this._serializer.Deserialize( serialization );

            return newCacheItem;
        }

        [Fact]
        public void TestDictionary()
        {
            var dictionary = new Dictionary<string, int>() { ["1"] = 1, ["2"] = 2 };
            var roundTrip = this.RoundTrip( dictionary );
            Assert.Equivalent( dictionary, roundTrip );
        }

        [Fact]
        public void TestNullValue()
        {
            var roundTrip = this.RoundTrip( null );

            Assert.Null( roundTrip );
        }

        [Fact]
        public void TestObject()
        {
            var o = new MyObject();
            var roundTripItem = (MyObject?) this.RoundTrip( o );
            Assert.Equal( o.Value, roundTripItem!.Value );
        }

        [DataContract]
        [Serializable]
        private sealed class MyObject
        {
            // ReSharper disable once MemberCanBePrivate.Local
#pragma warning disable SA1401
            public static int NextValue = 10;
#pragma warning restore SA1401

            [DataMember]
#pragma warning disable SA1401
            public int Value { get; set; } = NextValue++;
#pragma warning restore SA1401
        }
    }
}