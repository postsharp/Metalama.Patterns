// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: !!! [Porting] Disabled: Logging in caching tests currently rely on TextLoggingBackend.
#if false

using Xunit;

namespace Metalama.Patterns.Caching.Tests
{
    internal static class LoggingInitializer
    {
        [ModuleInitializer( 1 )]
        public static void Initialize()
        {
            Assert.False( LoggingServices.IsRuntimeLicenseExpired );
            LoggingServices.DefaultBackend = new ConsoleLoggingBackend();

            // By default, logging is disabled:
            Assert.False(
                LoggingServices.DefaultBackend.CurrentContextLocalConfiguration.Verbosity.GetSource( LoggingRoles.Caching, typeof(LoggingInitializer) )
                    .IsEnabled( LogLevel.Debug ) );

            // But we enable it:
            LoggingServices.DefaultBackend.DefaultContextLocalConfiguration.Verbosity.SetMinimalLevel( LogLevel.Debug, LoggingRoles.Caching );

            Assert.True(
                LoggingServices.DefaultBackend.CurrentContextLocalConfiguration.Verbosity.GetSource( LoggingRoles.Caching, typeof(LoggingInitializer) )
                    .IsEnabled( LogLevel.Debug ) );
        }
    }
}
#endif