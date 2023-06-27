// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Formatters;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Caching.ValueAdapters
{
    // TODO: [Porting] ValueAdapterFactory uses TypeExtensionFactory et al, requires them to be public in FT.Formatters.
    /// <summary>
    /// Registers and provides value adapters (<see cref="IValueAdapter"/>), which allow for instance to cache things like <see cref="System.IO.Stream"/> or <see cref="System.Collections.IEnumerable"/>,
    /// which could not be otherwise cached.
    /// </summary>
    public sealed class ValueAdapterFactory
    {
        private readonly ConcurrentDictionary<Type, TypeExtensionInfo<IValueAdapter>> valueAdaptersByValueType = new();
        private static readonly LogSource logger = LogSourceFactory.ForRole( LoggingRoles.Caching ).GetLogSource( typeof(ValueAdapterFactory) );
        private readonly TypeExtensionFactory<IValueAdapter> factory = new( typeof(IValueAdapter<>), null );

        internal ValueAdapterFactory()
        {
            this.Register( new StreamAdapter() );
            this.Register( typeof(IEnumerable<>), typeof(EnumerableAdapter<>) );
            this.Register( typeof(IEnumerator<>), typeof(EnumeratorAdapter<>) );
        }

        /// <summary>
        /// Registers an <see cref="IValueAdapter"/> instance and explicitly specifies the value type.
        /// </summary>
        /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
        /// <param name="valueAdapter">The adapter.</param>
        public void Register( Type valueType, IValueAdapter valueAdapter )
        {
            this.factory.RegisterTypeExtension( valueType, valueAdapter );
        }

        /// <summary>
        /// Registers a generic value adapter.
        /// </summary>
        /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
        /// <param name="valueAdapterType">The type of the value adapter. This type must implement the <see cref="IValueAdapter"/>
        /// interface and have the same number of generic parameters as <paramref name="valueType"/>.
        /// </param>
        public void Register( Type valueType, Type valueAdapterType )
        {
            this.factory.RegisterTypeExtension( valueType, valueAdapterType );
        }

        /// <summary>
        /// Registers an <see cref="IValueAdapter{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the cached value (typically the return type of the cached method).</typeparam>
        /// <param name="valueAdapter">The adapter.</param>
        [SuppressMessage( "Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters" )]
        public void Register<T>( IValueAdapter<T> valueAdapter )
        {
            this.Register( typeof(T), valueAdapter );
        }

        /// <summary>
        /// Gets an <see cref="IValueAdapter"/> given a value type.
        /// </summary>
        /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
        /// <returns>A value adapter for <paramref name="valueType"/>, or <c>null</c> if no value adapter is available for <paramref name="valueType"/>.</returns>
        public IValueAdapter Get( Type valueType )
        {
            return this.valueAdaptersByValueType.GetOrAdd( valueType, this.GetCore ).Extension;
        }

        private TypeExtensionInfo<IValueAdapter> GetCore( Type valueType )
        {
            return this.factory.GetTypeExtension( valueType, this.CacheUpdateCallback, () => null );
        }

        private void CacheUpdateCallback( TypeExtensionInfo<IValueAdapter> typeExtension )
        {
            this.valueAdaptersByValueType[typeExtension.ObjectType] = typeExtension;
        }
    }
}