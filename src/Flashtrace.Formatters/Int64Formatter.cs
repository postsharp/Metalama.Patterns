// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// Efficient formatter for <see cref="long"/>.
/// </summary>
[PublicAPI]
public sealed class Int64Formatter : Formatter<long>
{
    public Int64Formatter( IFormatterRepository repository ) : base( repository ) { }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, long value )
    {
        stringBuilder.Append( value );
    }
}