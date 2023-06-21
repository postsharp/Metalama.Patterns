// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using PostSharp.Patterns.Diagnostics.Custom;
using PostSharp.Patterns.Utilities;

namespace PostSharp.Patterns.Diagnostics
{
    [ExplicitCrossPackageInternal]
    internal static class LogSourceFactory
    {
        public static ILoggerFactory3 Default3 => ServiceLocator.GetService<ILoggerFactoryProvider3>().GetLoggerFactory3( LoggingRoles.Custom );

        public static ILoggerFactory3 ForRole3( string role ) => ServiceLocator.GetService<ILoggerFactoryProvider3>().GetLoggerFactory3( role );
    }
}


