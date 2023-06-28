// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using JetBrains.Annotations;
using System.Collections.Concurrent;

namespace Metalama.Patterns.Caching.ValueAdapters;

// TODO: [Porting] ValueAdapterFactory uses TypeExtensionFactory et al, requires them to be public in FT.Formatters.
/// <summary>
/// Registers and provides value adapters (<see cref="IValueAdapter"/>), which allow for instance to cache things like <see cref="System.IO.Stream"/> or <see cref="System.Collections.IEnumerable"/>,
/// which could not be otherwise cached.
/// </summary>
[PublicAPI]
public sealed class ValueAdapterFactory
{
    private readonly ConcurrentDictionary<Type, TypeExtensionInfo<IValueAdapter>> _valueAdaptersByValueType = new();
    private readonly TypeExtensionFactory<IValueAdapter> _factory = new( typeof(IValueAdapter<>), null );

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
    public void Register( Type valueType, IValueAdapter valueAdapter ) => this._factory.RegisterTypeExtension( valueType, valueAdapter );

    /// <summary>
    /// Registers a generic value adapter.
    /// </summary>
    /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
    /// <param name="valueAdapterType">The type of the value adapter. This type must implement the <see cref="IValueAdapter"/>
    /// interface and have the same number of generic parameters as <paramref name="valueType"/>.
    /// </param>
    public void Register( Type valueType, Type valueAdapterType ) => this._factory.RegisterTypeExtension( valueType, valueAdapterType );

    /// <summary>
    /// Registers an <see cref="IValueAdapter{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the cached value (typically the return type of the cached method).</typeparam>
    /// <param name="valueAdapter">The adapter.</param>
    public void Register<T>( IValueAdapter<T> valueAdapter ) => this.Register( typeof(T), valueAdapter );

    /// <summary>
    /// Gets an <see cref="IValueAdapter"/> given a value type.
    /// </summary>
    /// <param name="valueType">The type of the cached value (typically the return type of the cached method).</param>
    /// <returns>A value adapter for <paramref name="valueType"/>, or <c>null</c> if no value adapter is available for <paramref name="valueType"/>.</returns>
    public IValueAdapter? Get( Type valueType ) => this._valueAdaptersByValueType.GetOrAdd( valueType, this.GetCore ).Extension;

    private TypeExtensionInfo<IValueAdapter> GetCore( Type valueType ) => this._factory.GetTypeExtension( valueType, this.CacheUpdateCallback );

    private void CacheUpdateCallback( TypeExtensionInfo<IValueAdapter> typeExtension ) => this._valueAdaptersByValueType[typeExtension.ObjectType] = typeExtension;
}