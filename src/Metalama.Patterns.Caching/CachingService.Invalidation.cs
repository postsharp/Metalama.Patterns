// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Metalama.Patterns.Caching.Dependencies;
using Metalama.Patterns.Caching.Implementation;
using System.Collections.Concurrent;
using System.Reflection;
using static Flashtrace.Messages.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Invalidates the cache.
/// </summary>
public partial class CachingService
{
    private readonly LogSource _defaultLogger;

    private readonly ConcurrentDictionary<MethodInfo, int> _nestedCachedMethods = new();

    internal void AddedNestedCachedMethod( MethodInfo method ) => this._nestedCachedMethods.TryAdd( method, 0 );

    /// <summary>
    /// Invalidates a cache dependency given an <see cref="object"/>, i.e. removes all cached items that are dependent on that object.
    /// </summary>
    /// <param name="dependency">Typically, an <see cref="object"/>. If a <see cref="string"/>, <see cref="Delegate"/> or <see cref="ICacheDependency"/>
    /// is passed, the proper overload of the method is invoked. Otherwise, <paramref name="dependency"/> is wrapped into an <see cref="ObjectDependency"/> object.</param>
    public void Invalidate( object dependency )
    {
        switch ( dependency )
        {
            case Delegate method:
                this.InvalidateDelegate( method );

                break;

            case string key:
                this.Invalidate( key );

                break;

            case ICacheDependency cacheDependency:
                this.Invalidate( cacheDependency );

                break;

            default:
                this.Invalidate( new ObjectDependency( dependency, this ), dependency.GetType() );

                break;
        }
    }

    /// <summary>
    /// Asynchronously invalidates a cache dependency given an <see cref="object"/>, i.e. removes all cached items that are dependent on that object.
    /// </summary>
    /// <param name="dependency">Typically, an <see cref="object"/>. If a <see cref="string"/>, <see cref="Delegate"/> or <see cref="ICacheDependency"/>
    /// is passed, the proper overload of the method is invoked. Otherwise, <paramref name="dependency"/> is wrapped into an <see cref="ObjectDependency"/> object.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public ValueTask InvalidateAsync( object dependency )
    {
        switch ( dependency )
        {
            case Delegate method:
                return this.InvalidateDelegateAsync( method );

            case string key:
                return this.InvalidateAsync( key );

            case ICacheDependency cacheDependency:
                return this.InvalidateAsync( cacheDependency );

            default:
                return this.InvalidateAsync( new ObjectDependency( dependency, this ), dependency.GetType() );
        }
    }

    /// <summary>
    /// Invalidates a cache dependency given as an <see cref="ICacheDependency"/>, i.e. removes all cache items that are dependent on this dependency.
    /// </summary>
    /// <param name="dependency">A dependency.</param>
    public void Invalidate( ICacheDependency dependency ) => this.Invalidate( dependency, dependency.GetType() );

    private void Invalidate( ICacheDependency dependency, Type dependencyType )
    {
        using ( var activity =
               this._defaultLogger.Default.OpenActivity( Formatted( "Invalidating object dependency of type {DependencyType}", dependencyType ) ) )
        {
            try
            {
                this.Invalidate( dependency.GetCacheKey() );

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
    public ValueTask InvalidateAsync( ICacheDependency dependency ) => this.InvalidateAsync( dependency, dependency.GetType() );

    private async ValueTask InvalidateAsync( ICacheDependency dependency, Type dependencyType )
    {
        using ( var activity =
               this._defaultLogger.Default.OpenAsyncActivity( Formatted( "Invalidating object dependency of type {DependencyType}", dependencyType ) ) )
        {
            try
            {
                await this.InvalidateAsync( dependency.GetCacheKey() );

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
    public void Invalidate( string dependencyKey )
    {
        foreach ( var backend in this.AllBackends )
        {
            using ( var activity =
                   this._defaultLogger.Default.OpenActivity( Formatted( "Invalidate( key = {Key}, backend = {Backend} )", dependencyKey, backend ) ) )
            {
                try
                {
                    this.InvalidateImpl( backend, dependencyKey );

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

    private void InvalidateImpl( CachingBackend backend, string dependencyKey )
    {
        this._defaultLogger.Debug.IfEnabled?.Write( Formatted( "The dependency key is {Key}.", dependencyKey ) );

        backend.InvalidateDependency( dependencyKey );
    }

    /// <summary>
    /// Asynchronously invalidates a cache dependency given as <see cref="string"/>, i.e. removes all cache items that are dependent on this dependency key.
    /// </summary>
    /// <param name="dependencyKey"></param>
    public async ValueTask InvalidateAsync( string dependencyKey )
    {
        foreach ( var backend in this.AllBackends )
        {
            using ( var activity =
                   this._defaultLogger.Default.OpenAsyncActivity( Formatted( "InvalidateAsync( key = {Key}, backend = {Backend} )", dependencyKey, backend ) ) )
            {
                try
                {
                    await backend.InvalidateDependencyAsync( dependencyKey );

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
    /// <param name="method">The <see cref="MethodInfo"/> of the method call.</param>
    /// <param name="instance">The value of the <c>this</c> instance, or <c>null</c> for  methods.</param>
    /// <param name="args">The method arguments.</param>
    public void Invalidate( MethodInfo method, object? instance, params object?[] args )
    {
        using ( var activity = this._defaultLogger.Default.OpenActivity( Formatted( "Invalidate( method = {Method} )", method ) ) )
        {
            try
            {
                var key = this.KeyBuilder.BuildMethodKey(
                    CachedMethodMetadataRegistry.Instance.Get( method )
                    ?? throw new CachingAssertionFailedException( $"The method '{method}' is not registered." ),
                    args,
                    instance );

                this._defaultLogger.Debug.IfEnabled?.Write( Formatted( "Key=\"{Key}\".", key ) );

                foreach ( var backend in this.AllBackends )
                {
                    backend.RemoveItem( key );

                    if ( backend.SupportedFeatures.Dependencies )
                    {
                        backend.InvalidateDependency( key );
                    }
                    else if ( this._nestedCachedMethods.ContainsKey( method ) )
                    {
                        this._defaultLogger.Warning.IfEnabled?.Write(
                            Formatted(
                                "Method {Method} is being invalidated from the cache, but other cached methods depend on it. " +
                                "These dependent methods will not be invalidated because the current back-end does not support dependencies.",
                                method ) );
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
    /// <param name="method">The <see cref="MethodInfo"/> of the method call.</param>
    /// <param name="instance">The value of the <c>this</c> instance, or <c>null</c> for  methods.</param>
    /// <param name="args">The method arguments.</param>
    public async ValueTask InvalidateAsync( MethodInfo method, object? instance, params object[] args )
    {
        using ( var activity = this._defaultLogger.Default.OpenAsyncActivity( Formatted( "InvalidateAsync( method = {Method} )", method ) ) )
        {
            try
            {
                var key = this.KeyBuilder.BuildMethodKey(
                    CachedMethodMetadataRegistry.Instance.Get( method )
                    ?? throw new CachingAssertionFailedException( $"The method '{method}' is not registered." ),
                    args,
                    instance );

                this._defaultLogger.Debug.IfEnabled?.Write( Formatted( "Key=\"{Key}\".", key ) );

                foreach ( var backend in this.AllBackends )
                {
                    await backend.RemoveItemAsync( key );

                    if ( backend.SupportedFeatures.Dependencies )
                    {
                        await backend.InvalidateDependencyAsync( key );
                    }
                    else if ( this._nestedCachedMethods.ContainsKey( method ) )
                    {
                        this._defaultLogger.Warning.IfEnabled?.Write(
                            Formatted(
                                "Method {Method} is being invalidated from the cache, but other cached methods depend on it. " +
                                "These dependent methods will not be invalidated because the current back-end does not support dependencies.",
                                method ) );
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

    private void InvalidateDelegate( Delegate method, params object[] args ) => this.Invalidate( method.Method, method.Target, args );

    private ValueTask InvalidateDelegateAsync( Delegate method, params object[] args ) => this.InvalidateAsync( method.Method, method.Target, args );

    /// <summary>
    /// Removes a method call result from the cache giving the delegate of the method. This overload is for methods with 0 parameter.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method.</typeparam>
    /// <param name="method">A delegate of the method to invalidate.</param>
    public void Invalidate<TReturn>( Func<TReturn> method ) => this.InvalidateDelegate( method );

    /// <summary>
    /// Asynchronously removes a method call result from the cache giving the delegate of the method. This overload is for methods with 0 parameter.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method.</typeparam>
    /// <param name="method">A delegate of the method to invalidate.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public ValueTask InvalidateAsync<TReturn>( Func<TReturn> method ) => this.InvalidateDelegateAsync( method );

    private CachingContext OpenRecacheContext( Delegate method, params object[] args )
    {
        var key = this.KeyBuilder.BuildMethodKey(
            CachedMethodMetadataRegistry.Instance.Get( method.Method )
            ?? throw new CachingAssertionFailedException( $"The method '{method.Method}' is not registered." ),
            args,
            method.Target );

        return CachingContext.OpenRecacheContext( key, this );
    }

    /// <summary>
    /// Evaluates a method, ignoring the currently cached value, and replaces the corresponding cache item with the new return value of the method. This overload is for methods with 0 parameter.
    /// </summary>
    /// <typeparam name="TReturn">The return type of the method.</typeparam>
    /// <param name="method">A delegate of the method to evaluate.</param>
    /// <returns>The return value of <paramref name="method"/>.</returns>
    public TReturn Recache<TReturn>( Func<TReturn> method )
    {
        using ( var activity = this._defaultLogger.Default.OpenActivity( Formatted( "Recache( method = {Method} )", method.Method ) ) )
        {
            try
            {
                TReturn result;

                using ( this.OpenRecacheContext( method ) )
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
    public async Task<TReturn> RecacheAsync<TReturn>( Func<Task<TReturn>> method )
    {
        using ( var activity = this._defaultLogger.Default.OpenAsyncActivity( Formatted( "RecacheAsync( method = {Method} )", method.Method ) ) )
        {
            try
            {
                TReturn result;

                using ( this.OpenRecacheContext( method ) )
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