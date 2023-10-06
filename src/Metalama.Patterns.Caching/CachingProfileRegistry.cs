// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Allows to configure caching profiles (<see cref="CachingProfile"/>), and therefore influence the behavior
/// the caching of methods that are bound to this profile. Exposed on the <see cref="CachingService.Profiles"/> property.
/// </summary>
[PublicAPI]
public sealed class CachingProfileRegistry
{
    private readonly ImmutableDictionary<string, CachingProfile> _profiles;

    internal CachingProfileRegistry( ImmutableDictionary<string, CachingProfile> profiles )
    {
        this._profiles = profiles;
        this.AllBackends = profiles.Select( x => x.Value.Backend ).ToImmutableHashSet();
    }

    internal ImmutableHashSet<CachingBackend> AllBackends { get; }

    /// <summary>
    /// Gets the default <see cref="CachingProfile"/>.
    /// </summary>
    public CachingProfile Default => this[CachingProfile.DefaultName];

    /// <summary>
    /// Gets a <see cref="CachingProfile"/> of a given name. If no profile exists, a new profile is created, registered and returned.
    /// </summary>
    /// <param name="profileName">The profile name (a case-insensitive string).</param>
    /// <returns>A <see cref="CachingProfile"/> object with name <paramref name="profileName"/>.</returns>
    public CachingProfile this[ string? profileName ]
    {
        get
        {
            profileName ??= CachingProfile.DefaultName;

            if ( !this._profiles.TryGetValue( profileName, out var profile ) )
            {
                throw new KeyNotFoundException( $"The caching profile '{profileName}' has not been defined." );
            }

            return profile;
        }
    }
}