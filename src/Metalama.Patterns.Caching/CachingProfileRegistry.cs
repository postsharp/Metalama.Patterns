// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Immutable;
using System.Threading;
using PostSharp.Patterns.Contracts;

#pragma warning disable CA1034 // Nested types should not be visible

namespace PostSharp.Patterns.Caching
{
    partial class CachingServices
    {

        /// <summary>
        /// Allows to configure caching profiles (<see cref="CachingProfile"/>), and therefore influence the behavior
        /// of the <see cref="CacheAttribute"/> aspect at run-time. Exposed on the <see cref="CachingServices.Profiles"/> property.
        /// </summary>
        public sealed class CachingProfileRegistry
        {
            private volatile ImmutableDictionary<string, CachingProfile> profiles = ImmutableDictionary.Create<string, CachingProfile>(StringComparer.OrdinalIgnoreCase);
            private int revisionNumber;


            internal CachingProfileRegistry()
            {
                this.Reset();
            }

            /// <summary>
            /// Gets the revision number of all caching profiles. This property is incremented every time
            /// a profile is registered or modified.
            /// </summary>
            public int RevisionNumber => this.revisionNumber;


            internal void OnProfileChanged() => Interlocked.Increment( ref this.revisionNumber );

            /// <summary>
            /// Gets the default <see cref="CachingProfile"/>.
            /// </summary>
            public CachingProfile Default => this[CachingProfile.DefaultName];

            /// <summary>
            /// Gets a <see cref="CachingProfile"/> of a given name. If no profile exists, a new profile is created, registered and returned.
            /// </summary>
            /// <param name="profileName">The profile name (a case-insensitive string).</param>
            /// <returns>A <see cref="CachingProfile"/> object with name <paramref name="profileName"/>.</returns>
            public CachingProfile this[ [Required] string profileName ]
            {
                get
                {
                    CachingProfile profile;

                    ImmutableDictionary<string, CachingProfile> oldDictionary;
                    ImmutableDictionary<string, CachingProfile> newDictionary;

                    do
                    {
                        oldDictionary = this.profiles;

                        if ( oldDictionary.TryGetValue( profileName ?? CachingProfile.DefaultName, out profile ) )
                        {
                            return profile;
                        }

                        profile = new CachingProfile( profileName );
                        newDictionary = oldDictionary.SetItem( profile.Name, profile );

                    }
#pragma warning disable 420
                    while ( Interlocked.CompareExchange( ref this.profiles, newDictionary, oldDictionary ) != oldDictionary );
#pragma warning restore 420

                    this.OnProfileChanged();

                    return profile;
                }

            }

            /// <summary>
            /// Registers a <see cref="CachingProfile"/>.
            /// </summary>
            /// <param name="profile">A <see cref="CachingProfile"/>.</param>
            public void Register( [Required] CachingProfile profile )
            {
                ImmutableDictionary<string, CachingProfile> oldDictionary;
                ImmutableDictionary<string, CachingProfile> newDictionary;

                do
                {
                    oldDictionary = this.profiles;
                    newDictionary = oldDictionary.SetItem( profile.Name, profile );

                }
#pragma warning disable 420
                while ( Interlocked.CompareExchange( ref this.profiles, newDictionary, oldDictionary ) != oldDictionary );
#pragma warning restore 420

                this.OnProfileChanged();

            }

            /// <summary>
            /// Resets the current <see cref="CachingProfileRegistry"/> to the default values.
            /// </summary>
            public void Reset()
            {
                this.profiles =  ImmutableDictionary.Create<string, CachingProfile>(StringComparer.OrdinalIgnoreCase);
                this.Register( new CachingProfile( CachingProfile.DefaultName ) );
                this.OnProfileChanged();
            }


        }
    }
}
