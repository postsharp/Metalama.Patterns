// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Custom;

namespace Flashtrace
{
    [ExplicitCrossPackageInternal]
    internal static class LogSourceFactory
    {
        public static ILoggerFactory Default => ServiceLocator.GetService<ILoggerFactoryProvider>().GetLoggerFactory( LoggingRoles.Custom );

        public static ILoggerFactory ForRole( string role ) => ServiceLocator.GetService<ILoggerFactoryProvider>().GetLoggerFactory( role );
    }
}