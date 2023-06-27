// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace PostSharp.Patterns.Caching.Serializers
{
    /// <summary>
    /// Serializes an object into a byte array and deserializes the byte array back.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes an object into a byte array.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A byte array representing <paramref name="value"/>.</returns>
        byte[] Serialize( object value );

        /// <summary>
        /// Deserializes a byte array into an object.
        /// </summary>
        /// <param name="array">A byte array.</param>
        /// <returns>The object represented by <paramref name="array"/>.</returns>
        object Deserialize( byte[] array );
    }

    
}
