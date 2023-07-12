// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using JetBrains.Annotations;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Encapsulates information about a method being cached. This class should only be used by the caching framework and the code it generates.
/// </summary>
/// <remarks>
/// The implementation of <see cref="CachedMethodRegistration"/> is intentionally opaque (ie, internal) as it should be used only by the caching framework runtime.
/// </remarks>
[PublicAPI]
[EditorBrowsable( EditorBrowsableState.Never )]
public sealed class CachedMethodRegistration
{
    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the method.
    /// </summary>
    internal MethodInfo Method { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the <c>this</c> parameter
    /// (for non-static methods) should be included in the cache key.
    /// </summary>
    internal bool IsThisParameterIgnored { get; }

    /// <summary>
    /// Gets an array of <see cref="CachedParameterInfo"/>.
    /// </summary>
    internal ImmutableArray<CachedParameterInfo> Parameters { get; }

    /// <summary>
    /// Gets a value indicating whether the return type of the method can be <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="ReturnValueCanBeNull"/> is only concerned with whether an instance of the type can be represented
    /// by <see langword="null"/>. For example, primitives like <see cref="int"/> and other non-nullable structs cannot
    /// be represented by <see langword="null"/>.
    /// </remarks>
    internal bool ReturnValueCanBeNull { get; }

    /// <summary>
    /// Gets a delegate that can invoke the original uncached method.
    /// </summary>
    /// <remarks>
    /// Only one of <see cref="InvokeOriginalMethod"/>, <see cref="InvokeOriginalMethodAsyncTask"/> and <see cref="InvokeOriginalMethodAsyncValueTask"/>
    /// is initialized, the others will be <see langword="null"/>.
    /// </remarks>
    internal Func<object?, object?[], object?>? InvokeOriginalMethod { get; }

    /// <summary>
    /// Gets a delegate that can invoke the original uncached async method.
    /// </summary>
    /// <remarks>
    /// Only one of <see cref="InvokeOriginalMethod"/>, <see cref="InvokeOriginalMethodAsyncTask"/> and <see cref="InvokeOriginalMethodAsyncValueTask"/>
    /// is initialized, the others will be <see langword="null"/>.
    /// </remarks>
    internal Func<object?, object?[], Task<object?>>? InvokeOriginalMethodAsyncTask { get; }

    /// <summary>
    /// Gets a delegate that can invoke the original uncached async method.
    /// </summary>
    /// <remarks>
    /// Only one of <see cref="InvokeOriginalMethod"/>, <see cref="InvokeOriginalMethodAsyncTask"/> and <see cref="InvokeOriginalMethodAsyncValueTask"/>
    /// is initialized, the others will be <see langword="null"/>.
    /// </remarks>
    internal Func<object?, object?[], ValueTask<object?>>? InvokeOriginalMethodAsyncValueTask { get; }

    /// <summary>
    /// Gets the build time configuration that applies to the method.
    /// </summary>
    /// <remarks>
    /// This already takes account of the any <see cref="CacheConfigurationAttribute"/> instances applied to parent classes and the assembly.
    /// </remarks>
    internal ICompileTimeCacheItemConfiguration BuildTimeConfiguration { get; }

    /// <summary>
    /// Gets the awaitable result type for awaitable (eg, async) methods, or <see langword="null"/> where not applicable.
    /// </summary>
    /// <remarks>
    /// For example, for a method returning <see cref="Task{TResult}"/>, this would be the generic argument corresponding to <c>TResult</c>.
    /// </remarks>
    internal Type? AwaitableResultType { get; }

    private CachingProfile? _profile;
    private int _profileRevision;
    private CacheItemConfiguration? _mergedConfiguration;
    private SpinLock _initializeLock;
    private LogSource? _logSource;

    /// <summary>
    /// Gets the logger for the current registration.
    /// </summary>
    internal LogSource Logger => this._logSource ??= LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( this.Method.DeclaringType! );

    /// <summary>
    /// Gets the effective configuration which is determined by merging the build-time configuration with any applicable 
    /// profile-based configuration. This property always reflects any runtime changes to profile configuration.
    /// </summary>
    internal IRunTimeCacheItemConfiguration MergedConfiguration
    {
        get
        {
            if ( this._profile == null || this._profileRevision < CachingServices.Profiles.RevisionNumber || this._mergedConfiguration == null )
            {
                var initializeLockTaken = false;

                try
                {
                    this._initializeLock.Enter( ref initializeLockTaken );

                    if ( this._profile == null || this._profileRevision < CachingServices.Profiles.RevisionNumber || this._mergedConfiguration == null )
                    {
                        var profileName = this.BuildTimeConfiguration.ProfileName ?? CachingProfile.DefaultName;

                        var localProfile = CachingServices.Profiles[profileName];
                        
                        this._mergedConfiguration = this.BuildTimeConfiguration.CloneAsCacheItemConfiguration();
                        this._mergedConfiguration.ApplyFallback( localProfile );

                        Thread.MemoryBarrier();

                        // Need to set this after setting mergedConfiguration to prevent data races.
                        this._profile = localProfile;
                        this._profileRevision = CachingServices.Profiles.RevisionNumber;
                    }
                }
                finally
                {
                    if ( initializeLockTaken )
                    {
                        this._initializeLock.Exit();
                    }
                }
            }

            return this._mergedConfiguration;
        }
    }

    private CachedMethodRegistration(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        bool isThisParameterIgnored,
        ICompileTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
    {
        this.Method = method;
        this.Parameters = parameters;
        this.IsThisParameterIgnored = isThisParameterIgnored;
        this.BuildTimeConfiguration = buildTimeConfiguration.CloneAsCacheItemConfiguration();
        this.ReturnValueCanBeNull = returnValueCanBeNull;
    }

    internal CachedMethodRegistration(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        bool isThisParameterIgnored,
        Func<object?, object?[], object?> invokeOriginalMethod,
        ICompileTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
        : this( method, parameters, isThisParameterIgnored, buildTimeConfiguration, returnValueCanBeNull )
    {
        this.InvokeOriginalMethod = invokeOriginalMethod;
    }

    internal CachedMethodRegistration(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        Type awaitableResultType,
        bool isThisParameterIgnored,
        Func<object?, object?[], Task<object?>> invokeOriginalMethodAsyncTask,
        ICompileTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
        : this( method, parameters, isThisParameterIgnored, buildTimeConfiguration, returnValueCanBeNull )
    {
        this.InvokeOriginalMethodAsyncTask = invokeOriginalMethodAsyncTask;
        this.AwaitableResultType = awaitableResultType;
    }

    internal CachedMethodRegistration(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        Type awaitableResultType,
        bool isThisParameterIgnored,
        Func<object?, object?[], ValueTask<object?>> invokeOriginalMethodAsyncValueTask,
        ICompileTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
        : this( method, parameters, isThisParameterIgnored, buildTimeConfiguration, returnValueCanBeNull )
    {
        this.InvokeOriginalMethodAsyncValueTask = invokeOriginalMethodAsyncValueTask;
        this.AwaitableResultType = awaitableResultType;
    }
}