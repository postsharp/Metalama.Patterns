// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Formatters;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Formatters;
using Metalama.Patterns.Caching.Implementation;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Caching;

/// <summary>
/// The entry point to configure <c>Metalama.Patterns.Caching</c> at run-time.
/// </summary>
[PublicAPI]
public static partial class CachingServices
{
    private static readonly LogSource _defaultLogger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( typeof(CachingServices) );
    private static volatile CacheKeyBuilder _keyBuilder = new();
    private static volatile CachingBackend _backend = new UninitializedCachingBackend();

    public static FormatterRepository Formatters { get; } = new CachingFormatterRepository( CachingFormattingRole.Instance );

    /// <summary>
    /// Gets the <see cref="CachedMethodMetadataRegistry"/>.
    /// </summary>
    internal static CachedMethodMetadataRegistry CachedMethodMetadataRegistry { get; } = new();

    /// <summary>
    /// Gets or sets the <see cref="CacheKeyBuilder"/> used to generate caching keys, i.e. to serialize objects into a <see cref="string"/>.
    /// </summary>
    [AllowNull]
    public static CacheKeyBuilder DefaultKeyBuilder
    {
        get => _keyBuilder;
        set => _keyBuilder = value ?? new CacheKeyBuilder();
    }

    /// <summary>
    /// Gets or sets the default <see cref="CachingBackend"/>, i.e. the physical storage of cache items.
    /// </summary>
    [AllowNull]
    public static CachingBackend DefaultBackend
    {
        get => _backend;
        set
        {
            if ( _backend == value )
            {
                return;
            }

            _backend = value ?? new NullCachingBackend();
        }
    }

    /// <summary>
    /// Gets the repository of caching profiles (<see cref="CachingProfile"/>).
    /// </summary>
    public static CachingProfileRegistry Profiles { get; } = new();

    /// <summary>
    /// Gets the current caching context, so dependencies can be added.
    /// </summary>
    public static ICachingContext CurrentContext => CachingContext.Current;

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
    public static IDisposable SuspendDependencyPropagation()
    {
        return CachingContext.OpenSuspendedCacheContext();
    }
}