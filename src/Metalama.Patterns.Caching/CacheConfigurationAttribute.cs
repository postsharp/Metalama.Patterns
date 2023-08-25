// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Custom attribute that, when applied on a type, configures the <see cref="CacheAttribute"/> aspects applied to the methods of this type
/// or its derived types. When applied to an assembly, the <see cref="CacheConfigurationAttribute"/> custom attribute configures all methods
/// of the current assembly.
/// </summary>
/// <remarks>
/// <para>Any <see cref="CacheConfigurationAttribute"/> on the base class has always priority over a <see cref="CacheConfigurationAttribute"/>
/// on the assembly, even if the base class is in a different assembly.</para>
/// </remarks>
[PublicAPI]
[AttributeUsage( AttributeTargets.Class | AttributeTargets.Assembly )]
public sealed class CacheConfigurationAttribute : Attribute
{
    internal CacheItemConfiguration Configuration { get; private set; } = new();

    /// <summary>
    /// Gets or sets the name of the <see cref="CachingProfile"/> that contains the configuration of the cached methods.
    /// </summary>
    public string ProfileName
    {
        get => this.Configuration.ProfileName ?? CachingProfile.DefaultName;
        set => this.Configuration = this.Configuration with { ProfileName = value };
    }

    /// <summary>
    /// Gets or sets a value indicating whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
    /// when the cache item is removed from the cache.
    /// </summary>
    public bool AutoReload
    {
        get => this.Configuration.AutoReload.GetValueOrDefault();
        set => this.Configuration = this.Configuration with { AutoReload = value };
    }

    /// <summary>
    /// Gets or sets the total duration, in minutes, during which the result of the cached method  is stored in cache. The absolute
    /// expiration time is counted from the moment the method is evaluated and cached.
    /// </summary>
    public double AbsoluteExpiration
    {
        get => this.Configuration.AbsoluteExpiration.GetValueOrDefault( TimeSpan.Zero ).TotalMinutes;
        set => this.Configuration = this.Configuration with { AbsoluteExpiration = TimeSpan.FromMinutes( value ) };
    }

    /// <summary>
    /// Gets or sets the duration, in minutes, during which the result of the cached method is stored in cache after it has been
    /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
    /// </summary>
    public double SlidingExpiration
    {
        get => this.Configuration.SlidingExpiration.GetValueOrDefault( TimeSpan.Zero ).TotalMinutes;
        set => this.Configuration = this.Configuration with { SlidingExpiration = TimeSpan.FromMinutes( value ) };
    }

    /// <summary>
    /// Gets or sets the priority of the cached method.
    /// </summary>
    public CacheItemPriority Priority
    {
        get => this.Configuration.Priority.GetValueOrDefault( CacheItemPriority.Default );
        set => this.Configuration = this.Configuration with { Priority = value };
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>this</c> instance should be a part of the cache key. The default value of this property is <c>false</c>,
    /// which means that by default the <c>this</c> instance is a part of the cache key.
    /// </summary>
    public bool IgnoreThisParameter
    {
        get => this.Configuration.IgnoreThisParameter.GetValueOrDefault();
        set => this.Configuration = this.Configuration with { IgnoreThisParameter = value };
    }
}