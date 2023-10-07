// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using Flashtrace.Formatters.TypeExtensions;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Caching.ValueAdapters;

namespace Metalama.Patterns.Caching;

public partial class CachingService
{
    [PublicAPI]
    public sealed class Builder : IDisposable, IAsyncDisposable
    {
        private readonly Dictionary<string, CachingProfile> _profiles = new();
        private readonly List<Action<FormatterRepository.Builder>> _formattersBuildActions = new();
        private bool _isDisposed;

        internal TypeExtensionFactory<IValueAdapter> ValueAdapters { get; } = new( typeof(IValueAdapter<>), null );

        internal Builder( IServiceProvider? serviceProvider )
        {
            this.ServiceProvider = serviceProvider ?? new NullServiceProvider();

            this.AddValueAdapter( new StreamAdapter() );
            this.AddValueAdapter( typeof(IEnumerable<>), typeof(EnumerableAdapter<>) );
            this.AddValueAdapter( typeof(IEnumerator<>), typeof(EnumeratorAdapter<>) );
#if NETCOREAPP3_0_OR_GREATER
            this.AddValueAdapter( typeof(IAsyncEnumerable<>), typeof(AsyncEnumerableAdapter<>) );
            this.AddValueAdapter( typeof(IAsyncEnumerator<>), typeof(AsyncEnumeratorAdapter<>) );
#endif
        }

        private void CheckNotDisposed()
        {
            if ( this._isDisposed )
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        public IServiceProvider ServiceProvider { get; }

        public CachingBackend? Backend { get; set; }

        internal IReadOnlyCollection<CachingProfile> Profiles => this._profiles.Values;

        internal IReadOnlyList<Action<FormatterRepository.Builder>> FormattersBuildActions => this._formattersBuildActions;

        public Func<IFormatterRepository, CacheKeyBuilder>? KeyBuilderFactory { get; set; }

        public void AddProfile( CachingProfile profile )
        {
            this.CheckNotDisposed();
            this._profiles.Add( profile.Name, profile );
        }

        /// <summary>
        /// Registers an <see cref="IValueAdapter"/> instance and explicitly specifies the value type.
        /// </summary>
        /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
        /// <param name="valueAdapter">The adapter.</param>
        public void AddValueAdapter( Type valueType, IValueAdapter valueAdapter )
        {
            this.CheckNotDisposed();
            this.ValueAdapters.RegisterTypeExtension( valueType, valueAdapter );
        }

        /// <summary>
        /// Registers a generic value adapter.
        /// </summary>
        /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
        /// <param name="valueAdapterType">The type of the value adapter. This type must implement the <see cref="IValueAdapter"/>
        /// interface and have the same number of generic parameters as <paramref name="valueType"/>.
        /// </param>
        public void AddValueAdapter( Type valueType, Type valueAdapterType )
        {
            this.CheckNotDisposed();
            this.ValueAdapters.RegisterTypeExtension( valueType, valueAdapterType );
        }

        /// <summary>
        /// Registers an <see cref="IValueAdapter{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the cached value (typically the return type of the cached method).</typeparam>
        /// <param name="valueAdapter">The adapter.</param>
        public void AddValueAdapter<T>( IValueAdapter<T> valueAdapter )
        {
            this.CheckNotDisposed();
            this.AddValueAdapter( typeof(T), valueAdapter );
        }

        public void ConfigureFormatters( Action<FormatterRepository.Builder> action )
        {
            this.CheckNotDisposed();
            this._formattersBuildActions.Add( action );
        }

        private class NullServiceProvider : IServiceProvider
        {
            public object? GetService( Type serviceType ) => null;
        }

        public void Dispose()
        {
            this._isDisposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if ( this.Backend != null )
            {
                await ((IAsyncDisposable) this.Backend).DisposeAsync();
            }
        }
    }
}