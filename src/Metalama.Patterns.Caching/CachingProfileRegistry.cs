// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Allows to configure caching profiles (<see cref="CachingProfile"/>), and therefore influence the behavior
/// of the <see cref="CacheAttribute"/> aspect at run-time. Exposed on the <see cref="CachingService.Profiles"/> property.
/// </summary>
[PublicAPI]
public sealed class CachingProfileRegistry
{
    private readonly CachingService _cachingService;

    private volatile ImmutableDictionary<string, CachingProfile> _profiles =
        ImmutableDictionary.Create<string, CachingProfile>( StringComparer.OrdinalIgnoreCase );

    private int _revisionNumber;

    public CachingProfileRegistry( CachingService cachingService )
    {
        this._cachingService = cachingService;
        this.Reset();
    }

    internal ImmutableHashSet<CachingBackend> AllBackends { get; private set; } = ImmutableHashSet<CachingBackend>.Empty;

    /// <summary>
    /// Gets the revision number of all caching profiles. This property is incremented every time
    /// a profile is registered or modified.
    /// </summary>
    public int RevisionNumber => this._revisionNumber;

    internal void OnChange()
    {
        Interlocked.Increment( ref this._revisionNumber );
        this.AllBackends = this._profiles.Values.Select( p => p.Backend ).Where( b => b is not UninitializedCachingBackend ).Distinct().ToImmutableHashSet();
    }

    private void OnProfileChanged( object? sender, PropertyChangedEventArgs args ) => this.OnChange();

    /// <summary>
    /// Gets the default <see cref="CachingProfile"/>.
    /// </summary>
    public CachingProfile Default => this[CachingProfile.DefaultName];

    /// <summary>
    /// Gets a <see cref="CachingProfile"/> of a given name. If no profile exists, a new profile is created, registered and returned.
    /// </summary>
    /// <param name="profileName">The profile name (a case-insensitive string).</param>
    /// <returns>A <see cref="CachingProfile"/> object with name <paramref name="profileName"/>.</returns>
    public CachingProfile this[ string profileName ]
    {
        get
        {
            CachingProfile? profile;

            ImmutableDictionary<string, CachingProfile> oldDictionary;
            ImmutableDictionary<string, CachingProfile> newDictionary;

            do
            {
                oldDictionary = this._profiles;

                if ( oldDictionary.TryGetValue( profileName, out profile ) )
                {
                    return profile;
                }

                profile = new CachingProfile( profileName, this._cachingService );
                newDictionary = oldDictionary.SetItem( profile.Name, profile );
            }
            while ( Interlocked.CompareExchange( ref this._profiles, newDictionary, oldDictionary ) != oldDictionary );

            this.OnChange();

            return profile;
        }
    }

    /// <summary>
    /// Resets the current <see cref="CachingProfileRegistry"/> to the default values.
    /// </summary>
    public void Reset()
    {
        this._profiles = ImmutableDictionary.Create<string, CachingProfile>( StringComparer.OrdinalIgnoreCase );

        // Force the creation of the default backend. 
        _ = this.Default;
    }
}