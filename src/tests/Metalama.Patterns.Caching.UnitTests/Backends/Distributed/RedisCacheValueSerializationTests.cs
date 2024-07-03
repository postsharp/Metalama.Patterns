// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Backends.Redis;
using Metalama.Patterns.Caching.Tests.Serializers;

namespace Metalama.Patterns.Caching.Tests.Backends.Distributed;

public class RedisCacheValueSerializationTests : SerializerBaseTests
{
    public RedisCacheValueSerializationTests() : base( new RedisJsonCachingFormatter() ) { }

    protected override object? Wrap( object? o ) => new RedisCacheValue( o, TimeSpan.FromSeconds( 5 ) );

    protected override object? Unwrap( object? o ) => ((RedisCacheValue) o!).Value;
}