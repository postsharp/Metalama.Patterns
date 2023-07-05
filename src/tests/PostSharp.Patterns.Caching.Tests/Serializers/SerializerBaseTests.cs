using Xunit;
using PostSharp.Patterns.Caching.Dependencies;
using PostSharp.Patterns.Caching.Serializers;
using PostSharp.Patterns.Common.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostSharp.Patterns.Caching.Tests.Serializers
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
