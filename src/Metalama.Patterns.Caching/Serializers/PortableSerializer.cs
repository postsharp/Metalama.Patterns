// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Serializers;

// TODO: [Porting] Should we still use PortableSerializer/PortableFormatter? Remove disables once decided.

// ReSharper disable once InvalidXmlDocComment
/// <summary>
/// An implementation of <see cref="ISerializer"/> that uses the <see cref="PortableFormatter"/>
/// (for classes annotated with <see cref="PSerializableAttribute"/>).
/// </summary>
[Obsolete( "Porting TODO", true )]

// ReSharper disable once UnusedType.Global
public sealed class PortableSerializer : ISerializer
{
#if TODO
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
#endif

    /// <inheritdoc />
    public byte[] Serialize( object? value )
    {
#if !TODO
        throw new NotImplementedException( "TODO" );
#else
            if ( value == null )
                return ArrayHelper.Empty<byte>();

            using ( MemoryStream stream = new MemoryStream() )
            {
                this.serializer.Serialize( value, stream );
                return stream.ToArray();
            }
#endif
    }

    /// <inheritdoc />
    public object Deserialize( byte[]? array )
    {
#if !TODO
        throw new NotImplementedException( "TODO" );
#else
            if ( array == null || array.Length == 0 )
                return null;

            using ( MemoryStream stream = new MemoryStream( array ) )
            {
                return this.serializer.Deserialize( stream );
            }
#endif
    }
}