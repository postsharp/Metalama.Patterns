// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using System.Text.Json;

namespace Metalama.Patterns.Caching.Serializers;

/// <summary>
/// A serialized based on <c>System.Text.Json</c>.
/// </summary>
[PublicAPI]
public class JsonCachingFormatter : ICachingSerializer
{
    private readonly JsonSerializerOptions _options;

    public JsonCachingFormatter( JsonSerializerOptions? options = null )
    {
        this._options = options ?? new JsonSerializerOptions();
    }

    public byte[] Serialize( object? value )
    {
        if ( value == null )
        {
            return Array.Empty<byte>();
        }

        using ( var stream = new MemoryStream() )
        {
            // Write the assembly-qualified name.
            using var writer = new BinaryWriter( stream );
            writer.Write( this.GetTypeName( value.GetType() ) );

            // Write the JSON.
            JsonSerializer.Serialize( stream, value, this._options );

            return stream.ToArray();
        }
    }

    protected virtual string GetTypeName( Type type ) => type.AssemblyQualifiedName!;

    protected virtual Type ResolveTypeName( string assemblyQualifiedTypeName ) => Type.GetType( assemblyQualifiedTypeName, throwOnError: true )!;

    public object? Deserialize( byte[]? array )
    {
        if ( array == null || array.Length == 0 )
        {
            return null;
        }

        using ( var stream = new MemoryStream( array ) )
        {
            // Read the type name.
            using var reader = new BinaryReader( stream );
            var typeName = reader.ReadString();
            var type = this.ResolveTypeName( typeName );

            return JsonSerializer.Deserialize( stream, type, this._options );
        }
    }
}