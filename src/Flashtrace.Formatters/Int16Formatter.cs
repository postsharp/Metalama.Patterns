// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

/// <summary>
/// Efficient formatter for <see cref="short"/>.
/// </summary>
public sealed class Int16Formatter : Formatter<short>
{
    public Int16Formatter( IFormatterRepository repository ) : base( repository )
    {
    }

    /// <inheritdoc />
    public override void Write(UnsafeStringBuilder stringBuilder, short value)
    {
        stringBuilder.Append( value );
    }
}