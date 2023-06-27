// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching
{
    /// <summary>
    /// The entry point to configure <c>PostSharp.Patterns.Caching</c> at run-time.
    /// </summary>
    public static partial class CachingServices
    {
        private static volatile CacheKeyBuilder keyBuilder = new CacheKeyBuilder();
        private static volatile CachingBackend backend = new UninitializedCachingBackend();
        private static readonly LogSource defaultLogger = LogSourceFactory.ForRole3( LoggingRoles.Caching ).GetLogSource( typeof(CachingServices) );


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static CachingServices()
        {
            Formatters.Initialize();
        }



        /// <summary>
        /// Gets or sets the <see cref="CacheKeyBuilder"/> used to generate caching keys, i.e. to serialize objects into a <see cref="string"/>.
        /// </summary>
        public static CacheKeyBuilder DefaultKeyBuilder
        {
            get { return keyBuilder; }
            set { keyBuilder = value ?? new CacheKeyBuilder(); }
        }

        /// <summary>
        /// Gets or sets the default <see cref="CachingBackend"/>, i.e. the physical storage of cache items.
        /// </summary>
        public static CachingBackend DefaultBackend
        {
            get { return backend; }
            set
            {
                if ( backend == value )
                    return;

                backend = value ?? new NullCachingBackend();

            }
        }

      
        /// <summary>
        /// Gets the repository of caching profiles (<see cref="CachingProfile"/>).
        /// </summary>
        public static CachingProfileRegistry Profiles { get; } = new CachingProfileRegistry();

        /// <summary>
        /// Gets the current caching context, so dependencies can be added.
        /// </summary>
        public static ICachingContext CurrentContext => CachingContext.Current;


#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Temporarily suspends propagation of dependencies from subsequently called methods to the caller method.
        /// </summary>
        /// <returns><see cref="IDisposable"/> representation of the suspension. Disposing this object resumes the normal dependency propagation.</returns>
        /// <remarks>
        /// <para>
        /// By default, calling a cached method while another <see cref="CachingContext"/> is active automatically adds the former as a dependency of the later. 
        /// Since the <see cref="CurrentContext"/> is stored in an <see cref="System.Threading.AsyncLocal{T}"/> variable, it may be inadvertently used after the method call associated with it
        /// had already ended. This can happen, for example, when method calls <see cref="System.Threading.Tasks.Task.Run(Action)"/> and does not depend on the resulting <see cref="System.Threading.Tasks.Task"/>.
        /// </para>
        /// <para>
        /// This context leak does not break correctness but may lead to unnecessary dependency invalidations. Therefore it is recommended to use this method when calling asynchronous code
        /// in the context of cached methods and not being dependent on its result.
        /// </para>
        /// </remarks>
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
        public static IDisposable SuspendDependencyPropagation()
        {
            return CachingContext.OpenSuspendedCacheContext();
        }
    }

  

}
