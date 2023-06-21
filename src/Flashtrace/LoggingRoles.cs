// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;

namespace PostSharp.Patterns.Diagnostics
{
    /// <summary>
    /// List of standard logging roles.
    /// </summary>
    public static class LoggingRoles
    {
        /// <summary>
        /// Log records emitted by the Caching component.
        /// </summary>
        public const string Caching = "Cache";

        /// <summary>
        /// Default role for the Logging component.
        /// </summary>
        public const string Tracing = "Trace";

        /// <summary>
        /// Audit.
        /// </summary>
        [Obsolete( "This feature has been moved to the sample PostSharp.Samples.Logging.Audit." )]
        public const string Audit = "Audit";

        /// <summary>
        /// Logging of the logging component itself.
        /// </summary>
        public const string Meta = "Meta";

        /// <summary>
        /// Default role for custom logging using the <see cref="LogSource"/> class.
        /// </summary>
        public const string Custom = "Custom";

        [ExplicitCrossPackageInternal]
        internal static bool IsSystemRole( string role )
        {
            switch ( role )
            {
                case LoggingRoles.Meta:
                    return true;

                case LoggingRoles.Caching:
                    return true;

                default:
                    return false;
            }
        }

    }
}