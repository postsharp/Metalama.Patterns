// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Patterns.Utilities;
using PostSharp.Serialization;
using System.IO;

namespace PostSharp.Patterns.Caching.Serializers
{
    /// <summary>
    /// An implementation of <see cref="ISerializer"/> that uses the <see cref="PortableFormatter"/>
    /// (for classes annotated with <see cref="PSerializableAttribute"/>).
    /// </summary>
    public sealed class PortableSerializer : ISerializer
    {
        private readonly PortableFormatter serializer;

        /// <summary>
        /// Initializes a new <see cref="PortableSerializer"/> with a new default <see cref="PortableFormatter"/>.
        /// </summary>
        public PortableSerializer( ) : this( null )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="PortableSerializer"/> with a given <see cref="PortableFormatter"/>.
        /// </summary>
        /// <param name="serializer">A <see cref="PortableFormatter"/>, or <c>null</c> to use a new default <see cref="PortableFormatter"/>.</param>
        public PortableSerializer( PortableFormatter serializer )
        {
            this.serializer = serializer ?? new PortableFormatter();
        }


        /// <inheritdoc />
        public byte[] Serialize( object value )
        {
            if ( value == null )
                return ArrayHelper.Empty<byte>();

            using ( MemoryStream stream = new MemoryStream() )
            {
                this.serializer.Serialize( value, stream );
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
                return this.serializer.Deserialize( stream );
            }

        }
    }
}
