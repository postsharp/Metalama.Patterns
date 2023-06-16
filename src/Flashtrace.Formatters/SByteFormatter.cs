// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

/// <summary>
/// Efficient formatter for <see cref="sbyte"/>.
/// </summary>
public sealed class SByteFormatter : Formatter<sbyte>
{
    public SByteFormatter( IFormatterRepository repository ) : base( repository )
    {
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, sbyte value )
    {
        stringBuilder.Append( value );
    }
}