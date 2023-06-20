// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

// See http://www.codeproject.com/Articles/278820/Optimized-Enum-ToString
// for a much more complicated enum formatter. But that one always boxes
// (because ToInt32 boxes), which would not be easy to work around.

public sealed class EnumFormatter<T> : Formatter<T>
    where T : Enum
{
    public EnumFormatter( IFormatterRepository repository ) : base( repository ) { }

    public override void Write( UnsafeStringBuilder stringBuilder, T? value )
    {
        EnumFormatterCache<T>.Write( stringBuilder, value );
    }
}

/// <summary>
/// Efficient formatter for enums.
/// </summary>
internal static class EnumFormatterCache<T>
    where T : Enum
{
    // To make this formatter efficient (i.e. to avoid allocations) and thread-safe,
    // names of named values of the enum are stored in simpleNames, which is never mutated.
    // Other names (bitwise ORs for [Flags], unnamed values) are cached per thread in otherNames.

    private static readonly Dictionary<T, string> _simpleNames;

    [ThreadStatic]
    private static Dictionary<T, string>? _otherNames;

    static EnumFormatterCache()
    {
        var values = (T[]) Enum.GetValues( typeof(T) );

        // Distinct is required, because GetValues() returns duplicates for values with multiple names
        _simpleNames = values.Distinct().ToDictionary( v => v, v => v.ToString() );
    }

    public static void Write( UnsafeStringBuilder stringBuilder, T value )
    {
        stringBuilder.Append( GetString( value ) );
    }

    /// <summary>
    /// Returns the string value of the given enum value.
    /// </summary>
    public static string GetString( T value )
    {
        if ( value == null )
        {
            throw new ArgumentNullException( nameof(value) );
        }

        string name;

        if ( _simpleNames.TryGetValue( value, out name ) )
        {
            return name;
        }

        _otherNames ??= new Dictionary<T, string>();

        if ( _otherNames.TryGetValue( value, out name ) )
        {
            return name;
        }

        return _otherNames[value] = value.ToString();
    }
}

// TODO: Remove class.

/// <summary>
/// Efficient formatter for enums.
/// </summary>
[Obsolete( "Do we need this?", true )]
public static class EnumFormatter
{
    /// <summary>
    /// Returns the string value of the given enum value.
    /// </summary>
    public static string GetString<T>( T value )
        where T : Enum
    {
        return EnumFormatterCache<T>.GetString( value );
    }
}