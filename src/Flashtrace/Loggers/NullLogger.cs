// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Options;
using Flashtrace.Records;
using System.Diagnostics;

namespace Flashtrace.Loggers;

// ReSharper disable once UnusedType.Global : Usage is conditional.
internal partial class NullLogger : ILogger, IContextLocalLogger, IRoleLoggerFactory, ILoggingContext, ILogRecordBuilder,
                                    ILoggerFactory
{
    private static bool _warningEmitted;

    string ILogger.Role => null!;

    bool ILogger.RequiresSuspendResume => false;

    bool ILogger.IsEnabled( LogLevel level ) => false;

    bool IContextLocalLogger.IsEnabled( LogLevel level ) => false;

    void ILogger.Write( ILoggingContext? context, LogLevel level, LogRecordKind logRecordKind, string text, Exception? exception, in CallerInfo callerInfo )
        => EmitWarning( level );

    void ILogger.Write(
        ILoggingContext? context,
        LogLevel level,
        LogRecordKind recordKind,
        string text,
        object[] args,
        Exception? exception,
        in CallerInfo callerInfo )
        => EmitWarning( level );

    ILoggingContext ILogger.OpenActivity( in LogActivityOptions options, in CallerInfo callerInfo ) => this;

    ILoggingContext IContextLocalLogger.OpenActivity( in OpenActivityOptions options, in CallerInfo callerInfo, bool isAsync ) => this;

    void ILogger.ResumeActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void ILogger.SuspendActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void IContextLocalLogger.ResumeActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void IContextLocalLogger.SuspendActivity( ILoggingContext context, in CallerInfo callerInfo ) { }

    void IContextLocalLogger.SetWaitDependency( ILoggingContext context, object waited ) { }

    public ILoggingContext CurrentContext => null!;

    bool ILoggingContext.IsDisposed => false;

    int ILoggingContext.RecycleId => 0;

    bool ILoggingContext.IsAsync => false;

    string ILoggingContext.SyntheticId => null!;

    IRoleLoggerFactory ILogger.Factory => this;

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

#pragma warning disable SA1000 // Keywords should be spaced correctly
    void ILogRecordBuilder.WriteParameter<T>( int index, in ReadOnlySpan<char> parameterName, T? value, in LogParameterOptions options )
        where T : default { }
#pragma warning restore SA1000 // Keywords should be spaced correctly

    void ILogRecordBuilder.WriteString( in ReadOnlySpan<char> str ) { }

    void ILogRecordBuilder.SetException( Exception e ) { }

    void ILogRecordBuilder.SetExecutionTime( double executionTime, bool isOvertime ) { }

    ILogRecordBuilder IContextLocalLogger.GetRecordBuilder( in LogRecordOptions recordInfo, in CallerInfo callerInfo, ILoggingContext? context ) => this;

    void ILoggerExceptionHandler.OnInvalidUserCode( in CallerInfo callerInfo, string format, params object[] args ) { }

    void ILoggerExceptionHandler.OnInternalException( Exception exception ) { }

    void ILogRecordBuilder.Complete() { }

    void ILogRecordBuilder.BeginWriteItem( LogRecordItem item, in LogRecordTextOptions options ) { }

    void ILogRecordBuilder.EndWriteItem( LogRecordItem item ) { }

    IContextLocalLogger ILogger.GetContextLocalLogger() => this;

    IRoleLoggerFactory ILoggerFactory.ForRole( string role ) => this;

    ILogger IRoleLoggerFactory.GetLogger( Type type ) => this;

    public ILogger GetLogger( string sourceName ) => this;
}