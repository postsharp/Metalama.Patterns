// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

[Obsolete( "Consider using dependency injection." )]
[PublicAPI]
public static class LogSourceFactory
{
    private static ILoggerFactory GetLoggerFactoryProvider()
        => LoggingServiceLocator.GetService<ILoggerFactory>()
           ?? throw new InvalidOperationException( "The " + nameof(ILoggerFactory) + " service has not been registered." );

    public static IRoleLoggerFactory Default => GetLoggerFactoryProvider().ForRole( LoggingRoles.Default );

    public static IRoleLoggerFactory ForRole( string role ) => GetLoggerFactoryProvider().ForRole( role );
}