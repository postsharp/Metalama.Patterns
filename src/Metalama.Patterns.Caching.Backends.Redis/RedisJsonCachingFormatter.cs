// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Serializers;
using System.Text.Json;

namespace Metalama.Patterns.Caching.Backends.Redis;

public class RedisJsonCachingFormatter : JsonCachingFormatter
{
    public RedisJsonCachingFormatter( JsonSerializerOptions? options = null ) : base( GetOptions( options ) ) { }

    private static JsonSerializerOptions GetOptions( JsonSerializerOptions? options )
    {
        options ??= new JsonSerializerOptions();
        options.Converters.Add( new RedisCacheValueConverter() );
    }
}