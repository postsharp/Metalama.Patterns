// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Serializers;
using Xunit;

namespace Metalama.Patterns.Caching.Tests.Serializers
{
    public abstract class SerializerBaseTests
    {
        private readonly ISerializer _serializer;

        protected SerializerBaseTests( ISerializer serializer )
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
        public void TestNullValue()
        {
            var roundTrip = this.RoundTrip( null );

            Assert.Null( roundTrip );
        }
    }
}