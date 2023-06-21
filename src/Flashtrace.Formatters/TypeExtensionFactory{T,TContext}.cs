// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;

namespace Flashtrace.Formatters;

// T is for example IFormatter
// TContext is for example IFormatterRepository

/// <summary>
/// A TypeExtensionFactory for types deriving or implementing <typeparamref name="T"/>, where those types must have a constructor accepting a single argument of type <typeparamref name="TContext"/>.
/// </summary>
internal class TypeExtensionFactory<T, TContext> : TypeExtensionFactoryBase<T>
    where T : class
{
    private readonly object?[] _contextArray;

    public TypeExtensionFactory( Type genericInterfaceType, Type converterType, TContext? context )
        : base( genericInterfaceType, converterType )
    {
        this._contextArray = new object?[] { context };
    }

    [return: NotNullIfNotNull( nameof(o) )]
    public T? Convert( T? o, Type targetObjectType ) => this.Convert( o, targetObjectType, this._contextArray );

    public void RegisterTypeExtension( Type targetType, Type typeExtensionType )
        => this.RegisterTypeExtension( targetType, typeExtensionType, this._contextArray );

    public TypeExtensionInfo<T> GetTypeExtension(
        Type objectType,
        TypeExtensionCacheUpdateCallback<T> cacheUpdateCallback,
        Func<T> createDefault,
        Action<Exception>? onExceptionWhileCreatingTypeExtension = null )
        => this.GetTypeExtension(
            objectType,
            this._contextArray,
            cacheUpdateCallback,
            createDefault,
            onExceptionWhileCreatingTypeExtension );
}