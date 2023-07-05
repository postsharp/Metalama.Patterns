using Xunit;
using Metalama.Patterns.Caching.Dependencies;
using Metalama.Patterns.Caching.Serializers;
using Metalama.Patterns.Common.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.Tests.Serializers
{
    public abstract class SerializerBaseTests
    {
        ISerializer serializer;

        protected SerializerBaseTests( ISerializer serializer )
        {
            this.serializer = serializer;
        }

        protected object RoundTrip(object cacheItem)
        {
            byte[] serialization = this.serializer.Serialize( cacheItem );
            object newCacheItem = this.serializer.Deserialize( serialization );

            return newCacheItem;
        }

        [Fact]
        public void TestNullValue()
        {
            object roundTrip = this.RoundTrip(null);

            Assert.Null(roundTrip);
        }
    }
}
