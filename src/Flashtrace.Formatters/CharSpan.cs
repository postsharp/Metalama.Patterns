// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// Represents a span of <see cref="char"/> by encapsulating a substring or a range of a <see cref="char"/> array.
/// </summary>
[PublicAPI]
public readonly struct CharSpan : CharSpan.IArrayAccessor
{
    /* TODO: [FT-Review] Review explicit interface-based replacement pattern. I considered exposing Array publicly to be too unsafe. Cleanup when confirmed.
    [ExplicitCrossPackageInternal]
    internal object _array { get;  }
    */

    /// <summary>
    /// Provides access to the internal storage of <see cref="CharSpan"/>. 
    /// </summary>
    [PublicAPI]
    public interface IArrayAccessor
    {
        /// <summary>
        /// Gets the internal storage object. <see cref="Array"/> will be <see langword="null"/>, array of <see cref="char"/>, or <see cref="string"/>.
        /// Do not mutate the returned object.
        /// </summary>
        object? Array { get; }
    }

    internal int StartIndex { get; }

    /// <summary>
    /// Gets the number of <see cref="char"/> in the span.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets a value indicating whether the current instance represents a null string.
    /// </summary>
    public bool IsNull => this.Array == null;

    object? IArrayAccessor.Array => this.Array;

    internal object? Array { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharSpan"/> struct from an array of <see cref="char"/>.
    /// </summary>
    /// <param name="array">An array of <see cref="char"/>.</param>
    /// <param name="start">The start index of the span in the <paramref name="array"/>.</param>
    /// <param name="length">The number of characters in the span.</param>
    public CharSpan( char[]? array, int start, int length )
    {
        this.Array = array;
        this.StartIndex = start;
        this.Length = length;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharSpan"/> struct from a <see cref="string"/> and specifies the start and lenght of the substring.
    /// </summary>
    /// <param name="str">A string.</param>
    /// <param name="start">The index of the first character of the span in <paramref name="str"/>.</param>
    /// <param name="length">The number of characters in the span.</param>
    public CharSpan( string? str, int start, int length )
    {
        this.Array = str;
        this.StartIndex = start;
        this.Length = length;
    }

    // ReSharper disable once MergeConditionalExpression

    /// <summary>
    /// Initializes a new instance of the <see cref="CharSpan"/> struct from a <see cref="string"/>, and takes the whole string.
    /// </summary>
    /// <param name="str">A string.</param>
    public CharSpan( string? str ) : this( str, 0, str == null ? 0 : str.Length ) { }

    /// <summary>
    /// Converts a <see cref="string"/> into a <see cref="CharSpan"/>.
    /// </summary>
    /// <param name="str"></param>
    public static implicit operator CharSpan( string str ) => FromString( str );

    /// <summary>
    /// Converts a <see cref="string"/> into a <see cref="CharSpan"/>.
    /// </summary>
    /// <param name="str"></param>
    public static CharSpan FromString( string str ) => new( str );

    /// <summary>
    /// Converts an <see cref=" System.ArraySegment{T}"/> into a <see cref="CharSpan"/>.
    /// </summary>
    /// <param name="str"></param>
    public static implicit operator CharSpan( ArraySegment<char> str ) => FromArraySegment( str );

    /// <summary>
    /// Converts a <see cref="string"/> into a <see cref="CharSpan"/>.
    /// </summary>
    /// <param name="str"></param>
    public static CharSpan FromArraySegment( ArraySegment<char> str ) => new( str.Array, str.Offset, str.Count );

    /// <summary>
    /// Gets a value indicating whether the current <see cref="CharSpan"/> is backed by a <c>char[]</c>. In this case,
    /// the <see cref="ToCharArraySegment"/> method does not allocate memory.
    /// </summary>
    public bool IsBackedByCharArray => this.Array is char[];

    /// <inheritdoc/>
    public override string? ToString()
    {
        switch ( this.Array )
        {
            case string s:
                return s.Substring( this.StartIndex, this.Length );

            case char[] c:
                return new string( c, this.StartIndex, this.Length );

            default:
                return null;
        }
    }

    /// <summary>
    /// Converts the current <see cref="CharSpan"/> into an <see cref="System.ArraySegment{T}"/> of <see cref="char"/>.
    /// When the <see cref="IsBackedByCharArray"/> or <see cref="IsNull"/> property is <c>true</c>, this method does not allocate memory.
    /// </summary>
    /// <returns></returns>
    public ArraySegment<char> ToCharArraySegment()
    {
        switch ( this.Array )
        {
            case string s:
                return new ArraySegment<char>( s.ToCharArray(), this.StartIndex, this.Length );

            case char[] c:
                return new ArraySegment<char>( c, this.StartIndex, this.Length );

            default:
                return default;
        }
    }
}