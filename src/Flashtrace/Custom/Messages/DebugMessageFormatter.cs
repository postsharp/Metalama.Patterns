// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using System.Text;

namespace Flashtrace.Custom.Messages
{
    internal static class DebugMessageFormatter
    {
        public static string Format<T>( this T message ) where T : IMessage
        {
            using var writer = new Writer();
            message.Write( writer, CustomLogRecordItem.Message );

            return writer.ToString();
        }

        private class Writer : ICustomLogRecordBuilder
        {
            private StringBuilder stringBuilder = new();

            public void BeginWriteItem( CustomLogRecordItem item, in CustomLogRecordTextOptions options ) { }

            public void Complete() { }

            public void Dispose() { }

            public void EndWriteItem( CustomLogRecordItem item ) { }

            public void SetException( Exception exception )
            {
                this.stringBuilder.Append( ", " + exception.GetType().Name + " : " + exception.Message );
            }

            public void SetExecutionTime( double executionTime, bool isOvertime ) { }

            public void WriteCustomParameter<T>( int index, in CharSpan parameterName, T value, in CustomLogParameterOptions options )
            {
                this.stringBuilder.Append( value.ToString() );
            }

            public void WriteCustomString( in CharSpan str )
            {
                this.stringBuilder.Append( str.ToString() );
            }

            public override string ToString() => this.stringBuilder.ToString();
        }
    }
}