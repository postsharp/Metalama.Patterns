// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// List of standard logging roles.
/// </summary>
[PublicAPI]
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
    /// Logging of the logging component itself.
    /// </summary>
    public const string Meta = "Meta";

    /// <summary>
    /// Default role for logging using the <see cref="LogSource"/> class.
    /// </summary>
    public const string Default = "Default";

    // Was [ExplicitCrossPackageInternal]
    public static bool IsSystemRole( string role )
    {
        switch ( role )
        {
            case Meta:
                return true;

            case Caching:
                return true;

            default:
                return false;
        }
    }
}