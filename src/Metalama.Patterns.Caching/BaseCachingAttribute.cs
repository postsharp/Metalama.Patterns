using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

[RunTimeOrCompileTime]
public abstract class BaseCachingAttribute : Attribute, IHierarchicalOptionsProvider<CachingOptions>
{
    private CachingOptions _options = new CachingOptions();
    
    /// <summary>
    /// Gets or sets the name of the <see cref="CachingProfile"/> that contains the configuration of the cached methods.
    /// </summary>
    public string? ProfileName
    {
        get => this._options.ProfileName ?? CachingOptions.Default.ProfileName;
        set => this._options = this._options with { ProfileName = value };
    }

    /// <summary>
    /// Gets or sets a value indicating whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
    /// when the cache item is removed from the cache.
    /// </summary>
    public bool AutoReload
    {
        get => this._options.AutoReload ?? CachingOptions.Default.AutoReload!.Value;
        set => this._options = this._options with { AutoReload = value };
    }

    /// <summary>
    /// Gets or sets the total duration, in minutes, during which the result of the cached method  is stored in cache. The absolute
    /// expiration time is counted from the moment the method is evaluated and cached.
    /// </summary>
    public double AbsoluteExpiration
    {
        get => (this._options.AbsoluteExpiration ?? CachingOptions.Default.AbsoluteExpiration)?.TotalMinutes ?? 0;
        set => this._options = this._options with { AbsoluteExpiration = TimeSpan.FromMinutes( value ) };
    }

    /// <summary>
    /// Gets or sets the duration, in minutes, during which the result of the cached method is stored in cache after it has been
    /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
    /// </summary>
    public double SlidingExpiration
    {
        get => (this._options.SlidingExpiration ?? CachingOptions.Default.SlidingExpiration)?.TotalMinutes ?? 0;
        set => this._options = this._options with { SlidingExpiration = TimeSpan.FromMinutes( value ) };
    }

    /// <summary>
    /// Gets or sets the priority of the cached method.
    /// </summary>
    public CacheItemPriority Priority
    {
        get => this._options.Priority ?? CachingOptions.Default.Priority!.Value;
        set => this._options = this._options with { Priority = value };
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>this</c> instance should be a part of the cache key. The default value of this property is <c>false</c>,
    /// which means that by default the <c>this</c> instance is a part of the cache key.
    /// </summary>
    public bool IgnoreThisParameter
    {
        get => this._options.IgnoreThisParameter ?? CachingOptions.Default.IgnoreThisParameter!.Value;
        set => this._options = this._options with { IgnoreThisParameter = value };
    }

    public bool UseDependencyInjection
    {
        get => this._options.UseDependencyInjection ?? CachingOptions.Default.UseDependencyInjection!.Value;
        set => this._options = this._options with { UseDependencyInjection = value };
    }

    public CachingOptions GetOptions() => this._options;
}