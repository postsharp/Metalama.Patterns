// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Records;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Flashtrace.Loggers;

internal sealed class NetCoreSourceLoggerFactory : IFlashtraceLoggerFactory
{
    private readonly ILoggerFactory _underlyingLoggerFactory;
    private readonly HashSet<string> _enabledRoles;
    private readonly ConcurrentDictionary<string, IFlashtraceRoleLoggerFactory> _roleLoggerFactories = new();

    public NetCoreSourceLoggerFactory( ILoggerFactory underlyingLoggerFactory, IEnumerable<string> enabledRoles )
    {
        this._underlyingLoggerFactory = underlyingLoggerFactory;
        this._enabledRoles = new HashSet<string>();

        foreach ( var role in enabledRoles )
        {
            this._enabledRoles.Add( role );
        }
    }

    public IFlashtraceRoleLoggerFactory ForRole( string role )
    {
        if ( this._roleLoggerFactories.TryGetValue( role, out var factory ) )
        {
            return factory;
        }

        return this._roleLoggerFactories.GetOrAdd(
            role,
            _ =>
            {
                if ( this._enabledRoles.Contains( role ) )
                {
                    return new RoleLoggerFactory( this._underlyingLoggerFactory, role );
                }
                else
                {
                    return new NullFlashtraceLogger();
                }
            } );
    }

    private sealed class RoleLoggerFactory : IFlashtraceRoleLoggerFactory
    {
        private readonly ILoggerFactory _underlyingLoggerFactory;
        private readonly ConcurrentDictionary<string, Logger> _loggers = new();
        private readonly string _role;

        public RoleLoggerFactory( ILoggerFactory underlyingLoggerFactory, string role )
        {
            this._underlyingLoggerFactory = underlyingLoggerFactory;
            this._role = role;
        }

        public IFlashtraceLogger GetLogger( Type type ) => this.GetLogger( type.FullName! );

        public IFlashtraceLogger GetLogger( string sourceName )
        {
            if ( this._loggers.TryGetValue( sourceName, out var logger ) )
            {
                return logger;
            }

            return this._loggers.GetOrAdd( sourceName, s => new Logger( this._role, s, this._underlyingLoggerFactory.CreateLogger( sourceName ), this ) );
        }
    }

    private sealed class Logger : SimpleFlashtraceLogger
    {
        private readonly ILogger _underlyingLogger;

        public Logger( string role, string name, ILogger underlyingLogger, IFlashtraceRoleLoggerFactory factory ) : base( role, name )
        {
            this._underlyingLogger = underlyingLogger;
            this.Factory = factory;
        }

        public override bool IsEnabled( FlashtraceLevel level ) => this._underlyingLogger.IsEnabled( level.ToLogLevel() );

        public override IFlashtraceRoleLoggerFactory Factory { get; }

        protected override void Write( FlashtraceLevel level, LogRecordKind recordKind, string text, Exception exception )
        {
            this._underlyingLogger.Log( level.ToLogLevel(), exception, text );
        }
    }
}