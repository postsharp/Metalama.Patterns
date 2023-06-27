// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETFRAMEWORK
#define NET_DATA_CONTRACT_SERIALIZER
#endif

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Metalama.Patterns.Caching.Serializers
{
    /// <summary>
    /// An implementation of <see cref="ISerializer"/> that uses <see cref="NetDataContractSerializer"/>.
    /// You can derive this class to use a different <see cref="XmlObjectSerializer"/>.
    /// </summary>
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
#if !NET_DATA_CONTRACT_SERIALIZER
    [EditorBrowsable( EditorBrowsableState.Never )]
#endif
    public class DataContractSerializer : ISerializer
    {
        /// <summary>
        /// Initializes a new <see cref="DataContractSerializer"/>.
        /// </summary>
        public DataContractSerializer()
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
        public byte[] Serialize( object value )
        {
#if NET_DATA_CONTRACT_SERIALIZER
            if ( value == null )
                return Array.Empty<byte>();

            XmlObjectSerializer serializer = this.CreateSerializer();

            using ( MemoryStream stream = new MemoryStream() )
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
        public object Deserialize( byte[] array )
        {
#if NET_DATA_CONTRACT_SERIALIZER
            if ( array == null || array.Length == 0 )
                return null;

            XmlObjectSerializer serializer = this.CreateSerializer();

            using ( MemoryStream stream = new MemoryStream( array ) )
            {
                return serializer.ReadObject( stream );
            }
#else
            throw new PlatformNotSupportedException();
#endif
        }
    }
}