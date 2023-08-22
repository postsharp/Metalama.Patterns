// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Loggers;
using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// A basic service locator used to find global services.
/// </summary>
[PublicAPI]
public static class LoggingServiceLocator
{
    /// <summary>
    /// Event raised when a new service is registered.
    /// </summary>
    public static event EventHandler<ServiceRegisteredEventArgs>? ServiceRegistered;

    /// <summary>
    /// Registers a service.
    /// </summary>
    /// <typeparam name="T">Type of the service interface.</typeparam>
    /// <param name="service">Service implementation.</param>
    public static void RegisterService<T>( T service )
        where T : class
    {
        var oldService = Entry<T>.Value;
        (oldService as IDisposable)?.Dispose();

        Entry<T>.Value = service;
        ServiceRegistered?.Invoke( null, new ServiceRegisteredEventArgs( typeof(T) ) );
    }

    /// <summary>
    /// Gets a service implementation.
    /// </summary>
    /// <typeparam name="T">Type of the requested service interface.</typeparam>
    /// <returns>An implementation of <typeparamref name="T"/>, or <c>null</c> if no such service has been registered.</returns>
    public static T? GetService<T>()
        where T : class
        => Entry<T>.Value;

    static LoggingServiceLocator()
    {
        // TODO: [FT-Review] Decide default startup behaviour - TraceSourceLogger vs NullLogger.
#if true
        RegisterService<ILoggerFactory>( new TraceSourceLoggerFactory() );
        RegisterService<ILoggerFactoryProvider>( new TraceSourceLoggerFactory() );
#else
        RegisterService<ILoggerFactory>(new NullLogger());
        RegisterService<ILoggerFactoryProvider>(new NullLogger());
#endif
    }

    private static class Entry<T>
        where T : class
    {
#pragma warning disable SA1401
        public static T? Value;
#pragma warning restore SA1401
    }
}