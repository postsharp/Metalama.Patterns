// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Caching.Dependencies;

namespace Metalama.Patterns.Caching
{
    [Serializable]
    internal sealed class SuspendedCachingContext : MarshalByRefObject, IDisposable, ICachingContext
    {
        private ICachingContext suspendedContext;

        internal SuspendedCachingContext(ICachingContext suspendedContext)
        {
            this.suspendedContext = suspendedContext;
        }

        public ICachingContext Parent => null;

        public void AddDependencies( IEnumerable<string> dependencies )
        {
        }

        public void AddDependencies( IEnumerable<ICacheDependency> dependencies )
        {
        }

        public void AddDependency( string dependency )
        {
        }

        public void AddDependency( object dependency )
        {
        }

        public void AddDependency( ICacheDependency dependency )
        {
        }

        public CachingContextKind Kind => CachingContextKind.None;

        public void Dispose()
        {
            if (CachingContext.Current != this)
                throw new InvalidOperationException("Only the current context can be disposed.");
            
            CachingContext.Current = this.suspendedContext;
        }
    }
}
