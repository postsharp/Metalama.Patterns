// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters;

/// <summary>
/// Efficient formatter for <see cref="sbyte"/>.
/// </summary>
public sealed class SByteFormatter : Formatter<sbyte>
{
    /// <summary>
    /// The singleton instance of <see cref="SByteFormatter"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104")]
    public static readonly SByteFormatter Instance = new SByteFormatter();

    private SByteFormatter()
    {
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, sbyte value )
    {
        stringBuilder.Append( value );
    }
}