// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Flashtrace.Formatters;

/// <summary>
/// A class similar to <see cref="System.Text.StringBuilder"/>, but implemented using unsafe C#.
/// </summary>
public sealed unsafe class UnsafeStringBuilder : IDisposable
{
   
    private char[] array;
    private char* cursor;
    private char* start;
    private char* end;
    private GCHandle gcHandle;
    private UnsafeString unsafeString;

    /// <summary>
    /// <c>true</c> if an <see cref="OverflowException"/> should be thrown when
    /// the buffer capacity is insufficient, <c>false</c> if the <c>Append</c> method should return <c>false</c> without exception.
    /// </summary>
    public bool ThrowOnOverflow { get; }

    /// <summary>
    /// Initializes a new <see cref="UnsafeStringBuilder"/> and allocates a new buffer.
    /// </summary>
    /// <param name="capacity">The capacity of the new <see cref="UnsafeStringBuilder"/>.</param>
    /// <param name="throwOnOverflow"><c>true</c> if an <see cref="OverflowException"/> should be thrown when
    /// the buffer capacity is insufficient, <c>false</c> if the <c>Append</c> method should return <c>false</c> without exception.</param>
    public UnsafeStringBuilder( int capacity = 2048, bool throwOnOverflow = true)
    {
        this.ThrowOnOverflow = throwOnOverflow;
        this.array = new char[capacity];
        this.gcHandle = GCHandle.Alloc( this.CharArray, GCHandleType.Pinned );
        this.cursor = this.start = (char*) this.gcHandle.AddrOfPinnedObject();
        this.end = this.start + capacity;
        this.unsafeString = new UnsafeString(this);
    }

    /// <summary>
    /// Initializes a new <see cref="UnsafeStringBuilder"/> with a pre-allocated buffer/
    /// </summary>
    /// <param name="buffer">Pointer to the buffer.</param>
    /// <param name="size">Number of <c>char</c> in the buffer.</param>
    /// <param name="throwOnOverflow"><c>true</c> if an <see cref="OverflowException"/> should be thrown when
    /// the buffer capacity is insufficient, <c>false</c> if the <c>Append</c> method should return <c>false</c> without exception.</param>
    public UnsafeStringBuilder( char* buffer, int size, bool throwOnOverflow = true)
    {
        this.ThrowOnOverflow = throwOnOverflow;
        this.array = null;
        this.start = buffer;
        this.cursor = buffer;
        this.end = this.start + size;
        this.unsafeString = new UnsafeString(this);
        GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Gets the capacity (number of <c>char</c>) of the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    public int Capacity => (int) (this.end - this.start);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private bool OnOverflow()
    {
        if ( this.ThrowOnOverflow )
        {
            throw new InvalidOperationException( "Cannot append to the UnsafeStringBuilder because it has insufficient capacity." );
        }

        return false;
    }

    /// <summary>
    /// Appends one <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c">A <c>char</c>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char c )
    {
        if ( this.cursor + 1 > this.end )
        {
            return this.OnOverflow();
        }

        *this.cursor = c;
        this.cursor++;

        return true;
    }

    /// <summary>
    /// Appends two <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c1">A <c>char</c>.</param>
    /// <param name="c2">A <c>char</c>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char c1, char c2 )
    {
        if ( this.cursor + 2 > this.end )
        {
            return this.OnOverflow();
        }

        *this.cursor = c1;
        *(this.cursor+1) = c2;
        this.cursor+=2;

        return true;
    }

    /// <summary>
    /// Appends three <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c1">A <c>char</c>.</param>
    /// <param name="c2">A <c>char</c>.</param>
    /// <param name="c3">A <c>char</c>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char c1, char c2, char c3 )
    {
        if ( this.cursor + 3 > this.end )
        {
            return this.OnOverflow();
        }

        *this.cursor = c1;
        *(this.cursor+1) = c2;
        *(this.cursor+2) = c3;
        this.cursor+=3;

        return true;
    }

    /// <summary>
    /// Appends four <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c1">A <c>char</c>.</param>
    /// <param name="c2">A <c>char</c>.</param>
    /// <param name="c3">A <c>char</c>.</param>
    /// <param name="c4">A <c>char</c>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char c1, char c2, char c3, char c4 )
    {
        if ( this.cursor + 4 > this.end )
        {
            return this.OnOverflow();
        }

        *this.cursor = c1;
        *(this.cursor+1) = c2;
        *(this.cursor+2) = c3;
        *(this.cursor+3) = c4;
        this.cursor+=4;

        return true;
    }

    /// <summary>
    /// Appends five <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c1">A <c>char</c>.</param>
    /// <param name="c2">A <c>char</c>.</param>
    /// <param name="c3">A <c>char</c>.</param>
    /// <param name="c4">A <c>char</c>.</param>
    /// <param name="c5">A <c>char</c>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char c1, char c2, char c3, char c4, char c5 )
    {
        if ( this.cursor + 5 > this.end )
        {
            return this.OnOverflow();
        }

        *this.cursor = c1;
        *(this.cursor+1) = c2;
        *(this.cursor+2) = c3;
        *(this.cursor+3) = c4;
        *(this.cursor+4) = c5;
        this.cursor+=5;

        return true;
    }

    /// <summary>
    /// Appends an array segment of <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c">A non-null array of <c>char</c>.</param>
    /// <param name="offset">Index of the first <c>char</c> to be appended.</param>
    /// <param name="count">Number of <c>char</c> to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "count*2")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char[] c, int offset, int count )
    {
        if ( c == null || c.Length == 0 )
        {
            return true;
        }

        if ( offset + count > c.Length )
        {
            throw new ArgumentOutOfRangeException(nameof(c));
        }

        if ( this.cursor + count > this.end )
        {
            return this.OnOverflow();
        }

        fixed ( char* theirArray = c )
        {
            BufferHelper.CopyMemory( this.cursor, theirArray + offset, count * sizeof(char));
        }

        this.cursor += count;

        return true;
    }

    /// <summary>
    /// Appends an unmanaged array of <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c">A non-null pointer to an unmanaged array of <c>char</c>.</param>
    /// <param name="count">Number of <c>char</c> to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "count*2")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char* c, int count )
    {
        if ( c == null || count == 0 )
        {
            return true;
        }

        if ( this.cursor + count > this.end )
        {
            return this.OnOverflow();
        }

        BufferHelper.CopyMemory( this.cursor, c, count * sizeof(char));
        this.cursor += count;

        return true;
    }

    /// <summary>
    /// Appends several times the same <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c">A <c>char</c>.</param>
    /// <param name="count">The number of times <paramref name="c"/> has to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append(char c, int count)
    {
        if (this.cursor + count > this.end)
        {
            return this.OnOverflow();
        }

        for ( int i = 0; i < count / 4; i++ )
        {
            *(this.cursor + 0) = c;
            *(this.cursor + 1) = c;
            *(this.cursor + 2) = c;
            *(this.cursor + 3) = c;
            this.cursor += 4;
        }

        switch ( count % 4 )
        {
            case 0:
                break;
            case 1:
                *(this.cursor + 0) = c;
                this.cursor += 1;
                break;
            case 2:
                *(this.cursor + 0) = c;
                *(this.cursor + 1) = c;
                this.cursor += 2;
                break;
            case 3:
                *(this.cursor + 0) = c;
                *(this.cursor + 1) = c;
                *(this.cursor + 2) = c;
                this.cursor += 3;
                break;
        }

        return true;
    }

    /// <summary>
    /// Appends an array of <c>char</c> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="c">A non-null array of <c>char</c>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "c")]
    public bool Append( char[] c )
    {
        if ( c == null || c.Length == 0 )
        {
            return true;
        }


        int count = c.Length;
        if ( this.cursor + count > this.end )
        {
            return this.OnOverflow();
        }

        fixed ( char* theirArray = c )
        {
            BufferHelper.CopyMemory( this.cursor, theirArray, count * sizeof(char));
        }

        this.cursor += count;

        return true;
    }

    /// <summary>
    /// Appends a <see cref="string"/> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="str">A non-null <see cref="string"/>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage( "Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s" )]
    public bool Append( string str )
    {
        if ( str == null )
        {
            return true;
        }

        return this.Append( str, 0, str.Length );
    }

    /// <summary>
    /// Appends a part of a <see cref="string"/> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="str">A non-null <see cref="string"/></param>
    /// <param name="startIndex">The index of the first character of the string to append.</param>
    /// <param name="length">The number of characters to append.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow")]
    public bool Append( string str, int startIndex, int length )
    {

        if ( str == null || length == 0 )
        {
            return true;
        }

        if ( this.cursor + length > this.end )
        {
            return this.OnOverflow();
        }

        fixed ( char* theirArray = str )
        {
            BufferHelper.CopyMemory( this.cursor, theirArray + startIndex, length * sizeof( char ) );
        }

        this.cursor += length;

        return true;
    }

    /// <summary>
    /// Appends a <see cref="CharSpan"/> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="span">A <see cref="CharSpan"/>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    public bool Append( in CharSpan span)
    {

        switch ( span.Array )
        {
            case string s:
                return this.Append( s, span.StartIndex, span.Length );

            case char[] a:
                return this.Append( a, span.StartIndex, span.Length );

            case null:
                return true;

            default:
                throw new AssertionFailedException();
        }
    }

    /// <summary>
    /// Appends an <see cref="UnsafeString"/> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="s">A non-null <see cref="UnsafeString"/>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "s")]
    public bool Append(UnsafeString s)
    {
        if ( s == null || s.Length == 0 )
        {
            return true;
        }

        if ( s.StringBuilder != null )
        {
            return this.Append(s.StringBuilder);
        }
        else
        {
            int count = s.Length;

            if (this.cursor + count > this.end)
            {
                return this.OnOverflow();
            }

            if ( s.CharArray != null )
            {
                fixed ( char* source = s.CharArray )
                {
                    BufferHelper.CopyMemory(this.cursor, source, count * sizeof(char));
                }
            }
            else
            {
                fixed (char* source = s.ToString())
                {
                    BufferHelper.CopyMemory(this.cursor, source, count * sizeof(char));
                }
            }

            this.cursor += count;
        }

        return true;
    }

    /// <summary>
    /// Appends the current value of a <see cref="UnsafeStringBuilder"/> to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="stringBuilder">A <see cref="UnsafeStringBuilder"/>.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    public bool Append(UnsafeStringBuilder stringBuilder)
    {
        if ( stringBuilder == null || stringBuilder.Length == 0 )
        {
            return true;
        }

        int count = stringBuilder.Length;
        if (this.cursor + count > this.end)
        {
            return this.OnOverflow();
        }

        BufferHelper.CopyMemory(this.cursor, stringBuilder.start, count * sizeof(char));

        this.cursor += count;

        return true;
    }

    /// <summary>
    /// Appends a <see cref="byte"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    public bool Append( byte value )
    {
        if ( this.cursor + 3 > this.end )
        {
            return this.OnOverflow();
        }

        this.AppendByte( value );
        return true;
    }

    /// <summary>
    /// Appends an <see cref="sbyte"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    public bool Append( sbyte value )
    {
        if ( this.cursor + 4 > this.end )
        {
            return this.OnOverflow();
        }

        unchecked
        {

            if ( value >= 0 )
            {
                this.AppendByte( (byte) value );
            }
            else
            {
                *this.cursor = '-';
                this.cursor++;
                this.AppendByte( (byte) -value );

            }
        }
        return true;
    }


    private void AppendByte( byte value )
    {
        unchecked
        {
            if ( value >= 100 )
            {
                *(this.cursor + 0) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 1) % 10) + '0');

                this.cursor += 3;
            }
            else if ( value >= 10 )
            {
                *(this.cursor + 0) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 1) % 10) + '0');

                this.cursor += 2;
            }
            else
            {
                *(this.cursor + 0) = (char) (((value / 1) % 10) + '0');

                this.cursor += 1;
            }
        }
    }

    /// <summary>
    /// Appends a <see cref="ushort"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>

    public bool Append( ushort value )
    {
        if ( this.cursor + 5 > this.end )
        {
            return this.OnOverflow();
        }

        this.AppendUInt16( value );
        return true;
    }

    /// <summary>
    /// Appends a <see cref="short"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>
    public bool Append( short value )
    {
        if ( this.cursor + 6 > this.end )
        {
            return this.OnOverflow();
        }
        unchecked
        {
            if ( value >= 0 )
            {
                this.AppendUInt16( (ushort) value );
            }
            else
            {
                *this.cursor = '-';
                this.cursor++;
                this.AppendUInt16( (ushort) -value );

            }
        }

        return true;
    }

    private void AppendUInt16( ushort value )
    {
        unchecked
        {
            if ( value >= 10000 )
            {
                *(this.cursor + 0) = (char) (((value / 10000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 4) = (char) (((value / 1) % 10) + '0');

                this.cursor += 5;
            }
            else if ( value >= 1000 )
            {
                *(this.cursor + 0) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 1) % 10) + '0');

                this.cursor += 4;
            }
            else if ( value >= 100 )
            {
                *(this.cursor + 0) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 1) % 10) + '0');

                this.cursor += 3;
            }
            else if ( value >= 10 )
            {
                *(this.cursor + 0) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 1) % 10) + '0');

                this.cursor += 2;
            }
            else
            {
                *(this.cursor + 0) = (char) (((value / 1) % 10) + '0');

                this.cursor += 1;
            }
        }
    }

    /// <summary>
    /// Appends a <see cref="uint"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>

    public bool Append( uint value )
    {
        if ( this.cursor + 10 > this.end )
        {
            return this.OnOverflow();
        }

        this.AppendUInt32( value );
        return true;
    }

    /// <summary>
    /// Appends a <see cref="int"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>

    public bool Append( int value )
    {
        if ( this.cursor + 11 > this.end )
        {
            return this.OnOverflow();
        }

        unchecked
        {
            if ( value >= 0 )
            {
                this.AppendUInt32( (uint) value );
            }
            else
            {
                *this.cursor = '-';
                this.cursor++;
                this.AppendUInt32( (uint) -value );

            }
        }

        return true;
    }


    private void AppendUInt32( uint value )
    {
        unchecked
        {
            if ( value >= 1000000000 )
            {
                *(this.cursor + 0) = (char) (((value / 1000000000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 100000000) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 10000000) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 1000000) % 10) + '0');
                *(this.cursor + 4) = (char) (((value / 100000) % 10) + '0');
                *(this.cursor + 5) = (char) (((value / 10000) % 10) + '0');
                *(this.cursor + 6) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 7) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 8) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 9) = (char) (((value / 1) % 10) + '0');

                this.cursor += 10;

            }
            else if ( value >= 100000000 )
            {
                *(this.cursor + 0) = (char) (((value / 100000000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 10000000) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 1000000) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 100000) % 10) + '0');
                *(this.cursor + 4) = (char) (((value / 10000) % 10) + '0');
                *(this.cursor + 5) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 6) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 7) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 8) = (char) (((value / 1) % 10) + '0');

                this.cursor += 9;
            }
            else if ( value >= 10000000 )
            {
                *(this.cursor + 0) = (char) (((value / 10000000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 1000000) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 100000) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 10000) % 10) + '0');
                *(this.cursor + 4) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 5) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 6) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 7) = (char) (((value / 1) % 10) + '0');

                this.cursor += 8;
            }
            else if ( value >= 1000000 )
            {
                *(this.cursor + 0) = (char) (((value / 1000000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 100000) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 10000) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 4) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 5) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 6) = (char) (((value / 1) % 10) + '0');

                this.cursor += 7;
            }
            else if ( value >= 100000 )
            {
                *(this.cursor + 0) = (char) (((value / 100000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 10000) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 4) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 5) = (char) (((value / 1) % 10) + '0');

                this.cursor += 6;
            }
            else if ( value >= 10000 )
            {
                *(this.cursor + 0) = (char) (((value / 10000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 4) = (char) (((value / 1) % 10) + '0');

                this.cursor += 5;
            }
            else if ( value >= 1000 )
            {
                *(this.cursor + 0) = (char) (((value / 1000) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 3) = (char) (((value / 1) % 10) + '0');

                this.cursor += 4;
            }
            else if ( value >= 100 )
            {
                *(this.cursor + 0) = (char) (((value / 100) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 2) = (char) (((value / 1) % 10) + '0');

                this.cursor += 3;

            }
            else if ( value >= 10 )
            {
                *(this.cursor + 0) = (char) (((value / 10) % 10) + '0');
                *(this.cursor + 1) = (char) (((value / 1) % 10) + '0');

                this.cursor += 2;

            }
            else
            {
                *(this.cursor + 0) = (char) (((value / 1) % 10) + '0');

                this.cursor += 1;

            }
        }
    }

    private void AppendPaddedUInt32( uint value )
    {
        if (((value / 1000000000) % 10) != 0 )
            throw new AssertionFailedException();

        unchecked
        {
            *(this.cursor + 0) = (char)(((value / 100000000) % 10) + '0');
            *(this.cursor + 1) = (char)(((value / 10000000) % 10) + '0');
            *(this.cursor + 2) = (char)(((value / 1000000) % 10) + '0');
            *(this.cursor + 3) = (char)(((value / 100000) % 10) + '0');
            *(this.cursor + 4) = (char)(((value / 10000) % 10) + '0');
            *(this.cursor + 5) = (char)(((value / 1000) % 10) + '0');
            *(this.cursor + 6) = (char)(((value / 100) % 10) + '0');
            *(this.cursor + 7) = (char)(((value / 10) % 10) + '0');
            *(this.cursor + 8) = (char)(((value / 1) % 10) + '0');

            this.cursor += 9;

        }
    }

    /// <summary>
    /// Appends a <see cref="ulong"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>

    public bool Append( ulong value )
    {
        if ( this.cursor + 20 > this.end )
        {
            return this.OnOverflow();
        }

        this.AppendUInt64( value );
        return true;
    }

    /// <summary>
    /// Appends a <see cref="long"/> (with decimal formatting) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>

    public bool Append( long value )
    {
        if ( this.cursor + 22 > this.end )
        {
            return this.OnOverflow();
        }

        unchecked
        {
            if ( value >= 0 )
            {
                this.AppendUInt64( (ulong) value );
            }
            else
            {
                *this.cursor = '-';
                this.cursor++;
                this.AppendUInt64( (ulong) -value );

            }
        }

        return true;
    }

    private void AppendUInt64( ulong value )
    {
        unchecked
        {
            const uint uintMaxValueDecimal = 1000000000; // 10^9

            // 64-bit arithmetic is significantly slower
            // so we're going to use 32-bit arithmetic as much as possible even for 64-bit numbers

            // value max: 18 446 744 073 709 551 616
            if ( value <= uint.MaxValue )
            {
                this.AppendUInt32( (uint) value );
                return;
            }

            // max: 18 446 744 073
            ulong valueMedHigh = value / uintMaxValueDecimal;

            uint valueLow = (uint) (value - uintMaxValueDecimal * valueMedHigh);

            if ( valueMedHigh <= uint.MaxValue )
            {
                this.AppendUInt32( (uint) valueMedHigh );
            }
            else
            {
                // max: 18
                byte valueHigh = (byte) (valueMedHigh / uintMaxValueDecimal);

                uint valueMed = (uint) (valueMedHigh - uintMaxValueDecimal * valueHigh);

                this.AppendByte( valueHigh );
                this.AppendPaddedUInt32( valueMed );
            }

            this.AppendPaddedUInt32( valueLow );
        }
    }

    /// <summary>
    /// Appends a <see cref="bool"/> (<c>true</c> or <c>false</c>, litterally) to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="value">The value to be appended.</param>
    /// <returns><c>true</c> in case of success, <c>false</c> in case of buffer overflow.</returns>

    public bool Append( bool value )
    {
        if ( value )
        {
            return this.Append( 't', 'r', 'u', 'e' );
        }
        else
        {
            return this.Append( 'f', 'a', 'l', 's', 'e' );
        }

    }

    /// <summary>
    /// Clears the current <see cref="UnsafeStringBuilder"/> so it can be reused to build a new string.
    /// </summary>
    public void Clear()
    {
        this.Version++;
        this.cursor = this.start;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        // Cache through an UnsafeString.
        return this.ToUnsafeString().ToString();
    }

    /// <summary>
    /// Returns the substring starting at a given index and ending at the end of the current string.
    /// </summary>
    /// <param name="startIndex">Index of the first character of the substring.</param>
    /// <returns>The substring starting from <paramref name="startIndex"/>.</returns>
    public string Substring( int startIndex )
    {
        return this.Substring(startIndex, this.Length - startIndex);
    }

    /// <summary>
    /// Returns the substring starting at a given index and having a specified length.
    /// </summary>
    /// <param name="startIndex">Index of the first character of the substring.</param>
    /// <param name="length">Number of characters to return.</param>
    /// <returns>The substring starting from <paramref name="startIndex"/> having <paramref name="length"/> characters.</returns>
    public string Substring(int startIndex, int length)
    {
        char[] a = this.CharArray;
        return new string(a, startIndex, length);
    }

    internal string ToStringImpl()
    {
        int len = (int)(this.cursor - this.start);
        return new string(this.start, 0, len);
    }

    /// <summary>
    /// Gets the version of the current <see cref="UnsafeStringBuilder"/>. This property is incremented every time
    /// the current object is reused, more specifically, when the <see cref="Clear"/> method is called.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Determines whether the current <see cref="UnsafeStringBuilder"/> has been disposed.
    /// </summary>
    public bool IsDisposed => this.cursor == null;


    /// <summary>
    /// Gets the current number of characters in the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    public int Length
    {
#if AGGRESSIVE_INLINING
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        get { return (int) (this.cursor - this.start); }
    }

    /// <summary>
    /// Truncates the string to a maximum length.
    /// </summary>
    /// <param name="length">The wished length of the string after truncation</param>
    public void Truncate( int length )
    {
        if ( this.Length > length )
        {
            this.cursor = this.start + length;
        }
    }

    /// <summary>
    /// Gets a pointer to the unmanaged buffer of the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    public IntPtr Buffer => (IntPtr) this.start;

    internal char[] CharArray => this.array;

    /// <summary>
    /// Gets an <see cref="UnsafeString"/> that provides read-only access to the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <returns>An <see cref="UnsafeString"/> that provides read-only access to the current <see cref="UnsafeStringBuilder"/>.</returns>
    public UnsafeString ToUnsafeString()
    {
        if ( !this.unsafeString.Recycle() )
        {
            this.unsafeString = new UnsafeString(this);
        }

        return this.unsafeString;
    }


    /// <summary>
    /// Appends a null character at the end of the current string, without affecting the string length.
    /// In case of overflow, if <see cref="ThrowOnOverflow"/> is <c>false</c>, the last character of the string is removed
    /// and the string length is decreased by 1.
    /// </summary>
    /// <returns><c>true</c> if the string was null-terminated without affecting its length, or <c>false</c> if the last character of the string was replaced by the null character
    /// and the string length was decreased by 1.</returns>
    public bool SetNullTermination()
    {
        if (this.cursor + 1 > this.end)
        {
            if ( this.OnOverflow() )
            {
                // If we're to overflow, we suppress the last character.
                this.cursor--;
                *this.cursor = '\0';
                
                return false;
            }

        }

        *this.cursor = '\0';
        return true;

    }

    /// <summary>
    /// Gets the <c>char</c> at a given position in the current <see cref="UnsafeStringBuilder"/>.
    /// </summary>
    /// <param name="index">Index of the <c>char</c> in the buffer.</param>
    /// <returns>The <c>char</c> at position <paramref name="index"/>.</returns>
    public char this[ int index ]
    {
        get { return *(this.start + index); }
        set { *(this.start + index) = value; }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if ( this.start != null )
        {
            this.gcHandle.Free();
            this.cursor = this.start = this.end = null;
            this.array = null;
            GC.SuppressFinalize( this );
        }
    }

    /// <inheritdoc />
    ~UnsafeStringBuilder()
    {
        this.gcHandle.Free();
    }

}
