// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Runtime.Serialization.Formatters.Binary;

namespace Metalama.Patterns.Caching.Serializers;

/// <summary>
/// An implementation of <see cref="BinaryFormatter"/> that uses <see cref="BinaryFormatter"/>
/// (for classes annotated with <see cref="SerializableAttribute"/>).
/// </summary>
[Obsolete( "The BinaryFormatter facility is considered obsolete." )]
public sealed class BinaryCachingSerializer : ICachingSerializer
{
    private readonly BinaryFormatter _serializer = new();

    /// <inheritdoc />
    public byte[] Serialize( object? value )
    {
        if ( value == null )
        {
            return Array.Empty<byte>();
        }

        using ( var stream = new MemoryStream() )
        {
            this._serializer.Serialize( stream, value );

            return stream.ToArray();
        }
    }

    /// <inheritdoc />  
    public object? Deserialize( byte[]? array )
    {
        if ( array == null || array.Length == 0 )
        {
            return null;
        }

        using ( var stream = new MemoryStream( array ) )
        {
            return this._serializer.Deserialize( stream );
        }
    }
}