// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Dependencies;
using Metalama.Patterns.Contracts;
using System.Collections.Concurrent;
using System.Reflection;
using static Flashtrace.FormattedMessageBuilder;

#pragma warning disable CA1034  // Nested types should not be visible
#pragma warning disable IDE0008 // Use explicit type (we use var for logging)

namespace Metalama.Patterns.Caching;

public static partial class CachingServices
{
    /// <summary>
    /// Invalidates the cache.
    /// </summary>
    [PublicAPI]
    public static partial class Invalidation
    {
        private static readonly ConcurrentDictionary<MethodInfo, int> _nestedCachedMethods = new();

        internal static void AddedNestedCachedMethod( MethodInfo method ) => _nestedCachedMethods.TryAdd( method, 0 );

        /// <summary>
        /// Invalidates a cache dependency given an <see cref="object"/>, i.e. removes all cached items that are dependent on that object.
        /// </summary>
        /// <param name="dependency">Typically, an <see cref="object"/>. If a <see cref="string"/>, <see cref="Delegate"/> or <see cref="ICacheDependency"/>
        /// is passed, the proper overload of the method is invoked. Otherwise, <paramref name="dependency"/> is wrapped into an <see cref="ObjectDependency"/> object.</param>
        public static void Invalidate( [Required] object dependency )
        {
            switch ( dependency )
            {
                case Delegate method:
                    InvalidateDelegate( method );

                    break;

                case string key:
                    Invalidate( key );

                    break;

                case ICacheDependency cacheDependency:
                    Invalidate( cacheDependency );

                    break;

                default:
                    Invalidate( new ObjectDependency( dependency ), dependency.GetType() );

                    break;
            }
        }

        /// <summary>
        /// Asynchronously invalidates a cache dependency given an <see cref="object"/>, i.e. removes all cached items that are dependent on that object.
        /// </summary>
        /// <param name="dependency">Typically, an <see cref="object"/>. If a <see cref="string"/>, <see cref="Delegate"/> or <see cref="ICacheDependency"/>
        /// is passed, the proper overload of the method is invoked. Otherwise, <paramref name="dependency"/> is wrapped into an <see cref="ObjectDependency"/> object.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task InvalidateAsync( [Required] object dependency )
        {
            switch ( dependency )
            {
                case Delegate method:
                    return InvalidateDelegateAsync( method );

                case string key:
                    return InvalidateAsync( key );

                case ICacheDependency cacheDependency:
                    return InvalidateAsync( cacheDependency );

                default:
                    return InvalidateAsync( new ObjectDependency( dependency ), dependency.GetType() );
            }
        }

        /// <summary>
        /// Invalidates a cache dependency given as an <see cref="ICacheDependency"/>, i.e. removes all cache items that are dependent on this dependency.
        /// </summary>
        /// <param name="dependency">A dependency.</param>
        public static void Invalidate( [Required] ICacheDependency dependency ) => Invalidate( dependency, dependency.GetType() );

        private static void Invalidate( [Required] ICacheDependency dependency, Type dependencyType )
        {
            using ( var activity =
                   _defaultLogger.Default.OpenActivity( Formatted( "Invalidating object dependency of type {DependencyType}", dependencyType ) ) )
            {
                try
                {
                    Invalidate( dependency.GetCacheKey() );

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously invalidates a cache dependency given as an <see cref="ICacheDependency"/>, i.e. removes all cache items that are dependent on this dependency.
        /// </summary>
        /// <param name="dependency">A dependency.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task InvalidateAsync( [Required] ICacheDependency dependency ) => InvalidateAsync( dependency, dependency.GetType() );

        private static async Task InvalidateAsync( [Required] ICacheDependency dependency, Type dependencyType )
        {
            using ( var activity =
                   _defaultLogger.Default.OpenActivity( Formatted( "Invalidating object dependency of type {DependencyType}", dependencyType ) ) )
            {
                try
                {
                    await InvalidateAsync( dependency.GetCacheKey() );

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }

        /// <summary>
        /// Invalidates a cache dependency given as <see cref="string"/>, i.e. removes all cache items that are dependent on this dependency key.
        /// </summary>
        /// <param name="dependencyKey"></param>
        public static void Invalidate( [Required] string dependencyKey )
        {
            using ( var activity = _defaultLogger.Default.OpenActivity( Formatted( "Invalidating string dependency" ) ) )
            {
                try
                {
                    InvalidateImpl( dependencyKey );

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }

        private static void InvalidateImpl( [Required] string dependencyKey )
        {
            _defaultLogger.Debug.EnabledOrNull?.Write( Formatted( "The dependency key is {Key}.", dependencyKey ) );

            DefaultBackend.InvalidateDependency( dependencyKey );
        }

        /// <summary>
        /// Asynchronously invalidates a cache dependency given as <see cref="string"/>, i.e. removes all cache items that are dependent on this dependency key.
        /// </summary>
        /// <param name="dependencyKey"></param>
        public static async Task InvalidateAsync( [Required] string dependencyKey )
        {
            using ( var activity = _defaultLogger.Default.OpenActivity( Formatted( "InvalidateAsync( key = {Key} )", dependencyKey ) ) )
            {
                try
                {
                    _defaultLogger.Debug.EnabledOrNull?.Write( Formatted( "The dependency key is {Key}.", dependencyKey ) );

                    await DefaultBackend.InvalidateDependencyAsync( dependencyKey );

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }

        /// <summary>
        /// Removes a method call result from the cache giving the <see cref="MethodInfo"/> representing the method, the instance and the arguments of the method call.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> of the method call.</param>
        /// <param name="instance">The value of the <c>this</c> instance, or <c>null</c> for static methods.</param>
        /// <param name="args">The method arguments.</param>
        public static void Invalidate( [Required] MethodInfo method, object? instance, params object?[] args )
        {
            using ( var activity = _defaultLogger.Default.OpenActivity( Formatted( "Invalidate( method = {Method} )", method ) ) )
            {
                try
                {
                    var key = DefaultKeyBuilder.BuildMethodKey( method, args, instance );

                    _defaultLogger.Debug.EnabledOrNull?.Write( Formatted( "Key=\"{Key}\".", key ) );

                    DefaultBackend.RemoveItem( key );

                    if ( DefaultBackend.SupportedFeatures.Dependencies )
                    {
                        DefaultBackend.InvalidateDependency( key );
                    }
                    else if ( _nestedCachedMethods.ContainsKey( method ) )
                    {
                        _defaultLogger.Debug.EnabledOrNull?.Write(
                            Formatted(
                                "Method {Method} is being invalidated from the cache, but other cached methods depend on it. " +
                                "These dependent methods will not be invalidated because the current back-end does not support dependencies.",
                                method ) );
                    }

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously removes a method call result from the cache giving the <see cref="MethodInfo"/> representing the method, the instance and the arguments of the method call.
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> of the method call.</param>
        /// <param name="instance">The value of the <c>this</c> instance, or <c>null</c> for static methods.</param>
        /// <param name="args">The method arguments.</param>
        public static async Task InvalidateAsync( [Required] MethodInfo method, object? instance, params object[] args )
        {
            using ( var activity = _defaultLogger.Default.OpenActivity( Formatted( "InvalidateAsync( method = {Method} )", method ) ) )
            {
                try
                {
                    var key = DefaultKeyBuilder.BuildMethodKey( method, args, instance );

                    _defaultLogger.Debug.EnabledOrNull?.Write( Formatted( "Key=\"{Key}\".", key ) );

                    await DefaultBackend.RemoveItemAsync( key );

                    if ( DefaultBackend.SupportedFeatures.Dependencies )
                    {
                        await DefaultBackend.InvalidateDependencyAsync( key );
                    }
                    else if ( _nestedCachedMethods.ContainsKey( method ) )
                    {
                        _defaultLogger.Warning.Write(
                            Formatted(
                                "Method {Method} is being invalidated from the cache, but other cached methods depend on it. " +
                                "These dependent methods will not be invalidated because the current back-end does not support dependencies.",
                                method ) );
                    }

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }

        private static void InvalidateDelegate( Delegate method, params object[] args ) => Invalidate( method.Method, method.Target, args );

        private static Task InvalidateDelegateAsync( Delegate method, params object[] args ) => InvalidateAsync( method.Method, method.Target, args );

        /// <summary>
        /// Removes a method call result from the cache giving the delegate of the method. This overload is for methods with 0 parameter.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method.</typeparam>
        /// <param name="method">A delegate of the method to invalidate.</param>
        public static void Invalidate<TReturn>( [Required] Func<TReturn> method ) => InvalidateDelegate( method );

        /// <summary>
        /// Asynchronously removes a method call result from the cache giving the delegate of the method. This overload is for methods with 0 parameter.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method.</typeparam>
        /// <param name="method">A delegate of the method to invalidate.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public static Task InvalidateAsync<TReturn>( [Required] Func<TReturn> method ) => InvalidateDelegateAsync( method );

        private static CachingContext OpenRecacheContext( Delegate method, params object[] args )
        {
            var key = DefaultKeyBuilder.BuildMethodKey( method.Method, args, method.Target );

            return CachingContext.OpenRecacheContext( key );
        }

        /// <summary>
        /// Evaluates a method, ignoring the currently cached value, and replaces the corresponding cache item with the new return value of the method. This overload is for methods with 0 parameter.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method.</typeparam>
        /// <param name="method">A delegate of the method to evaluate.</param>
        /// <returns>The return value of <paramref name="method"/>.</returns>
        public static TReturn Recache<TReturn>( [Required] Func<TReturn> method )
        {
            using ( var activity = _defaultLogger.Default.OpenActivity( Formatted( "Recache( method = {Method} )", method.Method ) ) )
            {
                try
                {
                    TReturn result;

                    using ( OpenRecacheContext( method ) )
                    {
                        result = method();
                    }

                    activity.SetSuccess();

                    return result;
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }

        /// <summary>
        /// Asynchronously evaluates a method, ignoring the currently cached value, and replaces the corresponding cache item with the new return value of the method. This overload is for methods with 0 parameter.
        /// </summary>
        /// <typeparam name="TReturn">The return type of the method.</typeparam>
        /// <param name="method">A delegate of the method to evaluate.</param>
        /// <returns>A <see cref="Task{TResult}"/> that evaluates to the return value of <paramref name="method"/>.</returns>
        public static async Task<TReturn> RecacheAsync<TReturn>( [Required] Func<Task<TReturn>> method )
        {
            using ( var activity = _defaultLogger.Default.OpenActivity( Formatted( "RecacheAsync( method = {Method} )", method.Method ) ) )
            {
                try
                {
                    TReturn result;

                    using ( OpenRecacheContext( method ) )
                    {
                        result = await method();
                    }

                    activity.SetSuccess();

                    return result;
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }
    }
}