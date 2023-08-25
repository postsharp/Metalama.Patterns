// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Loggers;

internal sealed class TraceSourceLoggerFactory : IRoleLoggerFactory, ILoggerFactory
{
    ILogger IRoleLoggerFactory.GetLogger( Type type )
    {
        throw new NotImplementedException();
    }

    ILogger IRoleLoggerFactory.GetLogger( string sourceName )
    {
        throw new NotImplementedException();
    }

    IRoleLoggerFactory ILoggerFactory.ForRole( string role ) => new Factory( role );

    private sealed class Factory : IRoleLoggerFactory
    {
        private readonly string _role;

        public Factory( string role )
        {
            this._role = role;
        }

        ILogger IRoleLoggerFactory.GetLogger( Type type ) => new TraceSourceLogger( this, this._role, type.FullName! );

        public ILogger GetLogger( string sourceName ) => new TraceSourceLogger( this, this._role, sourceName );
    }
}