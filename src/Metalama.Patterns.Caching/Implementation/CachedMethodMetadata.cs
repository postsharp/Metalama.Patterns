// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Contracts;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Encapsulates information about a method being cached. This class should only be used by the caching framework and the code it generates.
/// </summary>
/// <remarks>
/// The implementation of <see cref="CachedMethodMetadata"/> is intentionally opaque (ie, internal) as it should be used only by the caching framework runtime.
/// </remarks>
[PublicAPI]
[EditorBrowsable( EditorBrowsableState.Never )]
public sealed class CachedMethodMetadata
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
    /// Gets the build time configuration that applies to the method.
    /// </summary>
    /// <remarks>
    /// This already takes account of the any <see cref="CachingConfigurationAttribute"/> instances applied to parent classes and the assembly.
    /// </remarks>
    internal ICacheItemConfiguration BuildTimeConfiguration { get; }

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

    /// <summary>
    /// Gets the effective configuration which is determined by merging the build-time configuration with any applicable 
    /// profile-based configuration. This property always reflects any runtime changes to profile configuration.
    /// </summary>
    internal ICacheItemConfiguration MergedConfiguration
    {
        get
        {
            if ( this._profile == null || this._profileRevision < CachingServices.Default.Profiles.RevisionNumber || this._mergedConfiguration == null )
            {
                var initializeLockTaken = false;

                try
                {
                    this._initializeLock.Enter( ref initializeLockTaken );

                    if ( this._profile == null || this._profileRevision < CachingServices.Default.Profiles.RevisionNumber
                                               || this._mergedConfiguration == null )
                    {
                        var profileName = this.BuildTimeConfiguration.ProfileName ?? CachingProfile.DefaultName;

                        var localProfile = CachingServices.Default.Profiles[profileName];

                        this._mergedConfiguration = this.BuildTimeConfiguration.AsCacheItemConfiguration().ApplyFallback( localProfile );

                        Thread.MemoryBarrier();

                        // Need to set this after setting mergedConfiguration to prevent data races.
                        this._profile = localProfile;
                        this._profileRevision = CachingServices.Default.Profiles.RevisionNumber;
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

    internal CachedMethodMetadata(
        MethodInfo method,
        ImmutableArray<CachedParameterInfo> parameters,
        Type? awaitableResultType,
        bool isThisParameterIgnored,
        ICacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
    {
        this.Method = method;
        this.Parameters = parameters;
        this.IsThisParameterIgnored = isThisParameterIgnored;
        this.BuildTimeConfiguration = buildTimeConfiguration.AsCacheItemConfiguration();
        this.ReturnValueCanBeNull = returnValueCanBeNull;
        this.AwaitableResultType = awaitableResultType;
    }

    public static CachedMethodMetadata Register(
        [Required] MethodInfo method,
        Type? awaitableResultType,
        [Required] ICacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
    {
        var metadata = new CachedMethodMetadata(
            method,
            GetCachedParameterInfos( method ),
            awaitableResultType,
            buildTimeConfiguration.IgnoreThisParameter.GetValueOrDefault(),
            buildTimeConfiguration,
            returnValueCanBeNull );

        CachedMethodMetadataRegistry.Instance.Register( metadata );

        return metadata;
    }

    private static ImmutableArray<CachedParameterInfo> GetCachedParameterInfos( MethodInfo method )
    {
        var parameterInfos = method.GetParameters();
        var cachedParameterInfos = new CachedParameterInfo[parameterInfos.Length];

        for ( var i = 0; i < parameterInfos.Length; i++ )
        {
            var isIgnored = parameterInfos[i].IsDefined( typeof(NotCacheKeyAttribute) );

            cachedParameterInfos[i] = new CachedParameterInfo( isIgnored );
        }

        return cachedParameterInfos.ToImmutableArray();
    }
}