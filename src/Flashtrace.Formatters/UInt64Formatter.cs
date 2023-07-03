// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="ulong"/> values.
/// </summary>
[PublicAPI]
public sealed class UInt64Formatter : Formatter<ulong>
{
    public UInt64Formatter( IFormatterRepository repository ) : base( repository ) { }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, ulong value )
    {
        stringBuilder.Append( value );
    }
}