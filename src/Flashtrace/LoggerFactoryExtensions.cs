// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Diagnostics.Contexts;
using PostSharp.Patterns.Diagnostics.Custom;

namespace PostSharp.Patterns.Diagnostics
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> interface.
    /// </summary>
    public static class LoggerFactoryExtensions
    {
        private static readonly ConcurrentDictionary<CacheKey, LogSource> cache = new ConcurrentDictionary<CacheKey, LogSource>();

        /// <summary>
        /// Gets a <see cref="LogSource"/> for a given role and <see cref="Type"/>.
        /// </summary>
        /// <param name="factory">An <see cref="ILoggerFactory"/>.</param>
        /// <param name="type">The type that will emit the records.</param>
        /// <returns>A <see cref="LogSource"/> for <paramref name="type"/>.</returns>
        public static LogSource GetLogSource( [Required] this ILoggerFactory3 factory, [Required] Type type )
        {
            CacheKey cacheKey = new CacheKey( factory, type.FullName );


            if ( cache.TryGetValue( cacheKey, out LogSource result ) )
            {
                return result;
            }
            else
            {
                LogSource newSource = new LogSource( ((ILoggerFactory3) factory).GetLogger( type ) );
                cache.TryAdd( cacheKey, newSource );
                return newSource;
            }
        }

        /// <summary>
        /// Gets a <see cref="LogSource"/> for a given role and <paramref name="sourceName"/>.
        /// </summary>
        /// <param name="factory">An <see cref="ILoggerFactory3"/>.</param>
        /// <param name="sourceName">Log source name to be used by the backend. Not all backends support creating sources by name.</param>
        /// <returns>A <see cref="LogSource"/> for <paramref name="sourceName"/>.</returns>
        public static LogSource GetLogSource( [Required] this ILoggerFactory3 factory, [Required] string sourceName  )
        {
            CacheKey cacheKey = new CacheKey( factory, sourceName );

            if (cache.TryGetValue( cacheKey, out LogSource result ))
            {
                return result;
            }
            else
            {
                LogSource newSource = new LogSource( factory.GetLogger( sourceName ) );
                cache.TryAdd( cacheKey, newSource );
                return newSource;
            }
        }

        /// <summary>
        /// Gets a <see cref="Logger"/> for a given role and for the calling type.
        /// </summary>
        /// <param name="factory">An <see cref="ILoggerFactory"/>.</param>
        /// <returns>A <see cref="Logger"/> for the calling type.</returns>
        [SuppressMessage( "Microsoft.Performance", "CA1801" )]
        public static LogSource GetLogSource( [Required] this ILoggerFactory3 factory )
        {
            CallerInfo callerInfo = CallerInfo.GetDynamic( 1 );
            return factory.GetLogSource( ref callerInfo );
        }

        /// <excludeOverload />
        [EditorBrowsable( EditorBrowsableState.Never )]
        public static LogSource GetLogSource( [Required] this ILoggerFactory3 factory, ref CallerInfo callerInfo )
        {
            // If we don't have a caller type, it's preferable to use System.Object as a safe fallback rather than throwing an exception.
            Type callerType = callerInfo.SourceType ?? typeof( object );

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
                int hashCode = 163068155;
                hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode( this.factory );
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode( this.name );
                return hashCode;
            }
        }

    }




}


