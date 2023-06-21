// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Flashtrace.Custom
{
    /// <summary>
    /// Parses the formatting string for custom messages of the <see cref="Logger"/> and <see cref="LogActivity"/> classes.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815")]
    public struct FormattingStringParser
    {
        private readonly string str;
        private readonly char[] charArray;
        private int position;
        private bool hasParameter;

        /// <summary>
        /// Initializes a new <see cref="FormattingStringParser"/>.
        /// </summary>
        /// <param name="formattingString">The formatting string.</param>
        public FormattingStringParser(string formattingString)
        {
            this.str = formattingString;
            this.charArray = formattingString.ToCharArray();
            this.position = 0;
            this.hasParameter = false;
        }


        private int GetIndexOf( char separator, out bool unescape )
        {
            unescape = false;

            if (this.position >= this.charArray.Length)
                return -1;

            int cursor = this.position;

            while ( true )
            {
                int index = this.str.IndexOf(separator, cursor);
                if (index < 0)
                {
                    return -1;
                }

                if ( index < this.str.Length-1 && this.str[index+1] == separator )
                {
                    unescape = true;
                    cursor = index + 2;

                    if (cursor >= this.charArray.Length)
                        return -1;

                    continue;
                }

                return index;
            }
        }

        /// <summary>
        /// Gets the next substring (until the next parameter or the end of the string).
        /// </summary>
        /// <returns>The next substring.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024")]
        public ArraySegment<char> GetNextSubstring()
        {
            bool unescape;
            int index = this.GetIndexOf('{', out unescape);
            int oldPosition = this.position;

            if (index < 0)
            {
                this.hasParameter = false;
                this.position = this.charArray.Length;
                return this.GetUnescapedString(oldPosition, this.charArray.Length - oldPosition, unescape);
            }
            else
            {
                this.hasParameter = true;
                this.position = index + 1;
                return this.GetUnescapedString(oldPosition, index - oldPosition, unescape);
            }
        }

        private ArraySegment<char> GetUnescapedString( int index, int count, bool unescape )
        {
            unescape = unescape || this.str.IndexOf("}}", index, count, StringComparison.Ordinal) >= 0 ;

            if ( unescape )
            {
                char[] unescaped = new char[count];
                int unescapedCharacterCount = 0;
                for (int i = 0; i < count; i++)
                {
                    unescaped[unescapedCharacterCount] = this.charArray[index+i];
                    unescapedCharacterCount++;
                    if (i < count - 1 && (this.charArray[index+i] == '{' || this.charArray[index + i] == '}') && this.charArray[index + i] == this.charArray[index + i + 1])
                    {
                        i++;
                    }
                }

                return new ArraySegment<char>(unescaped, 0, unescapedCharacterCount);
            }
            else
            {
                return new ArraySegment<char>(this.charArray, index, count);
            }
        }

      
        /// <summary>
        /// Gets the next parameter name.
        /// </summary>
        /// <returns>The name of the next parameter, or the default (null) value if there is no next parameter.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024")]
        [SuppressMessage("Microsoft.Performance", "CA1815")]
        public ArraySegment<char> GetNextParameter()
        {
            if ( !this.hasParameter )
            {
                return default(ArraySegment<char>);
            }

            bool unescape;
            int index = this.GetIndexOf('}', out unescape);

            if ( unescape )
            {
                throw new InvalidFormattingStringException("Parameter names cannot contain escaped brackets.");
            }

            int oldPosition = this.position;

            if (index < 0)
            {
                throw new InvalidFormattingStringException(string.Format( CultureInfo.InvariantCulture, "Expected character '}}' after string index {0}.", this.position));
            }

            this.position = index + 1;

            return new ArraySegment<char>(this.charArray, oldPosition, index - oldPosition);
        }
    }
}
