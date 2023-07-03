// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Internal;

internal sealed class TraceSourceLoggerFactory : ILoggerFactory, ILoggerFactoryProvider
{
    ILogger ILoggerFactory.GetLogger( Type type )
    {
        throw new NotImplementedException();
    }

    ILogger ILoggerFactory.GetLogger( string sourceName )
    {
        throw new NotImplementedException();
    }

    ILoggerFactory ILoggerFactoryProvider.GetLoggerFactory( string role ) => new Factory( role );

    private sealed class Factory : ILoggerFactory
    {
        private readonly string _role;

        public Factory( string role )
        {
            this._role = role;
        }

        // Probably no-one is using the Type property anyway, so let's just put null in there:
        ILogger ILoggerFactory.GetLogger( Type type ) => new TraceSourceLogger( this, this._role, type );

        public ILogger GetLogger( string sourceName ) => new TraceSourceLogger( this, this._role, null! );
    }
}