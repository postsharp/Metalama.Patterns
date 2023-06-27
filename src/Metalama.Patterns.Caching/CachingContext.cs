// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Contracts;
using PostSharp.Patterns.Caching.Dependencies;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace PostSharp.Patterns.Caching
{
    // Needs to be [Serializable] and derived from MarshalByRefObject because we are using CallContext in .NET 4.5.
    // No serialization actually occurs, but CallContext requires this.
    [Serializable]
    internal sealed class CachingContext : MarshalByRefObject, IDisposable, ICachingContext
    {
        private static readonly AsyncLocal<ICachingContext> currentContext = new AsyncLocal<ICachingContext>();

        public static ICachingContext Current
        {
            get { return currentContext.Value ?? (currentContext.Value = new NullCachingContext()); }
            internal set { currentContext.Value = value; }
        }

        private readonly string key;
        private bool disposed;
        private readonly object dependenciesSync = new object();
        private HashSet<string> dependencies;

        [SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Justification = "Not really serialized.")]
        private ImmutableHashSet<string> immutableDependencies;

        private CachingContext()
        {

        }


        private CachingContext(string key, CachingContextKind options, ICachingContext parent)
        {
            this.key = key;
            this.Kind = options;
            this.Parent = parent;
        }



        public CachingContextKind Kind { get; }

        internal ImmutableHashSet<string> Dependencies
        {
            get
            {
                if ( this.immutableDependencies == null )
                {
                    lock ( this.dependenciesSync )
                    {
                        if ( this.dependencies == null )
                        {
                            this.immutableDependencies = ImmutableHashSet<string>.Empty;
                        }
                        else
                        {
                            this.immutableDependencies = this.dependencies.ToImmutableHashSet();
                        }
                    }
                }

                return this.immutableDependencies;
            }
        }

        public ICachingContext Parent { get; }

        public static CachingContext OpenRecacheContext( string key )
        {
            CachingContext context = new CachingContext( key, CachingContextKind.Recache, Current );
            Current = context;
            return context;
        }



        internal static CachingContext OpenCacheContext( string key )
        {
            CachingContext context = new CachingContext( key, CachingContextKind.Cache, Current );
            Current = context;
            return context;
        }

        internal static SuspendedCachingContext OpenSuspendedCacheContext()
        {
            SuspendedCachingContext context = new SuspendedCachingContext(Current);
            Current = context;
            return context;
        }
        
        public void AddDependency([Required] ICacheDependency dependency)
        {          
            if (string.IsNullOrEmpty( this.key ))
                throw new InvalidOperationException("This method can be invoked only in the context of a cached method.");

            if (this.disposed)
            {
                this.Parent?.AddDependency(dependency);
                return;
            }

            lock ( this.dependenciesSync )
            {
                this.PrepareAddDependency();
                this.dependencies.Add( dependency.GetCacheKey() );
            }           
        }

        private void PrepareAddDependency()
        {
            if ( this.dependencies == null )
            {
                this.dependencies = new HashSet<string>();
            }

            this.immutableDependencies = null;
        }

        public void AddDependency([Required] object dependency)
        {
            switch (dependency)
            {
                case ICacheDependency cacheDependency:
                    this.AddDependency(cacheDependency);
                    return;
             
                case string str:
                    this.AddDependency(str);
                    return;

                default:
                    this.AddDependency(new ObjectDependency(dependency));
                    return;
            }
        }

        
        public void AddDependency([Required] string dependency)
        {
            this.AddDependency(new StringDependency(dependency));

            if (this.disposed)
            {
                this.Parent?.AddDependency(dependency);
                return;
            }
        }

        public void AddDependencies(IEnumerable<ICacheDependency> dependencies)
        {
            if (string.IsNullOrEmpty(this.key))
                throw new InvalidOperationException("This method can be invoked only in the context of a cached method.");

            if (this.disposed)
            {
                this.Parent?.AddDependencies(dependencies);
                return;
            }

            if (dependencies != null)
            {
                lock (this.dependenciesSync)
                {
                    this.PrepareAddDependency();
                    foreach (ICacheDependency dependency in dependencies)
                    {
                        this.dependencies.Add( dependency.GetCacheKey() );
                    }
                }
              
            }
        }

        public void AddDependencies(IEnumerable<string> dependencies)
        {
            if (this.disposed)
            {
                this.Parent?.AddDependencies(dependencies);
                return;
            }

            if (dependencies != null)
            {
                lock (this.dependenciesSync)
                {
                    this.PrepareAddDependency();
                    foreach (string dependency in dependencies)
                    {
                        this.dependencies.Add(dependency);
                    }
                }
            }
        }

        internal void AddDependenciesToParent(MethodInfo method)
        {
            if ( this.Parent != null )
            {
                this.Parent.AddDependencies( this.Dependencies );

                if ( CachingServices.DefaultBackend.SupportedFeatures.Dependencies )
                {
                    this.Parent.AddDependency( this.key );
                }
                else
                {
                    CachingServices.Invalidation.AddedNestedCachedMethod( method );
                }
            }
        }

        public void Dispose()
        {
            if ( Current != this )
                throw new InvalidOperationException("Only the current context can be disposed.");

            this.disposed = true;

            Current = this.Parent;
        }
    }
  
  }


