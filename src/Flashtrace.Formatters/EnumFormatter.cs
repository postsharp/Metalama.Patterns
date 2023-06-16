// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Flashtrace.Formatters;

// See http://www.codeproject.com/Articles/278820/Optimized-Enum-ToString
// for a much more complicated enum formatter. But that one always boxes
// (because ToInt32 boxes), which would not be easy to work around.

/// <summary>
/// Efficient formatter for enums.
/// </summary>
public sealed class EnumFormatter<T> : Formatter<T>
{
    /// <summary>
    /// The singleton instance of <see cref="EnumFormatter{T}"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104")]
    [SuppressMessage("Microsoft.Design", "CA1000")]
    public static readonly EnumFormatter<T> Instance = new EnumFormatter<T>();

    // To make this formatter efficient (i.e. to avoid allocations) and thread-safe,
    // names of named values of the enum are stored in simpleNames, which is never mutated.
    // Other names (bitwise ORs for [Flags], unnamed values) are cached per thread in otherNames.

    private static readonly Dictionary<T, string> simpleNames;

    [ThreadStatic]
    private static Dictionary<T, string> otherNames;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static EnumFormatter()
    {
        T[] values = (T[]) Enum.GetValues( typeof(T) );

        // Distinct is required, because GetValues() returns duplicates for values with multiple names
        simpleNames = values.Distinct().ToDictionary( v => v, v => v.ToString() );
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, T value )
    {
        stringBuilder.Append( GetString( value ) );
    }

    /// <summary>
    /// Returns the string value of the given enum value.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1000")]
    public static string GetString( T value )
    {
        string name;

        if ( simpleNames.TryGetValue( value, out name ) )
        {
            return name;
        }

        if ( otherNames == null )
        {
            otherNames = new Dictionary<T, string>();
        }

        if ( otherNames.TryGetValue( value, out name ) )
        {
            return name;
        }

        return otherNames[value] = value.ToString();
    }
}

/// <summary>
/// Efficient formatter for enums.
/// </summary>
public static class EnumFormatter
{
    /// <summary>
    /// Returns the string value of the given enum value.
    /// </summary>
    public static string GetString<T>( T value )
    {
        return EnumFormatter<T>.GetString( value );
    }
}