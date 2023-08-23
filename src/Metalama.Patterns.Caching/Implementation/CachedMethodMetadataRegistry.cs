﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Contracts;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// A cache of <see cref="CachedMethodMetadata"/> objects keyed on <see cref="MethodInfo"/>.
/// </summary>
internal sealed class CachedMethodMetadataRegistry
{
    private readonly ConcurrentDictionary<MethodInfo, CachedMethodMetadata> _methodInfoCache = new();

    // ReSharper disable once MemberCanBeInternal
    // ReSharper disable once UnusedParameter.Global
    /// <summary>
    /// Registers a cached method. This method should only be called from code generated by the <see cref="CacheAttribute"/> aspect.
    /// </summary>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public CachedMethodMetadata Register(
        [Required] MethodInfo method,
        Type? awaitableResultType,
        [Required] ICacheItemConfiguration buildTimeConfiguration,
        bool returnValueCanBeNull )
    {
        var toAdd = new CachedMethodMetadata(
            method,
            GetCachedParameterInfos( method ),
            awaitableResultType,
            buildTimeConfiguration.IgnoreThisParameter.GetValueOrDefault(),
            buildTimeConfiguration,
            returnValueCanBeNull );

        if ( !this._methodInfoCache.TryAdd( method, toAdd ) )
        {
            throw new CachingAssertionFailedException( $"The method '{method}' has already been registered." );
        }

        return toAdd;
    }

    /// <summary>
    /// Gets the <see cref="CachedMethodMetadata"/> for a given <see cref="MethodInfo"/>, or <see langword="null"/> if no <see cref="CachedMethodMetadata"/> was registered for <paramref name="method"/>.
    /// </summary>
    /// <param name="method">A <see cref="MethodInfo"/>.</param>
    /// <returns>The <see cref="CachedMethodMetadata"/> for <paramref name="method"/>, or <c>null</c> if no <see cref="CachedMethodMetadata"/> was registered for <paramref name="method"/>.</returns>
    /// <remarks>
    /// If no <see cref="CachedMethodMetadata"/> has been registered for the given <paramref name="method"/>, this method will run the class constructor
    /// of the declaring type of the method then lookup the registration again.
    /// </remarks>
    internal CachedMethodMetadata? Get( [Required] MethodInfo method )
    {
        if ( !this._methodInfoCache.TryGetValue( method, out var cachedMethodInfo ) )
        {
            // Perhaps the declaring type has not been initialized. Try to initialize it ourselves.
            RuntimeHelpers.RunClassConstructor( method.DeclaringType!.TypeHandle );

            if ( !this._methodInfoCache.TryGetValue( method, out cachedMethodInfo ) )
            {
                // Original code threw a perhaps misleading "Declaring type of '{0}' method has not been initialized." exception
                // in this case. However, the documented behaviour (including original documented behaviour) is to
                // return null if not found. However, the original code would never return null.
                return null;
            }
        }

        return cachedMethodInfo;
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