// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Flashtrace
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> interface.
    /// </summary>
    public static class LoggerFactoryExtensions
    {
        private static readonly ConcurrentDictionary<CacheKey, LogSource> cache = new();

        /// <summary>
        /// Gets a <see cref="LogSource"/> for a given role and <see cref="Type"/>.
        /// </summary>
        /// <param name="factory">An <see cref="ILoggerFactory"/>.</param>
        /// <param name="type">The type that will emit the records.</param>
        /// <returns>A <see cref="LogSource"/> for <paramref name="type"/>.</returns>
        public static LogSource GetLogSource( [Required] this ILoggerFactory factory, [Required] Type type )
        {
            var cacheKey = new CacheKey( factory, type.FullName );

            if ( cache.TryGetValue( cacheKey, out var result ) )
            {
                return result;
            }
            else
            {
                var newSource = new LogSource( ((ILoggerFactory) factory).GetLogger( type ) );
                cache.TryAdd( cacheKey, newSource );

                return newSource;
            }
        }

        /// <summary>
        /// Gets a <see cref="LogSource"/> for a given role and <paramref name="sourceName"/>.
        /// </summary>
        /// <param name="factory">An <see cref="ILoggerFactory"/>.</param>
        /// <param name="sourceName">Log source name to be used by the backend. Not all backends support creating sources by name.</param>
        /// <returns>A <see cref="LogSource"/> for <paramref name="sourceName"/>.</returns>
        public static LogSource GetLogSource( [Required] this ILoggerFactory factory, [Required] string sourceName )
        {
            var cacheKey = new CacheKey( factory, sourceName );

            if ( cache.TryGetValue( cacheKey, out var result ) )
            {
                return result;
            }
            else
            {
                var newSource = new LogSource( factory.GetLogger( sourceName ) );
                cache.TryAdd( cacheKey, newSource );

                return newSource;
            }
        }

        /// <summary>
        /// Gets a <see cref="LogSource"/> for a given role and for the calling type.
        /// </summary>
        /// <param name="factory">An <see cref="ILoggerFactory"/>.</param>
        /// <returns>A <see cref="LogSource"/> for the calling type.</returns>
        [SuppressMessage( "Microsoft.Performance", "CA1801" )]
        public static LogSource GetLogSource( [Required] this ILoggerFactory factory )
        {
            var callerInfo = CallerInfo.GetDynamic( 1 );

            return factory.GetLogSource( ref callerInfo );
        }

        /// <excludeOverload />
        [EditorBrowsable( EditorBrowsableState.Never )]
        public static LogSource GetLogSource( [Required] this ILoggerFactory factory, ref CallerInfo callerInfo )
        {
            // If we don't have a caller type, it's preferable to use System.Object as a safe fallback rather than throwing an exception.
            var callerType = callerInfo.SourceType ?? typeof(object);

            return factory.GetLogSource( callerType );
        }

        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            private readonly object factory;
            private readonly string name;

            public CacheKey( object factory, string name )
            {
                this.factory = factory;
                this.name = name;
            }

            public override bool Equals( object obj )
            {
                return obj is CacheKey entry && this.Equals( entry );
            }

            public bool Equals( CacheKey other )
            {
                return EqualityComparer<object>.Default.Equals( this.factory, other.factory ) &&
                       this.name == other.name;
            }

            public override int GetHashCode()
            {
                var hashCode = 163068155;
                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode( this.factory );
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode( this.name );

                return hashCode;
            }
        }
    }
}