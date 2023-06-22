// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using JetBrains.Annotations;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Flashtrace;

/// <summary>
/// Extension methods for the <see cref="ILoggerFactory"/> interface.
/// </summary>
[PublicAPI]
public static class LoggerFactoryExtensions
{
    private static readonly ConcurrentDictionary<CacheKey, LogSource> _cache = new();

    /// <summary>
    /// Gets a <see cref="LogSource"/> for a given role and <see cref="Type"/>.
    /// </summary>
    /// <param name="factory">An <see cref="ILoggerFactory"/>.</param>
    /// <param name="type">The type that will emit the records.</param>
    /// <returns>A <see cref="LogSource"/> for <paramref name="type"/>.</returns>
    public static LogSource GetLogSource( this ILoggerFactory factory, Type type )
    {
        if ( factory == null )
        {
            throw new ArgumentNullException( nameof(factory) );
        }

        if ( type == null )
        {
            throw new ArgumentNullException( nameof(type) );
        }

        var fullName = type.FullName;

        if ( fullName == null )
        {
            // TODO: Review terminology 'normal'

            throw new ArgumentException(
                $"Must be a 'normal' type. For example, array types, generic type parameters and open generic types are not supported. {nameof(type)}.FullName must not return null." );
        }

        var cacheKey = new CacheKey( factory, fullName );

        if ( _cache.TryGetValue( cacheKey, out var result ) )
        {
            return result;
        }
        else
        {
            var newSource = new LogSource( factory.GetLogger( type ) );
            _cache.TryAdd( cacheKey, newSource );

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

        if ( _cache.TryGetValue( cacheKey, out var result ) )
        {
            return result;
        }
        else
        {
            var newSource = new LogSource( factory.GetLogger( sourceName ) );
            _cache.TryAdd( cacheKey, newSource );

            return newSource;
        }
    }

    /// <summary>
    /// Gets a <see cref="LogSource"/> for a given role and for the calling type.
    /// </summary>
    /// <param name="factory">An <see cref="ILoggerFactory"/>.</param>
    /// <returns>A <see cref="LogSource"/> for the calling type.</returns>
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
        private readonly object _factory;
        private readonly string _name;

        public CacheKey( object factory, string name )
        {
            this._factory = factory;
            this._name = name;
        }

        public override bool Equals( object? obj ) => obj is CacheKey entry && this.Equals( entry );

        public bool Equals( CacheKey other )
            => EqualityComparer<object>.Default.Equals( this._factory, other._factory ) &&
               this._name == other._name;

        public override int GetHashCode()
        {
            var hashCode = 163068155;
            hashCode = (hashCode * -1521134295) + EqualityComparer<object>.Default.GetHashCode( this._factory );
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode( this._name );

            return hashCode;
        }
    }
}