// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

[PublicAPI]
public static class LogSourceFactory
{
    private static ILoggerFactoryProvider GetLoggerFactoryProvider()
        => ServiceLocator.GetService<ILoggerFactoryProvider>()
           ?? throw new InvalidOperationException( "The " + nameof(ILoggerFactoryProvider) + " service has not been registered." );

    public static ILoggerFactory Default => GetLoggerFactoryProvider().GetLoggerFactory( LoggingRoles.Default );

    public static ILoggerFactory ForRole( string role ) => GetLoggerFactoryProvider().GetLoggerFactory( role );
}