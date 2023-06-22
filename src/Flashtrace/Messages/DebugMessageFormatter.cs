// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using System.Text;

namespace Flashtrace.Messages;

internal static class DebugMessageFormatter
{
    public static string Format<T>( T message ) 
        where T : IMessage
    {
        using var writer = new Writer();
        message.Write( writer, CustomLogRecordItem.Message );

        return writer.ToString();
    }

    private sealed class Writer : ICustomLogRecordBuilder
    {
        private readonly StringBuilder _stringBuilder = new();

        public void BeginWriteItem( CustomLogRecordItem item, in CustomLogRecordTextOptions options ) { }

        public void Complete() { }

        public void Dispose() { }

        public void EndWriteItem( CustomLogRecordItem item ) { }

        public void SetException( Exception exception )
        {
            this._stringBuilder.Append( ", " + exception.GetType().Name + " : " + exception.Message );
        }

        public void SetExecutionTime( double executionTime, bool isOvertime ) { }

        public void WriteCustomParameter<T>( int index, in CharSpan parameterName, T? value, in CustomLogParameterOptions options )
        {
            this._stringBuilder.Append( value?.ToString() );
        }

        public void WriteCustomString( in CharSpan str )
        {
            this._stringBuilder.Append( str.ToString() );
        }

        public override string ToString() => this._stringBuilder.ToString();
    }
}