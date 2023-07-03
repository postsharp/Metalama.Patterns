// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Activities;
using JetBrains.Annotations;
using System.Globalization;

namespace Flashtrace.Messages;

/// <summary>
/// Parses the formatting string for messages of the <see cref="LogLevelSource"/> and <see cref="LogActivity{TActivityDescription}"/> classes.
/// </summary>
[PublicAPI]
public struct FormattingStringParser
{
    private readonly string _str;
    private readonly char[] _charArray;
    private int _position;
    private bool _hasParameter;

    /// <summary>
    /// Initializes a new instance of the <see cref="FormattingStringParser"/> struct.
    /// Initializes a new <see cref="FormattingStringParser"/>.
    /// </summary>
    /// <param name="formattingString">The formatting string.</param>
    public FormattingStringParser( string formattingString )
    {
        this._str = formattingString;
        this._charArray = formattingString.ToCharArray();
        this._position = 0;
        this._hasParameter = false;
    }

    private int GetIndexOf( char separator, out bool unescape )
    {
        unescape = false;

        if ( this._position >= this._charArray.Length )
        {
            return -1;
        }

        var cursor = this._position;

        while ( true )
        {
            var index = this._str.IndexOf( separator, cursor );

            if ( index < 0 )
            {
                return -1;
            }

            if ( index < this._str.Length - 1 && this._str[index + 1] == separator )
            {
                unescape = true;
                cursor = index + 2;

                if ( cursor >= this._charArray.Length )
                {
                    return -1;
                }

                continue;
            }

            return index;
        }
    }

    /// <summary>
    /// Gets the next substring (until the next parameter or the end of the string).
    /// </summary>
    /// <returns>The next substring.</returns>
    public ArraySegment<char> GetNextSubstring()
    {
        var index = this.GetIndexOf( '{', out var unescape );
        var oldPosition = this._position;

        if ( index < 0 )
        {
            this._hasParameter = false;
            this._position = this._charArray.Length;

            return this.GetUnescapedString( oldPosition, this._charArray.Length - oldPosition, unescape );
        }
        else
        {
            this._hasParameter = true;
            this._position = index + 1;

            return this.GetUnescapedString( oldPosition, index - oldPosition, unescape );
        }
    }

    private ArraySegment<char> GetUnescapedString( int index, int count, bool unescape )
    {
        unescape = unescape || this._str.IndexOf( "}}", index, count, StringComparison.Ordinal ) >= 0;

        if ( unescape )
        {
            var unescaped = new char[count];
            var unescapedCharacterCount = 0;

            for ( var i = 0; i < count; i++ )
            {
                unescaped[unescapedCharacterCount] = this._charArray[index + i];
                unescapedCharacterCount++;

                if ( i < count - 1 && (this._charArray[index + i] == '{' || this._charArray[index + i] == '}')
                                   && this._charArray[index + i] == this._charArray[index + i + 1] )
                {
                    i++;
                }
            }

            return new ArraySegment<char>( unescaped, 0, unescapedCharacterCount );
        }
        else
        {
            return new ArraySegment<char>( this._charArray, index, count );
        }
    }

    /// <summary>
    /// Gets the next parameter name.
    /// </summary>
    /// <returns>The name of the next parameter, or the default (null) value if there is no next parameter.</returns>
    public ArraySegment<char> GetNextParameter()
    {
        if ( !this._hasParameter )
        {
            return default;
        }

        var index = this.GetIndexOf( '}', out var unescape );

        if ( unescape )
        {
            throw new InvalidFormattingStringException( "Parameter names cannot contain escaped brackets." );
        }

        var oldPosition = this._position;

        if ( index < 0 )
        {
            throw new InvalidFormattingStringException(
                string.Format( CultureInfo.InvariantCulture, "Expected character '}}' after string index {0}.", this._position ) );
        }

        this._position = index + 1;

        return new ArraySegment<char>( this._charArray, oldPosition, index - oldPosition );
    }
}