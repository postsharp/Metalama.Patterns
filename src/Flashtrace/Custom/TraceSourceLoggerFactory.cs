// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#pragma warning disable CS0618 // Type or member is obsolete

namespace Flashtrace.Custom
{
#if SYSTEM_TRACE
    internal class TraceSourceLoggerFactory : ILoggerFactory, ILoggerFactoryProvider, ILoggerFactoryProvider3
    {
        ILogger ILoggerFactory.GetLogger(string role, Type type) => new Factory(role ).GetLogger( type);

        ILoggerFactory2 ILoggerFactoryProvider.GetLoggerFactory( string role ) => new Factory( role );
        
        ILoggerFactory3 ILoggerFactoryProvider3.GetLoggerFactory3( string role ) => new Factory( role );

        private class Factory : ILoggerFactory2, ILoggerFactory3
        {
            private readonly string role;

            public Factory( string role )
            {
                this.role = role;
            }

            public ILogger2 GetLogger( Type type ) => new TraceSourceLogger( this, this.role, type );
            
            // Probably no-one is using the Type property anyway, so let's just put null in there:
            ILogger3 ILoggerFactory3.GetLogger( Type type ) => new TraceSourceLogger( this, this.role, type );

            public ILogger3 GetLogger( string sourceName ) => new TraceSourceLogger( this, this.role, null );
            
        }
    }
#endif
}
