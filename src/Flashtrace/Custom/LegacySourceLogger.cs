// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Formatters;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Flashtrace.Custom
{
    /// <summary>
    /// A base class for implementations of <see cref="ILogger"/> that cannot depend on the <c>PostSharp.Patterns.Diagnostics</c> package.
    /// </summary>
    public abstract partial class LegacySourceLogger : ILogger, IContextLocalLogger, ILogActivityOptions
    {
        /// <summary>
        /// Initializes a new <see cref="LegacySourceLogger"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="type">The source <see cref="Type"/>.</param>
        protected LegacySourceLogger( string role, Type type )
        {
            this.Type = type;
            this.Role = role;
        }

        /// <inheritdoc/>
        public abstract bool IsEnabled( LogLevel level );

        /// <inheritdoc/>
        public abstract ILoggerFactory Factory { get; }

        /// <inheritdoc/>
        [SuppressMessage( "Microsoft.Naming", "CA1721" )]
        public Type Type { get; }

        /// <inheritdoc/>
        public string Role { get; }

        /// <inheritdoc/>
        [SuppressMessage( "Microsoft.Performance", "CA1822" )]
        public bool RequiresSuspendResume => false;

        private static string GetRecordKindText( LogRecordKind recordKind )
        {
            switch ( recordKind )
            {
                case LogRecordKind.CustomActivityEntry:
                    return "Starting";

                case LogRecordKind.CustomActivityFailure:
                case LogRecordKind.CustomActivityException:
                    return "Failed";

                case LogRecordKind.CustomActivitySuccess:
                    return "Succeeded";

                default:
                    return null;
            }
        }

        /// <summary>
        /// Writes a text message.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="recordKind"></param>
        /// <param name="text">The fully-rendered message.</param>
        /// <param name="exception">An optional <see cref="Exception"/>.</param>
        protected abstract void Write( LogLevel level, LogRecordKind recordKind, string text, Exception exception );

        private void WriteFormatted( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, object[] args, Exception exception )
        {
            var stringBuilder = new StringBuilder();

            var recordKindText = GetRecordKindText( recordKind );

            if ( recordKind == LogRecordKind.CustomActivityEntry )
            {
                WriteText( text, args, stringBuilder );
                ((Context) context).Description = stringBuilder.ToString();
                stringBuilder.Append( ": " );
                stringBuilder.Append( recordKindText );
            }
            else
            {
                if ( context != null )
                {
                    stringBuilder.Append( ((Context) context).Description );
                    stringBuilder.Append( ": " );

                    if ( recordKindText != null )
                    {
                        stringBuilder.Append( recordKindText );
                    }
                }

                if ( !string.IsNullOrEmpty( text ) )
                {
                    if ( stringBuilder.Length > 0 )
                    {
                        stringBuilder.Append( ", " );
                    }

                    WriteText( text, args, stringBuilder );
                }
            }

            if ( exception != null )
            {
                stringBuilder.AppendLine();
                stringBuilder.Append( exception.ToString() );
            }

            this.Write( level, recordKind, stringBuilder.ToString(), exception );
        }

        private static void WriteText( string text, object[] args, StringBuilder stringBuilder )
        {
            if ( args == null || args.Length == 0 )
            {
                stringBuilder.Append( text );
            }
            else
            {
                var parser = new FormattingStringParser( text );

                var parameterIndex = 0;

                while ( true )
                {
                    var str = parser.GetNextSubstring();

                    if ( str.Array == null )
                    {
                        break;
                    }

                    stringBuilder.Append( str.Array, str.Offset, str.Count );

                    str = parser.GetNextParameter();

                    if ( str.Array == null )
                    {
                        break;
                    }

                    stringBuilder.Append( args[parameterIndex] );
                    parameterIndex++;
                }
            }
        }

        void ILogger.Write(
            ILoggingContext context,
            LogLevel level,
            LogRecordKind recordKind,
            string text,
            object[] args,
            Exception exception,
            ref CallerInfo recordInfo )
        {
            if ( this.IsEnabled( level ) )
            {
                this.WriteFormatted( context, level, recordKind, text, args, exception );
            }
        }

        void ILogger.Write( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, Exception exception, ref CallerInfo recordInfo )
        {
            if ( this.IsEnabled( level ) )
            {
                this.WriteFormatted( context, level, recordKind, text, null, exception );
            }
        }

        ILoggingContext ILogger.OpenActivity( LogActivityOptions options, ref CallerInfo callerInfo )
        {
            return new Context( options.IsAsync );
        }

        ILoggingContext IContextLocalLogger.OpenActivity( in OpenActivityOptions options, ref CallerInfo callerInfo ) => new Context( callerInfo.IsAsync );

        void ILogger.ResumeActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

        void ILogger.SuspendActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

        void ILogger.SetWaitDependency( ILoggingContext context, object waited ) { }

        bool ILogger.RequiresSuspendResume => false;

        void IContextLocalLogger.ResumeActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

        void IContextLocalLogger.SuspendActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

        void IContextLocalLogger.SetWaitDependency( ILoggingContext context, object waited ) { }

        ILogActivityOptions ILogger.ActivityOptions => this;

        LogLevel ILogActivityOptions.ActivityLevel => LogLevel.Trace;

        LogLevel ILogActivityOptions.FailureLevel => LogLevel.Warning;

        LogLevel ILogActivityOptions.ExceptionLevel => LogLevel.Warning;

        string ILogger.Role => null;

        [SuppressMessage( "Microsoft.Design", "CA1031" )]
        void ILoggerExceptionHandler.OnInvalidUserCode( ref CallerInfo callerInfo, string format, params object[] args )
        {
            try
            {
                var message = "Error in user code calling the logging subsystem: " + string.Format( CultureInfo.InvariantCulture, format, args );

                message += Environment.NewLine + new StackTrace().ToString();

                this.WriteFormatted( null, LogLevel.Warning, LogRecordKind.CustomRecord, message, null, null );
            }
            catch { }
        }

        [SuppressMessage( "Microsoft.Design", "CA1031" )]
        void ILoggerExceptionHandler.OnInternalException( Exception exception )
        {
            try
            {
                var message = "Error in user code calling the logging subsystem: " + exception.ToString();
                this.WriteFormatted( null, LogLevel.Warning, LogRecordKind.CustomRecord, message, null, null );
            }
            catch { }
        }

        ICustomLogRecordBuilder IContextLocalLogger.GetRecordBuilder( in CustomLogRecordOptions recordInfo, ref CallerInfo callerInfo, ILoggingContext context )
        {
            return new RecordBuilder( this, recordInfo.Level, recordInfo.Kind, (Context) context );
        }

        ILoggerFactory ILogger.Factory => (ILoggerFactory) this.Factory;

        IContextLocalLogger ILogger.GetContextLocalLogger() => this;

        (IContextLocalLogger logger, bool isEnabled) ILogger.GetContextLocalLogger( LogLevel level )
        {
            return (this, this.IsEnabled( level ));
        }

        ILoggingContext ILogger.CurrentContext => throw new NotImplementedException();

        bool ILogger.IsEnabled( LogLevel level ) => this.IsEnabled( level );

        private class RecordBuilder : ICustomLogRecordBuilder
        {
            private LegacySourceLogger logger;
            private readonly StringBuilder stringBuilder = new();
            private readonly LogLevel level;
            private readonly LogRecordKind recordKind;
            private Exception exception;
            private readonly Context context;
            private bool appendComma;

            public RecordBuilder( LegacySourceLogger logger, LogLevel level, LogRecordKind recordKind, Context context )
            {
                this.logger = logger;
                this.level = level;
                this.recordKind = recordKind;
                this.context = context;
            }

            void ICustomLogRecordBuilder.BeginWriteItem( CustomLogRecordItem item, in CustomLogRecordTextOptions options )
            {
                switch ( item )
                {
                    case CustomLogRecordItem.Message:
                    case CustomLogRecordItem.ActivityDescription:
                        if ( options.Name != null )
                        {
                            this.stringBuilder.Append( options.Name );
                        }

                        break;

                    case CustomLogRecordItem.ActivityOutcome:
                        if ( this.stringBuilder.Length > 0 )
                        {
                            this.stringBuilder.Append( ": " );
                        }
                        else if ( this.context != null )
                        {
                            this.stringBuilder.Append( this.context.Description );
                            this.stringBuilder.Append( ": " );
                        }

                        var text = options.Name ?? GetRecordKindText( this.recordKind );

                        if ( text != null )
                        {
                            this.stringBuilder.Append( text );
                            this.appendComma = true;
                        }

                        break;
                }
            }

            void ICustomLogRecordBuilder.EndWriteItem( CustomLogRecordItem item ) { }

            public void Dispose()
            {
                this.logger = null;
            }

            void ICustomLogRecordBuilder.SetException( Exception e )
            {
                this.exception = e;
                this.WriteParameter( "exception", e, CustomLogParameterOptions.SemanticParameter );
            }

            void ICustomLogRecordBuilder.SetExecutionTime( double executionTime, bool isOvertime ) { }

            void ICustomLogRecordBuilder.WriteCustomParameter<T>( int index, in CharSpan parameterName, T value, in CustomLogParameterOptions options )
            {
                this.WriteParameter( parameterName.ToString(), value, options );
            }

            private void WriteParameter<T>( string parameterName, T value, in CustomLogParameterOptions options )
            {
                switch ( options.Mode )
                {
                    default:
                        this.WriteValue( value );

                        break;

                    case CustomLogParameterMode.NameValuePair:
                        this.stringBuilder.Append( ", " );
                        this.stringBuilder.Append( parameterName );
                        this.stringBuilder.Append( " = " );
                        this.WriteValue( value );

                        break;

                    case CustomLogParameterMode.Hidden:
                        break;
                }
            }

            private void WriteValue<T>( T value )
            {
                if ( value == null )
                {
                    this.stringBuilder.Append( "null" );
                }
                else
                {
                    this.stringBuilder.Append( value.ToString() );
                }
            }

            void ICustomLogRecordBuilder.WriteCustomString( in CharSpan str )
            {
                if ( this.appendComma )
                {
                    this.appendComma = false;
                    this.stringBuilder.Append( ", " );
                }

                this.stringBuilder.Append( str.ToString() );
            }

            void ICustomLogRecordBuilder.Complete()
            {
                if ( this.logger != null )
                {
                    if ( this.recordKind == LogRecordKind.CustomActivityEntry )
                    {
                        this.context.Description = this.stringBuilder.ToString();
                        this.stringBuilder.Append( ": " );
                        this.stringBuilder.Append( GetRecordKindText( LogRecordKind.CustomActivityEntry ) );
                    }

                    this.logger.Write( this.level, this.recordKind, this.stringBuilder.ToString(), this.exception );
                }

                this.Dispose();
            }

            void IDisposable.Dispose() { }
        }

        private class Context : ILoggingContext
        {
            public Context( bool isAsync )
            {
                this.IsAsync = isAsync;
            }

            public string Description
            {
                get;
                set;
            }

            public bool IsAsync { get; }

            public string SyntheticId => null;

            public bool IsDisposed { get; private set; }

            int ILoggingContext.RecycleId => 0;

            void IDisposable.Dispose()
            {
                this.IsDisposed = true;
            }

            [SuppressMessage( "Microsoft.Performance", "CA1822" )]
            public bool IsHidden => false;
        }
    }
}