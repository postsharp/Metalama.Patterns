// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Formatters;
using JetBrains.Annotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Flashtrace;

// Disable all warnings until to do below is decided.
// ReSharper disable all
#pragma warning disable SA1648
#pragma warning disable SA1404
#pragma warning disable SA1214
#pragma warning disable CA1822
#pragma warning disable CS8603
#pragma warning disable CS8604
#pragma warning disable CS8769
#pragma warning disable CS8625
#pragma warning disable CS8618
#pragma warning disable IDE0004

// TODO: !!! Is LegacySourceLogger still relevant/required? (see note)
/* LegacySourceLogger appears to intend both implicit and explicit interface implementation for some
 * members - for example, Role has inheritdoc which implies implicit of ILogger.Role, but also
 * has an explicit impl of ILogger.Role. Maybe there's a good reason for this. But I'm not going to
 * try and work it out until we decide if we're keeping this class or not.
 */

/// <summary>
/// A base class for implementations of <see cref="ILogger"/> that cannot depend on the <c>PostSharp.Patterns.Diagnostics</c> package.
/// </summary>
[PublicAPI]
public abstract partial class LegacySourceLogger : ILogger, IContextLocalLogger, ILogActivityOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LegacySourceLogger"/> class.
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

    (IContextLocalLogger Logger, bool IsEnabled) ILogger.GetContextLocalLogger( LogLevel level )
    {
        return (this, this.IsEnabled( level ));
    }

    ILoggingContext ILogger.CurrentContext => throw new NotImplementedException();

    bool ILogger.IsEnabled( LogLevel level ) => this.IsEnabled( level );

    private class RecordBuilder : ICustomLogRecordBuilder
    {
        private LegacySourceLogger _logger;
        private readonly StringBuilder _stringBuilder = new();
        private readonly LogLevel _level;
        private readonly LogRecordKind _recordKind;
        private Exception _exception;
        private readonly Context _context;
        private bool _appendComma;

        public RecordBuilder( LegacySourceLogger logger, LogLevel level, LogRecordKind recordKind, Context context )
        {
            this._logger = logger;
            this._level = level;
            this._recordKind = recordKind;
            this._context = context;
        }

        void ICustomLogRecordBuilder.BeginWriteItem( CustomLogRecordItem item, in CustomLogRecordTextOptions options )
        {
            switch ( item )
            {
                case CustomLogRecordItem.Message:
                case CustomLogRecordItem.ActivityDescription:
                    if ( options.Name != null )
                    {
                        this._stringBuilder.Append( options.Name );
                    }

                    break;

                case CustomLogRecordItem.ActivityOutcome:
                    if ( this._stringBuilder.Length > 0 )
                    {
                        this._stringBuilder.Append( ": " );
                    }
                    else if ( this._context != null )
                    {
                        this._stringBuilder.Append( this._context.Description );
                        this._stringBuilder.Append( ": " );
                    }

                    var text = options.Name ?? GetRecordKindText( this._recordKind );

                    if ( text != null )
                    {
                        this._stringBuilder.Append( text );
                        this._appendComma = true;
                    }

                    break;
            }
        }

        void ICustomLogRecordBuilder.EndWriteItem( CustomLogRecordItem item ) { }

        public void Dispose()
        {
            this._logger = null;
        }

        void ICustomLogRecordBuilder.SetException( Exception e )
        {
            this._exception = e;
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
                    this._stringBuilder.Append( ", " );
                    this._stringBuilder.Append( parameterName );
                    this._stringBuilder.Append( " = " );
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
                this._stringBuilder.Append( "null" );
            }
            else
            {
                this._stringBuilder.Append( value.ToString() );
            }
        }

        void ICustomLogRecordBuilder.WriteCustomString( in CharSpan str )
        {
            if ( this._appendComma )
            {
                this._appendComma = false;
                this._stringBuilder.Append( ", " );
            }

            this._stringBuilder.Append( str.ToString() );
        }

        void ICustomLogRecordBuilder.Complete()
        {
            if ( this._logger != null )
            {
                if ( this._recordKind == LogRecordKind.CustomActivityEntry )
                {
                    this._context.Description = this._stringBuilder.ToString();
                    this._stringBuilder.Append( ": " );
                    this._stringBuilder.Append( GetRecordKindText( LogRecordKind.CustomActivityEntry ) );
                }

                this._logger.Write( this._level, this._recordKind, this._stringBuilder.ToString(), this._exception );
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