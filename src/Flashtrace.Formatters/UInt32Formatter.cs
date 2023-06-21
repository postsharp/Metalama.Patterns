// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="uint"/> values.
/// </summary>
[PublicAPI]
public sealed class UInt32Formatter : Formatter<uint>
{
    public UInt32Formatter( IFormatterRepository repository ) : base( repository ) { }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, uint value )
    {
        stringBuilder.Append( value );
    }
}