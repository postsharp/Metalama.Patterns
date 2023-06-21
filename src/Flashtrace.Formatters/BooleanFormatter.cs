// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace.Formatters;

/// <summary>
/// A formatter for <see cref="bool"/> values.
/// </summary>
[PublicAPI]
public sealed class BooleanFormatter : Formatter<bool>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanFormatter"/> class.
    /// </summary>
    /// <param name="repository"></param>
    public BooleanFormatter( IFormatterRepository repository ) : base( repository ) { }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, bool value )
    {
        stringBuilder.Append( value );
    }
}