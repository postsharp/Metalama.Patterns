// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Text;

namespace Flashtrace.Formatters;

/// <summary>
/// Represents a span of <see cref="char"/> by encapsulating a substring or a range of a <see cref="char"/> array.
/// The benefit of <see cref="CharSpan"/> over <see cref="ReadOnlySpan{T}"/> is that it can wrap a string without allocating memory
/// even in .NET Framework.
/// </summary>
public readonly ref struct CharSpan
{
#if NET6_0_OR_GREATER
    private readonly ReadOnlySpan<char> _span;

    public int Length => this._span.Length;
#else
    public readonly int StartIndex;
    private readonly object? _data;
    
    /// <summary>
    /// Gets the number of <see cref="char"/> in the span.
    /// </summary>
    public int Length { get; }
#endif

    /// <summary>
    /// Gets a value indicating whether the current instance represents a null string.
    /// </summary>
    public bool IsEmpty => this.Length == 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="CharSpan"/> struct from an array of <see cref="char"/>.
    /// </summary>
    /// <param name="array">An array of <see cref="char"/>.</param>
    /// <param name="start">The start index of the span in the <paramref name="array"/>.</param>
    /// <param name="length">The number of characters in the span.</param>
    public CharSpan( char[]? array, int start, int length )
    {
#if NET6_0_OR_GREATER
        this._span = new ReadOnlySpan<char>( array, start, length );
#else
        this._data = array;
        this.StartIndex = start;
        this.Length = length;
#endif
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharSpan"/> struct from a <see cref="string"/> and specifies the start and lenght of the substring.
    /// </summary>
    /// <param name="str">A string.</param>
    /// <param name="start">The index of the first character of the span in <paramref name="str"/>.</param>
    /// <param name="length">The number of characters in the span.</param>
    public CharSpan( string? str, int start, int length )
    {
#if NET6_0_OR_GREATER
        if ( str != null )
        {
            this._span = ((ReadOnlySpan<char>) str).Slice( start, length );
        }
#else
        this._data = str;
        this.StartIndex = start;
        this.Length = length;
#endif
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

    /// <inheritdoc/>
    public override string? ToString()
    {
#if NET6_0_OR_GREATER
        return new string( this._span );
#else
        switch ( this._data )
        {
            case string s:
                return s.Substring( this.StartIndex, this.Length );

            case char[] c:
                return new string( c, this.StartIndex, this.Length );

            default:
                return null;
        }
#endif
    }

    public void AppendToStringBuilder( StringBuilder stringBuilder )
    {
#if NET6_0_OR_GREATER
        stringBuilder.Append( this._span );
#else
        switch ( this._data )
        {
            case string s:
                stringBuilder.Append( s, this.StartIndex, this.Length );

                break;

            case char[] c:
                stringBuilder.Append( c, this.StartIndex, this.Length );

                break;
        }
#endif
    }

    public bool AppendToStringBuilder( UnsafeStringBuilder stringBuilder )
    {
#if NET6_0_OR_GREATER
        return stringBuilder.Append( this._span );
#else
        switch ( this._data )
        {
            case string s:
                return stringBuilder.Append( s, this.StartIndex, this.Length );

            case char[] c:
                return stringBuilder.Append( c, this.StartIndex, this.Length );

            default:
                throw new FormattersAssertionFailedException();
        }
#endif
    }
}