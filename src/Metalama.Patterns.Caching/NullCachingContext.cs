// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Dependencies;

namespace Metalama.Patterns.Caching;

[Serializable]
internal sealed class NullCachingContext : MarshalByRefObject, ICachingContext
{
    public ICachingContext? Parent => null;

    public void AddDependencies( IEnumerable<string>? dependencies ) { }

    public void AddDependencies( IEnumerable<ICacheDependency>? dependencies ) { }

    public void AddDependency( string dependency ) { }

    public void AddDependency( object dependency ) { }

    public void AddDependency( ICacheDependency dependency ) { }

    public CachingContextKind Kind => CachingContextKind.None;
}