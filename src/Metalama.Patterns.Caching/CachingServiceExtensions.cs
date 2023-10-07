// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Patterns.Caching.Dependencies;
using Metalama.Patterns.Caching.Implementation;
using System.Reflection;
using static Flashtrace.Messages.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Extension methods for the <see cref="ICachingService"/> interface.
/// </summary>
[PublicAPI]
public static partial class CachingServiceExtensions
{
    public static void AddDependency( this ICachingService cachingService, string key ) => CachingContext.Current.AddDependency( key );

    public static void AddDependencies( this ICachingService cachingService, IEnumerable<string> keys ) => CachingContext.Current.AddDependencies( keys );

    /// <summary>
    /// Temporarily suspends propagation of dependencies from subsequently called methods to the caller method.
    /// </summary>
    /// <returns><see cref="IDisposable"/> representation of the suspension. Disposing this object resumes the normal dependency propagation.</returns>
    /// <remarks>
    /// <para>
    /// By default, calling a cached method while another caching is active automatically adds the former as a dependency of the later. 
    /// Since the current context is stored in an <see cref="System.Threading.AsyncLocal{T}"/> variable, it may be inadvertently used after the method call associated with it
    /// had already ended. This can happen, for example, when method calls <see cref="System.Threading.Tasks.Task.Run(Action)"/> and does not depend on the resulting <see cref="System.Threading.Tasks.Task"/>.
    /// </para>
    /// <para>
    /// This context leak does not break correctness but may lead to unnecessary dependency invalidations. Therefore it is recommended to use this method when calling asynchronous code
    /// in the context of cached methods and not being dependent on its result.
    /// </para>
    /// </remarks>
    public static IDisposable SuspendDependencyPropagation( this ICachingService cachingService ) => CachingContext.OpenSuspendedCacheContext();

    public static void AddDependency( this ICachingService cachingService, ICacheDependency dependency )
    {
        cachingService.AddDependency( dependency.GetCacheKey() );
    }

    public static void AddDependencies( this ICachingService cachingService, IEnumerable<ICacheDependency> dependencies )
    {
        cachingService.AddDependencies( dependencies.Select( x => x.GetCacheKey() ) );
    }

    public static void AddDependency( this ICachingService cachingService, object dependency )
    {
        switch ( dependency )
        {
            case ICacheDependency cacheDependency:
                cachingService.AddDependency( cacheDependency );

                return;

            case string str:
                cachingService.AddDependency( str );

                return;

            default:
                cachingService.AddDependency( new ObjectDependency( dependency, cachingService ) );

                return;
        }
    }

    /// <summary>
    /// Invalidates a cache dependency given an <see cref="object"/>, i.e. removes all cached items that are dependent on that object.
    /// </summary>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="dependency">Typically, an <see cref="object"/>. If a <see cref="string"/>, <see cref="Delegate"/> or <see cref="ICacheDependency"/>
    /// is passed, the proper overload of the method is invoked. Otherwise, <paramref name="dependency"/> is wrapped into an <see cref="ObjectDependency"/> object.</param>
    public static void Invalidate( this ICachingService cachingService, object dependency )
    {
        switch ( dependency )
        {
            case Delegate method:
                cachingService.InvalidateDelegate( method );

                break;

            case string key:
                cachingService.Invalidate( key );

                break;

            case ICacheDependency cacheDependency:
                cachingService.Invalidate( cacheDependency );

                break;

            default:
                cachingService.Invalidate( new ObjectDependency( dependency, cachingService ), dependency.GetType() );

                break;
        }
    }

    /// <summary>
    /// Asynchronously invalidates a cache dependency given an <see cref="object"/>, i.e. removes all cached items that are dependent on that object.
    /// </summary>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="dependency">Typically, an <see cref="object"/>. If a <see cref="string"/>, <see cref="Delegate"/> or <see cref="ICacheDependency"/>
    /// is passed, the proper overload of the method is invoked. Otherwise, <paramref name="dependency"/> is wrapped into an <see cref="ObjectDependency"/> object.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/>.</returns>
    public static ValueTask InvalidateAsync( this ICachingService cachingService, object dependency, CancellationToken cancellationToken = default )
    {
        switch ( dependency )
        {
            case Delegate method:
                return cachingService.InvalidateDelegateAsync( method, Array.Empty<object?>(), cancellationToken );

            case string key:
                return cachingService.InvalidateAsync( key, cancellationToken );

            case ICacheDependency cacheDependency:
                return cachingService.InvalidateAsync( cacheDependency, cancellationToken );

            default:
                return cachingService.InvalidateAsync( new ObjectDependency( dependency, cachingService ), dependency.GetType(), cancellationToken );
        }
    }

    /// <summary>
    /// Invalidates a cache dependency given as an <see cref="ICacheDependency"/>, i.e. removes all cache items that are dependent on this dependency.
    /// </summary>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="dependency">A dependency.</param>
    public static void Invalidate( this ICachingService cachingService, ICacheDependency dependency )
        => cachingService.Invalidate( dependency, dependency.GetType() );

    private static void Invalidate( this ICachingService cachingService, ICacheDependency dependency, Type dependencyType )
    {
        using ( var activity =
               cachingService.Logger.Default.OpenActivity( Formatted( "Invalidating object dependency of type {DependencyType}", dependencyType ) ) )
        {
            try
            {
                cachingService.Invalidate( dependency.GetCacheKey() );

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
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="dependency">A dependency.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/>.</returns>
    public static ValueTask InvalidateAsync( this ICachingService cachingService, ICacheDependency dependency, CancellationToken cancellationToken = default )
        => cachingService.InvalidateAsync( dependency, dependency.GetType(), cancellationToken );

    private static async ValueTask InvalidateAsync(
        this ICachingService cachingService,
        ICacheDependency dependency,
        Type dependencyType,
        CancellationToken cancellationToken )
    {
        using ( var activity =
               cachingService.Logger.Default.OpenAsyncActivity( Formatted( "Invalidating object dependency of type {DependencyType}", dependencyType ) ) )
        {
            try
            {
                await cachingService.InvalidateAsync( dependency.GetCacheKey(), cancellationToken );

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
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="dependencyKey"></param>
    public static void Invalidate( this ICachingService cachingService, string dependencyKey )
    {
        foreach ( var backend in cachingService.AllBackends )
        {
            using ( var activity =
                   cachingService.Logger.Default.OpenActivity( Formatted( "Invalidate( key = {Key}, backend = {Backend} )", dependencyKey, backend ) ) )
            {
                try
                {
                    cachingService.InvalidateImpl( backend, dependencyKey );

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }
    }

    private static void InvalidateImpl( this ICachingService cachingService, CachingBackend backend, string dependencyKey )
    {
        cachingService.Logger.Debug.IfEnabled?.Write( Formatted( "The dependency key is {Key}.", dependencyKey ) );

        backend.InvalidateDependency( dependencyKey );
    }

    /// <summary>
    /// Asynchronously invalidates a cache dependency given as <see cref="string"/>, i.e. removes all cache items that are dependent on this dependency key.
    /// </summary>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="dependencyKey"></param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask InvalidateAsync( this ICachingService cachingService, string dependencyKey, CancellationToken cancellationToken = default )
    {
        foreach ( var backend in cachingService.AllBackends )
        {
            using ( var activity =
                   cachingService.Logger.Default.OpenAsyncActivity(
                       Formatted( "InvalidateAsync( key = {Key}, backend = {Backend} )", dependencyKey, backend ) ) )
            {
                try
                {
                    await backend.InvalidateDependencyAsync( dependencyKey, cancellationToken );

                    activity.SetSuccess();
                }
                catch ( Exception e )
                {
                    activity.SetException( e );

                    throw;
                }
            }
        }
    }

    /// <summary>
    /// Removes a method call result from the cache giving the <see cref="MethodInfo"/> representing the method, the instance and the arguments of the method call.
    /// </summary>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="method">The <see cref="MethodInfo"/> of the method call.</param>
    /// <param name="instance">The value of the <c>this</c> instance, or <c>null</c> for  methods.</param>
    /// <param name="args">The method arguments.</param>
    public static void Invalidate( this ICachingService cachingService, MethodInfo method, object? instance, params object?[] args )
    {
        using ( var activity = cachingService.Logger.Default.OpenActivity( Formatted( "Invalidate( method = {Method} )", method ) ) )
        {
            try
            {
                var key = cachingService.KeyBuilder.BuildMethodKey(
                    CachedMethodMetadataRegistry.Instance.Get( method )
                    ?? throw new CachingAssertionFailedException( $"The method '{method}' is not registered." ),
                    instance,
                    args );

                cachingService.Logger.Debug.IfEnabled?.Write( Formatted( "Key=\"{Key}\".", key ) );

                foreach ( var backend in cachingService.AllBackends )
                {
                    backend.RemoveItem( key );

                    if ( backend.SupportedFeatures.Dependencies )
                    {
                        backend.InvalidateDependency( key );
                    }
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
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="method">The <see cref="MethodInfo"/> of the method call.</param>
    /// <param name="instance">The value of the <c>this</c> instance, or <c>null</c> for  methods.</param>
    /// <param name="args">The method arguments.</param>
    /// <param name="cancellationToken"></param>
    public static async ValueTask InvalidateAsync(
        this ICachingService cachingService,
        MethodInfo method,
        object? instance,
        object?[] args,
        CancellationToken cancellationToken = default )
    {
        using ( var activity = cachingService.Logger.Default.OpenAsyncActivity( Formatted( "InvalidateAsync( method = {Method} )", method ) ) )
        {
            try
            {
                var key = cachingService.KeyBuilder.BuildMethodKey(
                    CachedMethodMetadataRegistry.Instance.Get( method )
                    ?? throw new CachingAssertionFailedException( $"The method '{method}' is not registered." ),
                    instance,
                    args );

                cachingService.Logger.Debug.IfEnabled?.Write( Formatted( "Key=\"{Key}\".", key ) );

                foreach ( var backend in cachingService.AllBackends )
                {
                    await backend.RemoveItemAsync( key, cancellationToken );

                    if ( backend.SupportedFeatures.Dependencies )
                    {
                        await backend.InvalidateDependencyAsync( key, cancellationToken );
                    }
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

    private static void InvalidateDelegate( this ICachingService cachingService, Delegate method, params object?[] args )
        => cachingService.Invalidate( method.Method, method.Target, args );

    private static ValueTask InvalidateDelegateAsync(
        this ICachingService cachingService,
        Delegate method,
        object?[] args,
        CancellationToken cancellationToken = default )
        => cachingService.InvalidateAsync( method.Method, method.Target, args, cancellationToken );

    /// <summary>
    /// Removes a method call result from the cache giving the delegate of the method. This overload is for methods with 0 parameter.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method.</typeparam>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="method">A delegate of the method to invalidate.</param>
    public static void Invalidate<TReturn>( this ICachingService cachingService, Func<TReturn> method ) => cachingService.InvalidateDelegate( method );

    /// <summary>
    /// Asynchronously removes a method call result from the cache giving the delegate of the method. This overload is for methods with 0 parameter.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method.</typeparam>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="method">A delegate of the method to invalidate.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task"/>.</returns>
    public static ValueTask InvalidateAsync<TReturn>( this ICachingService cachingService, Func<TReturn> method, CancellationToken cancellationToken = default )
        => cachingService.InvalidateDelegateAsync( method, Array.Empty<object?>(), cancellationToken );

    private static CachingContext OpenRecacheContext( this ICachingService cachingService, Delegate method, params object?[] args )
    {
        var key = cachingService.KeyBuilder.BuildMethodKey(
            CachedMethodMetadataRegistry.Instance.Get( method.Method )
            ?? throw new CachingAssertionFailedException( $"The method '{method.Method}' is not registered." ),
            method.Target,
            args );

        return CachingContext.OpenRecacheContext( key );
    }

    /// <summary>
    /// Evaluates a method, ignoring the currently cached value, and replaces the corresponding cache item with the new return value of the method. This overload is for methods with 0 parameter.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method.</typeparam>
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="method">A delegate of the method to evaluate.</param>
    /// <returns>The return value of <paramref name="method"/>.</returns>
    public static TReturn Recache<TReturn>( this ICachingService cachingService, Func<TReturn> method )
    {
        using ( var activity = cachingService.Logger.Default.OpenActivity( Formatted( "Recache( method = {Method} )", method.Method ) ) )
        {
            try
            {
                TReturn result;

                using ( cachingService.OpenRecacheContext( method ) )
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
    /// <param name="cachingService">The <see cref="ICachingService"/>.</param>
    /// <param name="method">A delegate of the method to evaluate.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Task{TResult}"/> that evaluates to the return value of <paramref name="method"/>.</returns>
    public static async Task<TReturn> RecacheAsync<TReturn>(
        this ICachingService cachingService,
        Func<Task<TReturn>> method,
        CancellationToken cancellationToken = default )
    {
        using ( var activity = cachingService.Logger.Default.OpenAsyncActivity( Formatted( "RecacheAsync( method = {Method} )", method.Method ) ) )
        {
            try
            {
                TReturn result;

                using ( cachingService.OpenRecacheContext( method ) )
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