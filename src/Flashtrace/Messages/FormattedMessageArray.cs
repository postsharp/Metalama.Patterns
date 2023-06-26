// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Records;
using JetBrains.Annotations;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Flashtrace.Messages;

/// <summary>
/// Encapsulates a formatted message with an arbitrary number of parameters. Use the <see cref="FormattedMessageBuilder"/> class to create an instance of this type.
/// </summary>
[PublicAPI]
public readonly struct FormattedMessageArray : IMessage
{
    private readonly string _formattingString;
    private readonly object?[] _args;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] // To avoid copying the struct.
    internal FormattedMessageArray( string formattingString, object?[] args )
    {
        this._formattingString = formattingString;
        this._args = args;
    }

    void IMessage.Write( ILogRecordBuilder recordBuilder, LogRecordItem item )
    {
        recordBuilder.BeginWriteItem( item, new LogRecordTextOptions( this._args.Length ) );

        var parser = new FormattingStringParser( this._formattingString );

        for ( var i = 0; i < this._args.Length; i++ )
        {
            recordBuilder.WriteString( parser.GetNextSubstring() );
            var parameter = parser.GetNextParameter();

            if ( parameter.Array == null )
            {
                throw new InvalidFormattingStringException(
                    string.Format( CultureInfo.InvariantCulture, "The formatting string must have exactly {0} parameters.", this._args.Length ) );
            }

            recordBuilder.WriteParameter( i, parameter, this._args[i], LogParameterOptions.FormattedStringParameter );
        }

        recordBuilder.WriteString( parser.GetNextSubstring() );

        if ( parser.GetNextParameter().Array != null )
        {
            throw new InvalidFormattingStringException(
                string.Format( CultureInfo.InvariantCulture, "The formatting string must have exactly {0} parameters.", this._args.Length ) );
        }

        recordBuilder.EndWriteItem( item );
    }

    /// <inheritdoc/>
    public override string ToString() => DebugMessageFormatter.Format( this );
}