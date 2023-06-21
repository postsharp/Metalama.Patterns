// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
#define TRACE // Because TraceEvent has [Conditional("TRACE")]

using System.Collections.Concurrent;
using System.Diagnostics;

namespace Flashtrace.Custom
{
    internal partial class TraceSourceLogger : LegacySourceLogger
    {
        private readonly TraceSource traceSource;
        private static readonly ConcurrentDictionary<string, TraceSource> traceSources = new ConcurrentDictionary<string, TraceSource>(StringComparer.OrdinalIgnoreCase );

        internal TraceSourceLogger( ILoggerFactory3 factory, string role, Type type ) : base( role, type )
        {
            this.Factory = factory;
            this.traceSource = GetTraceSource(role);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public override ILoggerFactory2 Factory { get; }
#pragma warning restore CS0618 // Type or member is obsolete

        private static string GetSourceName( string role )
        {
            return "PostSharp." + role;
        }

        public static TraceSource GetTraceSource( string role = "Custom" )
        {
            string sourceName = GetSourceName(role);
            return traceSources.GetOrAdd(sourceName, n => new TraceSource(n ) );
        }

        public override bool IsEnabled( LogLevel level )
        {
            if ( level == LogLevel.None )
                return false;
            return this.traceSource.Switch.ShouldTrace( GetTraceEventType( level ) );
        }

        protected override void Write( LogLevel level, LogRecordKind recordKind, string text, Exception exception )
        {
            TraceEventType eventType = GetTraceEventType( level );
            this.traceSource.TraceEvent( eventType, 0, text );
        }



        private static TraceEventType GetTraceEventType( LogLevel level )
        {
            switch ( level )
            {
                case LogLevel.Trace:
                    return TraceEventType.Verbose;
                case LogLevel.Debug:
                    return TraceEventType.Verbose;
                case LogLevel.Info:
                    return TraceEventType.Information;
                case LogLevel.Warning:
                    return TraceEventType.Warning;
                case LogLevel.Error:
                    return TraceEventType.Error;
                case LogLevel.Critical:
                    return TraceEventType.Critical;
                default:
                    throw new ArgumentOutOfRangeException( nameof(level), level, null );
            }
        }

 

    
    }
}
