// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Messages;
using Flashtrace.Options;
using Flashtrace.Records;
using Flashtrace.Utilities;
using JetBrains.Annotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace Flashtrace.Loggers;

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

// TODO: For now, making this internal pending encountering a necessary use case.
/* LegacySourceLogger appears to intend both implicit and explicit interface implementation for some
 * members - for example, Role has inheritdoc which implies implicit of ILogger.Role, but also
 * has an explicit impl of ILogger.Role. Maybe there's a good reason for this. But I'm not going to
 * try and work it out until we decide if we're keeping this class or not.
 */

/// <summary>
/// A base class for simple and low-performance implementations of <see cref="IFlashtraceLogger"/>.
/// </summary>
/// <remarks>
/// <para>The simplification stems from the wrapping of all message arguments in an object array, which
/// allocates memory.</para>
/// </remarks>
[PublicAPI]
public abstract partial class SimpleFlashtraceLogger : IFlashtraceLogger, IFlashtraceLocalLogger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleFlashtraceLogger"/> class.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <param name="name">The source name.</param>
    protected SimpleFlashtraceLogger( FlashtraceRole role, string name )
    {
        this.Name = name;
        this.Role = role;
    }

    /// <inheritdoc/>
    public abstract bool IsEnabled( FlashtraceLevel level );

    /// <inheritdoc/>
    public abstract IFlashtraceRoleLoggerFactory Factory { get; }

    public string Name { get; }

    /// <inheritdoc/>
    public FlashtraceRole Role { get; }

    /// <inheritdoc/>
    public bool RequiresSuspendResume => false;

    private static string GetRecordKindText( LogRecordKind recordKind )
    {
        // TODO: [FT-Review] If LegacySourceLogger is retained (see TODO at top of this file), decide how to rework this properly.
#if false // Orginal code:
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
#else // Quick fix when removing LogRecordKind.CustomActivityFailure et al, retains effective behaviour of the original so tests still pass:
        return recordKind switch
        {
            LogRecordKind.ActivityEntry => "Starting",
            _ => null
        };
#endif
    }

    /// <summary>
    /// Writes a text message.
    /// </summary>
    /// <param name="level"></param>
    /// <param name="recordKind"></param>
    /// <param name="text">The fully-rendered message.</param>
    /// <param name="exception">An optional <see cref="Exception"/>.</param>
    protected abstract void Write( FlashtraceLevel level, LogRecordKind recordKind, string text, Exception exception );

    private void WriteFormatted( ILoggingContext context, FlashtraceLevel level, LogRecordKind recordKind, string text, object[] args, Exception exception )
    {
        var stringBuilder = new StringBuilder();

        var recordKindText = GetRecordKindText( recordKind );

        if ( recordKind == LogRecordKind.ActivityEntry )
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
            var parser = new FormattingStringParser( text.AsSpan() );

            var parameterIndex = 0;

            while ( true )
            {
                var str = parser.GetNextText();

                stringBuilder.PortableAppend( str );

                if ( !parser.TryGetNextParameter( out _ ) )
                {
                    break;
                }

                stringBuilder.Append( args[parameterIndex] );
                parameterIndex++;
            }
        }
    }

    void IFlashtraceLogger.Write(
        ILoggingContext context,
        FlashtraceLevel level,
        LogRecordKind recordKind,
        string text,
        object[] args,
        Exception exception,
        in CallerInfo recordInfo )
    {
        if ( this.IsEnabled( level ) )
        {
            this.WriteFormatted( context, level, recordKind, text, args, exception );
        }
    }

    void IFlashtraceLogger.Write(
        ILoggingContext context,
        FlashtraceLevel level,
        LogRecordKind recordKind,
        string text,
        Exception exception,
        in CallerInfo recordInfo )
    {
        if ( this.IsEnabled( level ) )
        {
            this.WriteFormatted( context, level, recordKind, text, null, exception );
        }
    }

    ILoggingContext IFlashtraceLogger.OpenActivity( in LogActivityOptions options, in CallerInfo callerInfo )
    {
        return new Context( options.IsAsync );
    }

    ILoggingContext IFlashtraceLocalLogger.OpenActivity( in OpenActivityOptions options, in CallerInfo callerInfo, bool isAsync ) => new Context( isAsync );

    void IFlashtraceLogger.ResumeActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void IFlashtraceLogger.SuspendActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void IFlashtraceLogger.SetWaitDependency( ILoggingContext context, object waited ) { }

    bool IFlashtraceLogger.RequiresSuspendResume => false;

    void IFlashtraceLocalLogger.ResumeActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void IFlashtraceLocalLogger.SuspendActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void IFlashtraceLocalLogger.SetWaitDependency( ILoggingContext context, object waited ) { }

    string IFlashtraceLogger.Role => null;

    void IFlashtraceExceptionHandler.OnInvalidUserCode( in CallerInfo callerInfo, string format, params object[] args )
    {
        try
        {
            var message = "Error in user code calling the logging subsystem: " + string.Format( CultureInfo.InvariantCulture, format, args );

            message += Environment.NewLine + new StackTrace().ToString();

            this.WriteFormatted( null, FlashtraceLevel.Warning, LogRecordKind.Message, message, null, null );
        }
        catch { }
    }

    void IFlashtraceExceptionHandler.OnInternalException( Exception exception )
    {
        try
        {
            var message = "Error in user code calling the logging subsystem: " + exception.ToString();
            this.WriteFormatted( null, FlashtraceLevel.Warning, LogRecordKind.Message, message, null, null );
        }
        catch { }
    }

    ILogRecordBuilder IFlashtraceLocalLogger.GetRecordBuilder( in LogRecordOptions recordInfo, in CallerInfo callerInfo, ILoggingContext context )
    {
        return new RecordBuilder( this, recordInfo.Level, recordInfo.Kind, (Context) context );
    }

    IFlashtraceRoleLoggerFactory IFlashtraceLogger.Factory => (IFlashtraceRoleLoggerFactory) this.Factory;

    IFlashtraceLocalLogger IFlashtraceLogger.GetContextLocalLogger() => this;

    (IFlashtraceLocalLogger Logger, bool IsEnabled) IFlashtraceLogger.GetContextLocalLogger( FlashtraceLevel level )
    {
        return (this, this.IsEnabled( level ));
    }

    ILoggingContext IFlashtraceLogger.CurrentContext => throw new NotImplementedException();

    bool IFlashtraceLogger.IsEnabled( FlashtraceLevel level ) => this.IsEnabled( level );

    private class RecordBuilder : ILogRecordBuilder
    {
        private SimpleFlashtraceLogger _flashtraceLogger;
        private readonly StringBuilder _stringBuilder = new();
        private readonly FlashtraceLevel _level;
        private readonly LogRecordKind _recordKind;
        private Exception _exception;
        private readonly Context _context;
        private bool _appendComma;

        public RecordBuilder( SimpleFlashtraceLogger flashtraceLogger, FlashtraceLevel level, LogRecordKind recordKind, Context context )
        {
            this._flashtraceLogger = flashtraceLogger;
            this._level = level;
            this._recordKind = recordKind;
            this._context = context;
        }

        void ILogRecordBuilder.BeginWriteItem( LogRecordItem item, in LogRecordTextOptions options )
        {
            switch ( item )
            {
                case LogRecordItem.Message:
                case LogRecordItem.ActivityDescription:
                    if ( options.Name != null )
                    {
                        this._stringBuilder.Append( options.Name );
                    }

                    break;

                case LogRecordItem.ActivityOutcome:
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

        void ILogRecordBuilder.EndWriteItem( LogRecordItem item ) { }

        public void Dispose()
        {
            this._flashtraceLogger = null;
        }

        void ILogRecordBuilder.SetException( Exception e )
        {
            this._exception = e;
            this.WriteParameter( "exception", e, LogParameterOptions.SemanticParameter );
        }

        void ILogRecordBuilder.SetExecutionTime( double executionTime, bool isOvertime ) { }

        void ILogRecordBuilder.WriteParameter<T>( int index, in ReadOnlySpan<char> parameterName, T? value, in LogParameterOptions options )
            where T : default
        {
            this.WriteParameter( parameterName.ToString(), value, options );
        }

        private void WriteParameter<T>( string parameterName, T value, in LogParameterOptions options )
        {
            switch ( options.Mode )
            {
                default:
                    this.WriteValue( value );

                    break;

                case LogParameterMode.NameValuePair:
                    this._stringBuilder.Append( ", " );
                    this._stringBuilder.Append( parameterName );
                    this._stringBuilder.Append( " = " );
                    this.WriteValue( value );

                    break;

                case LogParameterMode.Hidden:
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

        void ILogRecordBuilder.WriteString( in ReadOnlySpan<char> span )
        {
            if ( this._appendComma )
            {
                this._appendComma = false;
                this._stringBuilder.Append( ", " );
            }

            this._stringBuilder.PortableAppend( span );
        }

        void ILogRecordBuilder.Complete()
        {
            if ( this._flashtraceLogger != null )
            {
                if ( this._recordKind == LogRecordKind.ActivityEntry )
                {
                    this._context.Description = this._stringBuilder.ToString();
                    this._stringBuilder.Append( ": " );
                    this._stringBuilder.Append( GetRecordKindText( LogRecordKind.ActivityEntry ) );
                }

                this._flashtraceLogger.Write( this._level, this._recordKind, this._stringBuilder.ToString(), this._exception );
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