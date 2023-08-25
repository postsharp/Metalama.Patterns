// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Loggers;
using JetBrains.Annotations;

namespace Flashtrace;

[Obsolete( "Use dependency injection." )]
[PublicAPI]
public static class LogSourceFactory
{
    public static ILoggerFactory DefaultFactory { get; set; } = new NullLogger();
    
    public static IRoleLoggerFactory Default => DefaultFactory.ForRole( LoggingRoles.Default );

    public static IRoleLoggerFactory ForRole( string role ) => DefaultFactory.ForRole( role );
}