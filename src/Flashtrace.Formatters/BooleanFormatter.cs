// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="bool"/> values.
/// </summary>
public sealed class BooleanFormatter : Formatter<bool>
{
    /// <summary>
    /// The singleton instance of <see cref="BooleanFormatter"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104")]
    public static readonly BooleanFormatter Instance = new BooleanFormatter();

    private BooleanFormatter()
    {
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, bool value )
    {
        stringBuilder.Append( value );
    }
}