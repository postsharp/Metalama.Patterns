// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Loggers;

internal sealed class TraceSourceLoggerFactory : ILoggerFactory
{
    IRoleLoggerFactory ILoggerFactory.ForRole( string role ) => new RoleLoggerFactory( role );

    private sealed class RoleLoggerFactory : IRoleLoggerFactory
    {
        private readonly string _role;

        public RoleLoggerFactory( string role )
        {
            this._role = role;
        }

        ILogger IRoleLoggerFactory.GetLogger( Type type ) => new TraceSourceLogger( this, this._role, type.FullName! );

        public ILogger GetLogger( string sourceName ) => new TraceSourceLogger( this, this._role, sourceName );
    }
}