// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Runtime.Serialization.Formatters.Binary;

namespace Metalama.Patterns.Caching.Serializers
{
    /// <summary>
    /// An implementation of <see cref="BinaryFormatter"/> that uses <see cref="BinaryFormatter"/>
    /// (for classes annotated with <see cref="SerializableAttribute"/>).
    /// </summary>
    public sealed class BinarySerializer : ISerializer
    {
        private readonly BinaryFormatter serializer = new BinaryFormatter();

        /// <inheritdoc />
        public byte[] Serialize( object value )
        {
            if ( value == null )
                return Array.Empty<byte>();

            using ( MemoryStream stream = new MemoryStream() )
            {
#pragma warning disable SYSLIB0011
                this.serializer.Serialize( stream, value );
#pragma warning restore SYSLIB0011
                return stream.ToArray();
            }
        }

        /// <inheritdoc />  
        public object Deserialize( byte[] array )
        {
            if ( array == null || array.Length == 0 )
                return null;

            using ( MemoryStream stream = new MemoryStream( array ) )
            {
#pragma warning disable SYSLIB0011
                return this.serializer.Deserialize( stream );
#pragma warning restore SYSLIB0011
            }

        }
    }
}
