// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// Efficient formatter for <see cref="int"/>.
/// </summary>
[PublicAPI]
public sealed class Int32Formatter : Formatter<int>
{
    public Int32Formatter( IFormatterRepository repository ) : base( repository ) { }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, int value )
    {
        stringBuilder.Append( value );
    }
}