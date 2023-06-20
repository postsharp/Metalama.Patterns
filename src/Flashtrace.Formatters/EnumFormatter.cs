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