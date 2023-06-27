// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Locking;
using Metalama.Patterns.Contracts;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Allows for centralized and run-time configuration of several instances of the <see cref="CacheAttribute"/> aspect.
/// </summary>
public sealed class CachingProfile : ICacheItemConfiguration
{
    /// <summary>
    /// The name of the default profile.
    /// </summary>
    public const string DefaultName = "default";

    private bool _isEnabled = true;
    private bool? _autoReload;
    private TimeSpan? _absoluteExpiration;
    private TimeSpan? _slidingExpiration;
    private CacheItemPriority? _priority;
    private ILockManager _lockManager = new NullLockManager();
    private IAcquireLockTimeoutStrategy _acquireLockTimeoutStrategy = new DefaultAcquireLockTimeoutStrategy();

    /// <summary>
    /// Initializes a new <see cref="CachingProfile"/>.
    /// </summary>
    /// <param name="name">Profile name (a case-insensitive string).</param>
    public CachingProfile( [Required] string name )
    {
        this.Name = name;
    }

    /// <summary>
    /// Gets the profile name  (a case-insensitive string).
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Determines whether caching is enabled for the current profile.
    /// </summary>
    public bool IsEnabled
    {
        get { return this._isEnabled; }
        set
        {
            CachingServices.Profiles.OnProfileChanged();
            this._isEnabled = value;
        }
    }

    /// <summary>
    /// Determines whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
    /// when the cache item is removed from the cache.
    /// </summary>
    public bool? AutoReload
    {
        get { return this._autoReload; }
        set
        {
            CachingServices.Profiles.OnProfileChanged();
            this._autoReload = value;
        }
    }

    /// <summary>
    /// Gets or sets the total duration during which the result of the current method is stored in cache. The absolute
    /// expiration time is counted from the moment the method is evaluated and cached.
    /// </summary>
    public TimeSpan? AbsoluteExpiration
    {
        get { return this._absoluteExpiration; }
        set
        {
            CachingServices.Profiles.OnProfileChanged();
            this._absoluteExpiration = value;
        }
    }

    /// <summary>
    /// Gets or sets the duration during which the result of the current method is stored in cache after it has been
    /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
    /// </summary>
    public TimeSpan? SlidingExpiration
    {
        get { return this._slidingExpiration; }
        set
        {
            CachingServices.Profiles.OnProfileChanged();
            this._slidingExpiration = value;
        }
    }

    /// <summary>
    /// Gets or sets the priority of the cached methods.
    /// </summary>
    public CacheItemPriority? Priority
    {
        get { return this._priority; }
        set
        {
            CachingServices.Profiles.OnProfileChanged();
            this._priority = value;
        }
    }

    /// <inheritdoc />

    // We can't modify specify IgnoreThisParameter in a profile because this setting is used at build time.
    bool? ICacheItemConfiguration.IgnoreThisParameter => null;

    /// <inheritdoc />
    bool? ICacheItemConfiguration.IsEnabled => this.IsEnabled;

    /// <inheritdoc />
    TimeSpan? ICacheItemConfiguration.AbsoluteExpiration => this.AbsoluteExpiration;

    /// <inheritdoc />
    TimeSpan? ICacheItemConfiguration.SlidingExpiration => this.SlidingExpiration;

    /// <inheritdoc />
    bool? ICacheItemConfiguration.AutoReload => this.AutoReload;

    /// <inheritdoc />
    string ICacheItemConfiguration.ProfileName => this.Name;

    /// <summary>
    /// Gets or sets the lock manager used to synchronize the execution of methods
    /// that are cached with the current profile.  The default lock manager is <see cref="NullLockManager"/>,
    /// which allows for concurrent execution of all methods. An alternative implementation is <see cref="LocalLockManager"/>,
    /// which prevents concurrent execution of the same method with the same parameters, in the current process (or AppDomain).
    /// </summary>
    public ILockManager LockManager
    {
        get { return this._lockManager; }
        set { this._lockManager = value ?? new NullLockManager(); }
    }

    /// <summary>
    /// Gets or sets the maximum time that the caching aspect will wait for the <see cref="LockManager"/> to acquire a lock.
    /// To specify an infinite waiting time, set this property to <c>TimeSpan.FromMilliseconds( -1 )</c>. The default
    /// behavior is to wait infinitely.
    /// </summary>
    public TimeSpan AcquireLockTimeout { get; set; } = TimeSpan.FromMilliseconds( -1 );

    /// <summary>
    /// Gets or sets the behavior in case that the caching aspect could not acquire a lock because of a timeout. 
    /// The default behavior is to throw a <see cref="TimeoutException"/>. You can implement your own strategy by implementing
    /// the <see cref="IAcquireLockTimeoutStrategy"/> interface. If the <see cref="IAcquireLockTimeoutStrategy.OnTimeout"/> does not return
    /// any exception, the cached method will be evaluated (even without a lock).
    /// </summary>
    public IAcquireLockTimeoutStrategy AcquireLockTimeoutStrategy
    {
        get { return this._acquireLockTimeoutStrategy; }
        set { this._acquireLockTimeoutStrategy = value ?? new DefaultAcquireLockTimeoutStrategy(); }
    }
}