﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Caching.Implementation;

/// <summary>
/// Configuration of a <see cref="CacheItem"/>.
/// </summary>
[CompileTime]
internal sealed class CompileTimeCacheItemConfiguration
{
    // This class is near-identical to CacheItemConfiguration, but can be used exclusively at compile time.
    // It's not practical to make CacheItemConfiguration [RunTimeOrCompileTime] because this
    // then spreads the [RunTimeOrCompileTime] requirement a wide body of code.

    public CompileTimeCacheItemConfiguration() { }

    public string? ProfileName { get; set; }

    public bool? AutoReload { get; set; }

    public TimeSpan? AbsoluteExpiration { get; set; }

    public TimeSpan? SlidingExpiration { get; set; }

    public CacheItemPriority? Priority { get; set; }

    public bool? IgnoreThisParameter { get; set; }

    public void ApplyFallback( CompileTimeCacheItemConfiguration fallback )
    {
        this.AutoReload ??= fallback.AutoReload;
        this.AbsoluteExpiration ??= fallback.AbsoluteExpiration;
        this.SlidingExpiration ??= fallback.SlidingExpiration;
        this.Priority ??= fallback.Priority;
        this.ProfileName ??= fallback.ProfileName;
        this.IgnoreThisParameter ??= fallback.IgnoreThisParameter;
    }

    internal CompileTimeCacheItemConfiguration Clone() => (CompileTimeCacheItemConfiguration) this.MemberwiseClone();

    /// <summary>
    /// Initializes a new instance of the <see cref="CompileTimeCacheItemConfiguration"/> class from the given
    /// custom attribute which must have construction semantics equivalent to <see cref="CacheConfigurationAttribute"/>.
    /// </summary>
    internal CompileTimeCacheItemConfiguration( IAttribute attribute )
    {
        if ( attribute == null )
        {
            throw new ArgumentNullException( nameof( attribute ) );
        }

        try
        {
            if ( attribute.TryGetNamedArgument( nameof( CacheConfigurationAttribute.ProfileName ), out var profileName ) )
            {
                this.ProfileName = (string?) profileName.Value;
            }

            if ( attribute.TryGetNamedArgument( nameof( CacheConfigurationAttribute.AutoReload ), out var autoReload ) )
            {
                this.AutoReload = (bool) autoReload.Value!;
            }

            if ( attribute.TryGetNamedArgument( nameof( CacheConfigurationAttribute.AbsoluteExpiration ), out var absoluteExpiration ) )
            {
                this.AbsoluteExpiration = TimeSpan.FromMinutes( (double) absoluteExpiration.Value! );
            }

            if ( attribute.TryGetNamedArgument( nameof( CacheConfigurationAttribute.SlidingExpiration ), out var slidingExpiration ) )
            {
                this.SlidingExpiration = TimeSpan.FromMinutes( (double) slidingExpiration.Value! );
            }

            if ( attribute.TryGetNamedArgument( nameof( CacheConfigurationAttribute.Priority ), out var priority ) )
            {
                this.Priority = (CacheItemPriority) priority.Value!;
            }

            if ( attribute.TryGetNamedArgument( nameof( CacheConfigurationAttribute.IgnoreThisParameter ), out var ignoreThisParameter ) )
            {
                this.IgnoreThisParameter = (bool) ignoreThisParameter.Value!;
            }
        }
        catch ( Exception e )
        {
            throw new MetalamaPatternsCachingAssertionFailedException(
                "The construction semantics of the attribute do not match those of " + nameof( CacheConfigurationAttribute ) + ".",
                e );
        }
    }
}