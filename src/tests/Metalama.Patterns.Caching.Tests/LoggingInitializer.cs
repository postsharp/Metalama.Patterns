using Xunit;
using Metalama.Aspects;
using Metalama.Patterns.Common.Tests.Helpers;
using Metalama.Patterns.Diagnostics;
using Metalama.Patterns.Diagnostics.Backends.Console;

namespace Metalama.Patterns.Caching.Tests
{
    static class LoggingInitializer
    {
        [ModuleInitializer( 1 )]
        public static void Initialize()
        {
            Assert.False(LoggingServices.IsRuntimeLicenseExpired);
            LoggingServices.DefaultBackend = new ConsoleLoggingBackend();
            // By default, logging is disabled:
            Assert.False( LoggingServices.DefaultBackend.CurrentContextLocalConfiguration.Verbosity.GetSource( LoggingRoles.Caching, typeof(LoggingInitializer) )
                                          .IsEnabled( LogLevel.Debug ));
            // But we enable it:
            LoggingServices.DefaultBackend.DefaultContextLocalConfiguration.Verbosity.SetMinimalLevel(LogLevel.Debug, LoggingRoles.Caching);
            Assert.True( LoggingServices.DefaultBackend.CurrentContextLocalConfiguration.Verbosity.GetSource( LoggingRoles.Caching, typeof(LoggingInitializer) )
                                           .IsEnabled( LogLevel.Debug ));
        }
    }
}
