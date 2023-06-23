// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#define TRACE // Because TraceEvent has [Conditional("TRACE")]

using System.Collections.Concurrent;
using System.Diagnostics;

namespace Flashtrace;

internal sealed class TraceSourceLogger : LegacySourceLogger
{
    private readonly TraceSource _traceSource;
    private static readonly ConcurrentDictionary<string, TraceSource> _traceSources = new( StringComparer.OrdinalIgnoreCase );

    internal TraceSourceLogger( ILoggerFactory factory, string role, Type type ) : base( role, type )
    {
        this.Factory = factory;
        this._traceSource = GetTraceSource( role );
    }

    public override ILoggerFactory Factory { get; }

    private static string GetSourceName( string role )
    {
        return "Flashtrace." + role;
    }

    public static TraceSource GetTraceSource( string role = LoggingRoles.Default )
    {
        var sourceName = GetSourceName( role );

        return _traceSources.GetOrAdd( sourceName, n => new TraceSource( n ) );
    }

    public override bool IsEnabled( LogLevel level )
    {
        if ( level == LogLevel.None )
        {
            return false;
        }

        return this._traceSource.Switch.ShouldTrace( GetTraceEventType( level ) );
    }

    protected override void Write( LogLevel level, LogRecordKind recordKind, string text, Exception exception )
    {
        var eventType = GetTraceEventType( level );
        this._traceSource.TraceEvent( eventType, 0, text );
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