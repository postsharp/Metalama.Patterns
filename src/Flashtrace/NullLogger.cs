// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Formatters;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Flashtrace;

// ReSharper disable once UnusedType.Global : Usage is conditional.
internal partial class NullLogger : ILogger, IContextLocalLogger, ILoggerFactory, ILoggingContext, ICustomLogRecordBuilder, ILogActivityOptions,
                                    ILoggerFactoryProvider
{
    private static bool _warningEmitted;

    string ILogger.Role => null!;

    bool ILogger.RequiresSuspendResume => false;

    bool ILogger.IsEnabled( LogLevel level ) => false;

    bool IContextLocalLogger.IsEnabled( LogLevel level ) => false;

    void ILogger.Write( ILoggingContext? context, LogLevel level, LogRecordKind logRecordKind, string text, Exception? exception, ref CallerInfo callerInfo )
        => EmitWarning( level );

    void ILogger.Write(
        ILoggingContext? context,
        LogLevel level,
        LogRecordKind recordKind,
        string text,
        object[] args,
        Exception? exception,
        ref CallerInfo callerInfo )
        => EmitWarning( level );

    ILoggingContext ILogger.OpenActivity( LogActivityOptions options, ref CallerInfo callerInfo ) => this;

    ILoggingContext IContextLocalLogger.OpenActivity( in OpenActivityOptions options, ref CallerInfo callerInfo ) => this;

    void ILogger.ResumeActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

    void ILogger.SuspendActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

    void IContextLocalLogger.ResumeActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

    void IContextLocalLogger.SuspendActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

    void IContextLocalLogger.SetWaitDependency( ILoggingContext context, object waited ) { }

    public ILoggingContext CurrentContext => null!;

    bool ILoggingContext.IsDisposed => false;

    int ILoggingContext.RecycleId => 0;

    LogLevel ILogActivityOptions.ActivityLevel => LogLevel.None;

    LogLevel ILogActivityOptions.FailureLevel => LogLevel.None;

    LogLevel ILogActivityOptions.ExceptionLevel => LogLevel.None;

    // TODO: Implementation is [Obsolete], but base interface property is not.
    [Obsolete]
    ILogActivityOptions ILogger.ActivityOptions => this;

    bool ILoggingContext.IsAsync => false;

    string ILoggingContext.SyntheticId => null!;

    ILoggerFactory ILogger.Factory => this;

    (IContextLocalLogger Logger, bool IsEnabled) ILogger.GetContextLocalLogger( LogLevel level ) => (this, false);

    private static void EmitWarning( LogLevel level )
    {
        if ( _warningEmitted || level < LogLevel.Warning )
        {
            return;
        }

        _warningEmitted = true;

        Trace.TraceWarning( "Flashtrace has not been initialized, but warnings and errors are being emitted and lost." );
    }

    void ILogger.SetWaitDependency( ILoggingContext context, object waited ) { }

    void IDisposable.Dispose() { }

    void ICustomLogRecordBuilder.WriteCustomParameter<T>( int index, in CharSpan parameterName, [AllowNull] T value, in CustomLogParameterOptions options ) { }
    
    void ICustomLogRecordBuilder.WriteCustomString( in CharSpan str ) { }

    void ICustomLogRecordBuilder.SetException( Exception e ) { }

    void ICustomLogRecordBuilder.SetExecutionTime( double executionTime, bool isOvertime ) { }

    ICustomLogRecordBuilder IContextLocalLogger.GetRecordBuilder( in CustomLogRecordOptions recordInfo, ref CallerInfo callerInfo, ILoggingContext? context )
        => this;

    void ILoggerExceptionHandler.OnInvalidUserCode( ref CallerInfo callerInfo, string format, params object[] args ) { }

    void ILoggerExceptionHandler.OnInternalException( Exception exception ) { }

    void ICustomLogRecordBuilder.Complete() { }

    void ICustomLogRecordBuilder.BeginWriteItem( CustomLogRecordItem item, in CustomLogRecordTextOptions options ) { }

    void ICustomLogRecordBuilder.EndWriteItem( CustomLogRecordItem item ) { }

    IContextLocalLogger ILogger.GetContextLocalLogger() => this;

    ILoggerFactory ILoggerFactoryProvider.GetLoggerFactory( string role ) => this;

    ILogger ILoggerFactory.GetLogger( Type type ) => this;

    public ILogger GetLogger( string sourceName ) => this;
}