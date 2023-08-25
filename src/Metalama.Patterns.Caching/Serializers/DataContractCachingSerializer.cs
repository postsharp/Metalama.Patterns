// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETFRAMEWORK
#define NET_DATA_CONTRACT_SERIALIZER
#else
using System.ComponentModel;
#endif
using JetBrains.Annotations;
using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching.Serializers;

// ReSharper disable once InvalidXmlDocComment
/// <summary>
/// An implementation of <see cref="ICachingSerializer"/> that uses <see cref="NetDataContractSerializer"/>.
/// You can derive this class to use a different <see cref="XmlObjectSerializer"/>.
/// </summary>
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
#if !NET_DATA_CONTRACT_SERIALIZER
[EditorBrowsable( EditorBrowsableState.Never )]
#endif
[PublicAPI]
public class DataContractCachingSerializer : ICachingSerializer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DataContractCachingSerializer"/> class.
    /// </summary>
    public DataContractCachingSerializer()
    {
#if !NET_DATA_CONTRACT_SERIALIZER
        throw new PlatformNotSupportedException();
#endif
    }

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Creates a new <see cref="XmlObjectSerializer"/>. The default implementation creates a <see cref="NetDataContractSerializer"/>.
    /// </summary>
    /// <returns>A new <see cref="XmlObjectSerializer"/>.</returns>
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
#if !NET_DATA_CONTRACT_SERIALIZER
    [EditorBrowsable( EditorBrowsableState.Never )]
#endif
    protected virtual XmlObjectSerializer CreateSerializer()
    {
#if NET_DATA_CONTRACT_SERIALIZER
        return new NetDataContractSerializer();
#else
        throw new PlatformNotSupportedException();
#endif
    }

    /// <inheritdoc />
#if !NET_DATA_CONTRACT_SERIALIZER
    [EditorBrowsable( EditorBrowsableState.Never )]
#endif
    public byte[] Serialize( object? value )
    {
#if NET_DATA_CONTRACT_SERIALIZER
        if ( value == null )
        {
            return Array.Empty<byte>();
        }

        var serializer = this.CreateSerializer();

        using ( var stream = new MemoryStream() )
        {
            serializer.WriteObject( stream, value );

            return stream.ToArray();
        }
#else
        throw new PlatformNotSupportedException();
#endif
    }

    /// <inheritdoc />
#if !NET_DATA_CONTRACT_SERIALIZER
    [EditorBrowsable( EditorBrowsableState.Never )]
#endif

    // ReSharper disable once ReturnTypeCanBeNotNullable
    public object? Deserialize( byte[]? array )
    {
#if NET_DATA_CONTRACT_SERIALIZER
        if ( array == null || array.Length == 0 )
        {
            return null;
        }

        var serializer = this.CreateSerializer();

        using ( var stream = new MemoryStream( array ) )
        {
            return serializer.ReadObject( stream );
        }
#else
        throw new PlatformNotSupportedException();
#endif
    }
}