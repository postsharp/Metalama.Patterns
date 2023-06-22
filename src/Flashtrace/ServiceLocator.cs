// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;

namespace Flashtrace
{
    /// <summary>
    /// A basic service locator used by PostSharp Patterns to find global services.
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// Event raised when a new service is registered.
        /// </summary>
        public static event EventHandler<ServiceRegisteredEventArgs> ServiceRegistered;

        /// <summary>
        /// Registers a service.
        /// </summary>
        /// <typeparam name="T">Type of the service interface.</typeparam>
        /// <param name="service">Service implementation.</param>
        public static void RegisterService<T>( T service ) where T : class
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
        public static T GetService<T>() where T : class
        {
            return Entry<T>.Value;
        }

        static ServiceLocator()
        {
            RegisterService<ILoggerFactory>( new TraceSourceLoggerFactory() );
            RegisterService<ILoggerFactoryProvider>( new TraceSourceLoggerFactory() );
        }

        private static class Entry<T> where T : class
        {
            public static T Value;
        }
    }

    /// <summary>
    /// Arguments of the <see cref="ServiceLocator.ServiceRegistered"/> event.
    /// </summary>
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
}