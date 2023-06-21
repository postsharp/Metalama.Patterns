// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="ushort"/> values.
/// </summary>
[PublicAPI]
public sealed class UInt16Formatter : Formatter<ushort>
{
    public UInt16Formatter( IFormatterRepository repository ) : base( repository ) { }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, ushort value )
    {
        stringBuilder.Append( value );
    }
}