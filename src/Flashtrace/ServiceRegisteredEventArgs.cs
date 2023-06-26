// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Arguments of the <see cref="LoggingServiceLocator.ServiceRegistered"/> event.
/// </summary>
[PublicAPI]
public sealed class ServiceRegisteredEventArgs : EventArgs
{
    /// <summary>
    /// Gets the <see cref="Type"/> of the service interface.
    /// </summary>
    public Type ServiceType { get; }

    internal ServiceRegisteredEventArgs( Type type )
    {
        this.ServiceType = type;
    }
}