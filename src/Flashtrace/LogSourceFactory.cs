// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Custom;

namespace Flashtrace
{
    [ExplicitCrossPackageInternal]
    internal static class LogSourceFactory
    {
        public static ILoggerFactory3 Default3 => ServiceLocator.GetService<ILoggerFactoryProvider3>().GetLoggerFactory3( LoggingRoles.Custom );

        public static ILoggerFactory3 ForRole3( string role ) => ServiceLocator.GetService<ILoggerFactoryProvider3>().GetLoggerFactory3( role );
    }
}