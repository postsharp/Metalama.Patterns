// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using JetBrains.Annotations;
using System.Collections.Immutable;
using System.Reflection;
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Encapsulates information about a method being cached.
/// </summary>
[PublicAPI]
public sealed class CachedMethodRegistration
{
    /// <summary>
    /// Gets the <see cref="MethodInfo"/> of the method.
    /// </summary>
    public MethodInfo Method { get; }

    /// <summary>
    /// Gets a value indicating whether the value of the <c>this</c> parameter
    /// (for non-static methods) should be included in the cache key.
    /// </summary>
    public bool IsThisParameterIgnored { get; }

    /// <summary>
    /// Gets an array of <see cref="CachedParameterInfo"/>.
    /// </summary>
    public ImmutableArray<CachedParameterInfo> Parameters { get; }

    /// <summary>
    /// Gets a value indicating whether the return type of the method can be <see langword="null"/>.
    /// </summary>
    public bool ReturnValueCanBeNull { get; }

    /// <summary>
    /// Gets a delegate that can invoke the original uncached method.
    /// </summary>
    /// <remarks>
    /// Only one of <see cref="InvokeOriginalMethod"/>, <see cref="InvokeOriginalMethodAsyncTask"/> and <see cref="InvokeOriginalMethodAsyncValueTask"/>
    /// is initialized, the others will be <see langword="null"/>.
    /// </remarks>
    public Func<object?, object?[], object?>? InvokeOriginalMethod { get; }

    /// <summary>
    /// Gets a delegate that can invoke the original uncached async method.
    /// </summary>
    /// <remarks>
    /// Only one of <see cref="InvokeOriginalMethod"/>, <see cref="InvokeOriginalMethodAsyncTask"/> and <see cref="InvokeOriginalMethodAsyncValueTask"/>
    /// is initialized, the others will be <see langword="null"/>.
    /// </remarks>
    public Func<object?, object?[], Task<object?>>? InvokeOriginalMethodAsyncTask { get; }

    /// <summary>
    /// Gets a delegate that can invoke the original uncached async method.
    /// </summary>
    /// <remarks>
    /// Only one of <see cref="InvokeOriginalMethod"/>, <see cref="InvokeOriginalMethodAsyncTask"/> and <see cref="InvokeOriginalMethodAsyncValueTask"/>
    /// is initialized, the others will be <see langword="null"/>.
    /// </remarks>
    public Func<object?, object?[], ValueTask<object?>>? InvokeOriginalMethodAsyncValueTask { get; }

    /// <summary>
    /// Gets the build time configuration that applies to the method.
    /// </summary>
    /// <remarks>
    /// This already takes account of the any <see cref="CacheConfigurationAttribute"/> instances applied to parent classes and the assembly.
    /// </remarks>
    public IRunTimeCacheItemConfiguration BuildTimeConfiguration { get; }

    private static readonly CachingProfile _disabledProfile = new( "Disabled" ) { IsEnabled = false };
    private CachingProfile? _profile;
    private int _profileRevision;
    private CacheItemConfiguration? _mergedConfiguration;
    private SpinLock _initializeLock;
    private LogSource? _logSource;

    /// <summary>
    /// Gets the logger for the current registration.
    /// </summary>
    public LogSource Logger
        => this._logSource ??= LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( this.Method.DeclaringType! );

    /// <summary>
    /// Gets the effective configuration which is determined by merging the build-time configuration with any applicable 
    /// profile-based configuration. This property always reflects any runtime changes to profile configuration.
    /// </summary>
    public IRunTimeCacheItemConfiguration MergedConfiguration
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

                        if ( localProfile == null )
                        {
                            localProfile = _disabledProfile;
                            this._profile = _disabledProfile;

                            this.Logger
                                .Warning.Write(
                                    Formatted(
                                        "The cache is incorrectly configured for method {Method}: there is no profile named {Profile}.",
                                        this.Method,
                                        profileName ) );
                        }

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
        IRunTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
    {
        this.Method = method;
        this.Parameters = parameters;
        this.IsThisParameterIgnored = isThisParameterIgnored;
        this.BuildTimeConfiguration = buildTimeConfiguration;
        this.ReturnValueCanBeNull = returnValueCanBeNull;
    }

    internal CachedMethodRegistration(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        bool isThisParameterIgnored,
        Func<object?, object?[], object?> invokeOriginalMethod,
        IRunTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
        : this( method, parameters, isThisParameterIgnored, buildTimeConfiguration, returnValueCanBeNull )
    {
        this.InvokeOriginalMethod = invokeOriginalMethod;
    }

    internal CachedMethodRegistration(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        bool isThisParameterIgnored,
        Func<object?, object?[], Task<object?>> invokeOriginalMethodAsyncTask,
        IRunTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
        : this( method, parameters, isThisParameterIgnored, buildTimeConfiguration, returnValueCanBeNull )
    {
        this.InvokeOriginalMethodAsyncTask = invokeOriginalMethodAsyncTask;
    }

    internal CachedMethodRegistration(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        bool isThisParameterIgnored,
        Func<object?, object?[], ValueTask<object?>> invokeOriginalMethodAsyncValueTask,
        IRunTimeCacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
        : this( method, parameters, isThisParameterIgnored, buildTimeConfiguration, returnValueCanBeNull )
    {
        this.InvokeOriginalMethodAsyncValueTask = invokeOriginalMethodAsyncValueTask;
    }
}