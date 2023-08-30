// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Dependencies;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Contracts;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Metalama.Patterns.Caching;

internal sealed class CachingContext : IDisposable, ICachingContext
{
    private static readonly AsyncLocal<ICachingContext?> _currentContext = new();
    private readonly CachingService _cachingService;

    [AllowNull]
    public static ICachingContext Current
    {
        get => _currentContext.Value ??= new NullCachingContext();
        internal set => _currentContext.Value = value;
    }

    private readonly string _key;
    private readonly object _dependenciesSync = new();

    private bool _disposed;
    private HashSet<string>? _dependencies;
    private ImmutableHashSet<string>? _immutableDependencies;

    private CachingContext( string key, CachingContextKind options, ICachingContext? parent, CachingService cachingService )
    {
        this._key = key;
        this.Kind = options;
        this.Parent = parent;
        this._cachingService = cachingService;
    }

    public CachingContextKind Kind { get; }

    internal ImmutableHashSet<string> Dependencies
    {
        get
        {
            if ( this._immutableDependencies == null )
            {
                lock ( this._dependenciesSync )
                {
                    this._immutableDependencies =
                        this._dependencies == null
                            ? ImmutableHashSet<string>.Empty
                            : this._dependencies.ToImmutableHashSet();
                }
            }

            return this._immutableDependencies;
        }
    }

    public ICachingContext? Parent { get; }

    internal static CachingContext OpenRecacheContext( string key, CachingService cachingService )
    {
        var context = new CachingContext( key, CachingContextKind.Recache, Current, cachingService );
        Current = context;

        return context;
    }

    internal static CachingContext OpenCacheContext( string key, CachingService cachingService )
    {
        var context = new CachingContext( key, CachingContextKind.Cache, Current, cachingService );
        Current = context;

        return context;
    }

    internal static SuspendedCachingContext OpenSuspendedCacheContext()
    {
        var context = new SuspendedCachingContext( Current );
        Current = context;

        return context;
    }

    public void AddDependency( [Required] ICacheDependency dependency )
    {
        if ( string.IsNullOrEmpty( this._key ) )
        {
            throw new InvalidOperationException( "This method can be invoked only in the context of a cached method." );
        }

        if ( this._disposed )
        {
            this.Parent?.AddDependency( dependency );

            return;
        }

        lock ( this._dependenciesSync )
        {
            this.PrepareAddDependency();
            this._dependencies.Add( dependency.GetCacheKey() );
        }
    }

    [MemberNotNull( nameof(_dependencies) )]
    private void PrepareAddDependency()
    {
        this._dependencies ??= new HashSet<string>();
        this._immutableDependencies = null;
    }

    public void AddDependency( [Required] object dependency )
    {
        switch ( dependency )
        {
            case ICacheDependency cacheDependency:
                this.AddDependency( cacheDependency );

                return;

            case string str:
                this.AddDependency( str );

                return;

            default:
                this.AddDependency( new ObjectDependency( dependency, this._cachingService ) );

                return;
        }
    }

    public void AddDependency( [Required] string dependency )
    {
        this.AddDependency( new StringDependency( dependency ) );

        if ( this._disposed )
        {
            this.Parent?.AddDependency( dependency );
        }
    }

    public void AddDependencies( IEnumerable<ICacheDependency>? dependencies )
    {
        if ( string.IsNullOrEmpty( this._key ) )
        {
            throw new InvalidOperationException( "This method can be invoked only in the context of a cached method." );
        }

        if ( this._disposed )
        {
            this.Parent?.AddDependencies( dependencies );

            return;
        }

        if ( dependencies != null )
        {
            lock ( this._dependenciesSync )
            {
                this.PrepareAddDependency();

                foreach ( var dependency in dependencies )
                {
                    this._dependencies.Add( dependency.GetCacheKey() );
                }
            }
        }
    }

    public void AddDependencies( IEnumerable<string>? dependencies )
    {
        if ( this._disposed )
        {
            this.Parent?.AddDependencies( dependencies );

            return;
        }

        if ( dependencies != null )
        {
            lock ( this._dependenciesSync )
            {
                this.PrepareAddDependency();

                foreach ( var dependency in dependencies )
                {
                    this._dependencies.Add( dependency );
                }
            }
        }
    }

    internal void AddDependenciesToParent( CachingBackend backend, MethodInfo method )
    {
        if ( this.Parent != null )
        {
            this.Parent.AddDependencies( this.Dependencies );

            if ( backend.SupportedFeatures.Dependencies )
            {
                this.Parent.AddDependency( this._key );
            }
            else
            {
                this._cachingService.AddedNestedCachedMethod( method );
            }
        }
    }

    public void Dispose()
    {
        if ( Current != this )
        {
            throw new InvalidOperationException( "Only the current context can be disposed." );
        }

        this._disposed = true;

        Current = this.Parent;
    }
}