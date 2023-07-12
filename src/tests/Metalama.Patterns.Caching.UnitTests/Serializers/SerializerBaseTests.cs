// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;
using Metalama.Patterns.Caching.Dependencies;
using Metalama.Patterns.Caching.Serializers;
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
        private ISerializer serializer;

        protected SerializerBaseTests( ISerializer serializer )
        {
            this.serializer = serializer;
        }

        protected object RoundTrip( object cacheItem )
        {
            var serialization = this.serializer.Serialize( cacheItem );
            var newCacheItem = this.serializer.Deserialize( serialization );

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