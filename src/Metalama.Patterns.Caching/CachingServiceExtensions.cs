// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Dependencies;

namespace Metalama.Patterns.Caching;

public static class CachingServiceExtensions
{
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
}