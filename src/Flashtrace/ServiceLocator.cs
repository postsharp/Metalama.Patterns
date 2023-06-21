// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Custom;

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
        public static void RegisterService<T>(T service) where T : class
        {
            T oldService = Entry<T>.Value;
            (oldService as IDisposable)?.Dispose();

            Entry<T>.Value = service;
            ServiceRegistered?.Invoke(null, new ServiceRegisteredEventArgs(typeof(T)));
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ServiceLocator()
        {
#pragma warning disable CS0618 // Type or member is obsolete

#if true || SYSTEM_TRACE
            RegisterService<ILoggerFactory>(new TraceSourceLoggerFactory());
            RegisterService<ILoggerFactoryProvider>(new TraceSourceLoggerFactory());
            RegisterService<ILoggerFactoryProvider3>(new TraceSourceLoggerFactory());
#else
            RegisterService<ILoggerFactory>(new NullLogger());
            RegisterService<ILoggerFactoryProvider>(new NullLogger());
            RegisterService<ILoggerFactoryProvider3>(new NullLogger());
#endif

#pragma warning restore CS0618 // Type or member is obsolete
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
