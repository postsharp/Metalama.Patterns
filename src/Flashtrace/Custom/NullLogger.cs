// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Formatters;
using System.Diagnostics.CodeAnalysis;

namespace Flashtrace.Custom
{
#pragma warning disable CS0618 // Type or member is obsolete
    internal partial class NullLogger : ILogger3, IContextLocalLogger, ILoggerFactory, ILoggingContext, ICustomLogRecordBuilder, ILogActivityOptions, ILoggerFactoryProvider, ILoggerFactory2, ILoggerFactoryProvider3, ILoggerFactory3
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private static bool warningEmitted;

        string ILogger.Role => null;

        Type ILogger.Type => null;

        bool ILogger.RequiresSuspendResume => false;

        ILogger ILoggerFactory.GetLogger( string role, Type type ) => this;

        bool ILogger.IsEnabled( LogLevel level ) => false;
        bool IContextLocalLogger.IsEnabled(LogLevel level) => false;

        void ILogger.Write( ILoggingContext context, LogLevel level, LogRecordKind logRecordKind, string text, Exception exception, ref CallerInfo callerInfo ) => EmitWarning( level );


        void ILogger.Write( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, object[] args, Exception exception, ref CallerInfo callerInfo ) => EmitWarning( level );

        ILoggingContext ILogger.OpenActivity( LogActivityOptions options, ref CallerInfo callerInfo ) => this;

        ILoggingContext IContextLocalLogger.OpenActivity( in OpenActivityOptions options, ref CallerInfo callerInfo ) => this;
        void ILogger.ResumeActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

        void ILogger.SuspendActivity( ILoggingContext context, ref CallerInfo callerInfo ) { }

        void IContextLocalLogger.ResumeActivity(ILoggingContext context, ref CallerInfo callerInfo) { }

        void IContextLocalLogger.SuspendActivity(ILoggingContext context, ref CallerInfo callerInfo) { }

        void IContextLocalLogger.SetWaitDependency(ILoggingContext context, object waited) { }


        [SuppressMessage("Microsoft.Performance", "CA1822")]
        public ILoggingContext CurrentContext => null;

        bool ILoggingContext.IsDisposed => false;

        int ILoggingContext.RecycleId => 0;

        LogLevel ILogActivityOptions.ActivityLevel => LogLevel.None;

        LogLevel ILogActivityOptions.FailureLevel => LogLevel.None;

        LogLevel ILogActivityOptions.ExceptionLevel => LogLevel.None;

        [Obsolete]
        ILogActivityOptions ILogger.ActivityOptions => this;

        bool ILoggingContext.IsAsync => false;

        string ILoggingContext.SyntheticId => null;

        void ILoggingContext.ForEachProperty( LoggingPropertyVisitor<object> visitor, bool includeAncestors )
        {
        }

        void ILoggingContext.ForEachProperty<T>( LoggingPropertyVisitor<T> visitor, ref T state, bool includeAncestors )
        {
        }

#pragma warning disable CS0618 // Type or member is obsolete
        ILoggerFactory2 ILogger2.Factory => this;
#pragma warning restore CS0618 // Type or member is obsolete

        (IContextLocalLogger logger, bool isEnabled) ILogger3.GetContextLocalLogger( LogLevel level ) => (this, false);
        

        private static void EmitWarning( LogLevel level )
        {
            if ( warningEmitted || level < LogLevel.Warning )
            {
                return;
            }

            warningEmitted = true;

#if SYSTEM_TRACE
            Trace.TraceWarning( "PostSharp.Diagnostics.LoggingServices has not been initialized, but warnings and errors are being emitted and lost." );
#endif
        }


        void ILogger.SetWaitDependency( ILoggingContext context, object waited ) { }
        void IDisposable.Dispose() { }

        
        void ICustomLogRecordBuilder.WriteCustomParameter<T>( int index, in CharSpan parameterName, T value, in CustomLogParameterOptions format ) { }
        void ICustomLogRecordBuilder.WriteCustomString( in CharSpan str ) { }
        void ICustomLogRecordBuilder.SetException( Exception e ) { }
        void ICustomLogRecordBuilder.SetExecutionTime( double executionTime, bool isOvertime ) { }

        
        
        ICustomLogRecordBuilder IContextLocalLogger.GetRecordBuilder( in CustomLogRecordOptions recordInfo, ref CallerInfo callerInfo, ILoggingContext context ) => this;
        void ILoggerExceptionHandler.OnInvalidUserCode( ref CallerInfo callerInfo, string format, params object[] args ) { }
        void ILoggerExceptionHandler.OnInternalException( Exception exception ) { }

        void ICustomLogRecordBuilder.Complete() { }

        void ICustomLogRecordBuilder.BeginWriteItem(CustomLogRecordItem item, in CustomLogRecordTextOptions options) { }
        void ICustomLogRecordBuilder.EndWriteItem(CustomLogRecordItem item) { }

        ILoggerFactory3 ILogger3.Factory => this;

        IContextLocalLogger ILogger2.GetContextLocalLogger() => this;

#pragma warning disable CS0618 // Type or member is obsolete
        ILoggerFactory2 ILoggerFactoryProvider.GetLoggerFactory( string role ) => this;
#pragma warning restore CS0618 // Type or member is obsolete

        ILogger2 ILoggerFactory2.GetLogger( Type type ) => this;

        ILogger3 ILoggerFactory3.GetLogger( Type type ) => this;

        public ILogger3 GetLogger( string sourceName ) => this;

        public ILoggerFactory3 GetLoggerFactory3( string role ) => this;
    }
}
