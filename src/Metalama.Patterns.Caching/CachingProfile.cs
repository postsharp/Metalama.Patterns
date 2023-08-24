// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.Locking;
using Metalama.Patterns.Contracts;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Allows for centralized and run-time configuration of several instances of the <see cref="CacheAttribute"/> aspect.
/// </summary>
[PublicAPI]
public sealed class CachingProfile : ICacheItemConfiguration, INotifyPropertyChanged
{
    private readonly CachingService _cachingService;

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
    /// Initializes a new instance of the <see cref="CachingProfile"/> class.
    /// </summary>
    /// <param name="name">Profile name (a case-insensitive string).</param>
    /// <param name="cachingService"></param>
    internal CachingProfile( [Required] string name, CachingService cachingService )
    {
        this._cachingService = cachingService;
        this.Name = name;
    }

    /// <summary>
    /// Gets the profile name  (a case-insensitive string).
    /// </summary>
    public string Name { get; }

    public CachingBackend Backend => this._cachingService.DefaultBackend;

    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled for the current profile.
    /// </summary>
    public bool IsEnabled
    {
        get => this._isEnabled;
        set
        {
            this._isEnabled = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the method calls are automatically reloaded (by re-evaluating the target method with the same arguments)
    /// when the cache item is removed from the cache.
    /// </summary>
    public bool? AutoReload
    {
        get => this._autoReload;
        set
        {
            this._autoReload = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the total duration during which the result of the current method is stored in cache. The absolute
    /// expiration time is counted from the moment the method is evaluated and cached.
    /// </summary>
    public TimeSpan? AbsoluteExpiration
    {
        get => this._absoluteExpiration;
        set
        {
            this._absoluteExpiration = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the duration during which the result of the current method is stored in cache after it has been
    /// added to or accessed from the cache. The expiration is extended every time the value is accessed from the cache.
    /// </summary>
    public TimeSpan? SlidingExpiration
    {
        get => this._slidingExpiration;
        set
        {
            this._slidingExpiration = value;
            this.OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the priority of the cached methods.
    /// </summary>
    public CacheItemPriority? Priority
    {
        get => this._priority;
        set
        {
            this._priority = value;
            this.OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    bool? ICacheItemConfiguration.IsEnabled => this._isEnabled;

    // We can't modify specify IgnoreThisParameter in a profile because this setting is used at build time.
    /// <inheritdoc />
    bool? ICacheItemConfiguration.IgnoreThisParameter => null;

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
    [AllowNull]
    public ILockManager LockManager
    {
        get => this._lockManager;
        set => this._lockManager = value ?? new NullLockManager();
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
    [AllowNull]
    public IAcquireLockTimeoutStrategy AcquireLockTimeoutStrategy
    {
        get => this._acquireLockTimeoutStrategy;
        set => this._acquireLockTimeoutStrategy = value ?? new DefaultAcquireLockTimeoutStrategy();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
}