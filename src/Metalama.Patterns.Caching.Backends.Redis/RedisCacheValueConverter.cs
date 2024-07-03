// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Metalama.Patterns.Caching.Backends.Redis;

internal sealed class RedisCacheValueConverter : JsonConverter<RedisCacheValue>
{
    public override RedisCacheValue Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        // Create a JsonDocument from the reader to read the properties
        using ( var doc = JsonDocument.ParseValue( ref reader ) )
        {
            var root = doc.RootElement;

            // Read SlidingExpiration property
            var slidingExpiration = TimeSpan.FromSeconds( root.GetProperty( "SlidingExpiration" ).GetDouble() );

            // Read Value property
            var valueElement = root.GetProperty( "Value" );

            object? value;

            switch ( valueElement.ValueKind )
            {
                case JsonValueKind.String:
                    value = valueElement.GetString();

                    break;

                case JsonValueKind.Number:
                    value = valueElement.GetDecimal();

                    break;

                case JsonValueKind.True:
                    value = true;

                    break;

                case JsonValueKind.False:
                    value = false;

                    break;

                case JsonValueKind.Object or JsonValueKind.Array:
                    var typeName = root.GetProperty( "ValueType" ).GetString()
                                   ?? throw new CachingAssertionFailedException( "Invalid cache item payload." );

                    var type = Type.GetType( typeName, throwOnError: true )!;
                    value = JsonSerializer.Deserialize( valueElement.GetRawText(), type, options );

                    break;

                case JsonValueKind.Null:
                    value = null;

                    break;

                default:
                    throw new NotSupportedException( $"Unexpected JSON token: {valueElement.ValueKind}" );
            }

            return new RedisCacheValue( value, slidingExpiration );
        }
    }

    public override void Write( Utf8JsonWriter writer, RedisCacheValue value, JsonSerializerOptions options )
    {
        writer.WriteStartObject();

        // Serialize SlidingExpiration property
        writer.WriteNumber( "SlidingExpiration", value.SlidingExpiration.TotalSeconds );

        // Serialize Value property

        if ( value.Value != null )
        {
            writer.WriteString( "ValueType", value.Value.GetType().AssemblyQualifiedName );

            writer.WritePropertyName( "Value" );
            JsonSerializer.Serialize( writer, value.Value, value.Value.GetType(), options );
        }
        else
        {
            writer.WriteNull( "Value" );
        }

        writer.WriteEndObject();
    }
}