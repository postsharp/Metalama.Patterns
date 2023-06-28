// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Serializers;

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
    byte[] Serialize( object? value );

    /// <summary>
    /// Deserializes a byte array into an object.
    /// </summary>
    /// <param name="array">A byte array.</param>
    /// <returns>The object represented by <paramref name="array"/>.</returns>
    object? Deserialize( byte[]? array );
}