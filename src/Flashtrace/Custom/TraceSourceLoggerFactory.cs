// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Custom
{
    internal class TraceSourceLoggerFactory : ILoggerFactory, ILoggerFactoryProvider
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

        private class Factory : ILoggerFactory
        {
            private readonly string role;

            public Factory( string role )
            {
                this.role = role;
            }

            public ILogger GetLogger( Type type ) => new TraceSourceLogger( this, this.role, type );

            // Probably no-one is using the Type property anyway, so let's just put null in there:
            ILogger ILoggerFactory.GetLogger( Type type ) => new TraceSourceLogger( this, this.role, type );

            public ILogger GetLogger( string sourceName ) => new TraceSourceLogger( this, this.role, null );
        }
    }
}