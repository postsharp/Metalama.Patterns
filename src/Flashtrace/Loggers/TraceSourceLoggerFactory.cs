// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Loggers;

internal sealed class TraceSourceLoggerFactory : IFlashtraceLoggerFactory
{
    IFlashtraceRoleLoggerFactory IFlashtraceLoggerFactory.ForRole( string role ) => new RoleLoggerFactory( role );

    private sealed class RoleLoggerFactory : IFlashtraceRoleLoggerFactory
    {
        private readonly string _role;

        public RoleLoggerFactory( string role )
        {
            this._role = role;
        }

        IFlashtraceLogger IFlashtraceRoleLoggerFactory.GetLogger( Type type ) => new TraceSourceFlashtraceLogger( this, this._role, type.FullName! );

        public IFlashtraceLogger GetLogger( string sourceName ) => new TraceSourceFlashtraceLogger( this, this._role, sourceName );
    }
}