// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

/// <summary>
/// Accesses a repository of formatters.
/// </summary>
public interface IFormatterRepository
{
    /// <summary>
    /// Gets the <see cref="IFormatter{T}"/> for type <typeparamref name="T"/>.
    /// </summary>
    IFormatter<T> Get<T>();

    /// <summary>
    /// Gets the <see cref="IFormatter"/> for the specified <see cref="Type"/>. 
    /// </summary>
    IFormatter Get( Type objectType );

    /// <summary>
    /// Gets the <see cref="FormattingRole"/> associated with the current formatter repository.
    /// </summary>
    FormattingRole Role { get; }
}