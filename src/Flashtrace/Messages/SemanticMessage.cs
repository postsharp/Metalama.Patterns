// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Flashtrace.Messages;

/// <summary>
/// Encapsulates a semantic message without parameter. Use the <see cref="SemanticMessageBuilder"/> class to create an instance of this type.
/// </summary>
[SuppressMessage( "Microsoft.Performance", "CA1815", Justification = "Equal is not a use case" )]
public readonly struct SemanticMessage : IMessage
{
    private readonly string _messageName;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] // To avoid copying the struct.
    internal SemanticMessage( string messageName )
    {
        this._messageName = messageName;
    }

    void IMessage.Write( ICustomLogRecordBuilder builder, CustomLogRecordItem item )
    {
        builder.BeginWriteItem( item, new CustomLogRecordTextOptions( 0, this._messageName ) );
        builder.EndWriteItem( item );
    }

    /// <inheritdoc/>
    public override string ToString() => DebugMessageFormatter.Format( this );
}