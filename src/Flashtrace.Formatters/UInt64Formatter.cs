// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="ulong"/> values.
/// </summary>
public sealed class UInt64Formatter : Formatter<ulong>
{
    /// <summary>
    /// The singleton instance of <see cref="UInt64Formatter"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Security", "CA2104")]
    public static readonly UInt64Formatter Instance = new UInt64Formatter();


    private UInt64Formatter()
    {
    }

    /// <inheritdoc />
    public override void Write(UnsafeStringBuilder stringBuilder, ulong value)
    {
         stringBuilder.Append( value );
    }
    
}