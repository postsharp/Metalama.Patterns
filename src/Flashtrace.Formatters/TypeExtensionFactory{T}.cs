// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

// T is for example IFormatter

/// <summary>
/// A TypeExtensionFactory for types deriving or implementing <typeparamref name="T"/>, where those types must have a parameterless constructor.
/// </summary>
public  class TypeExtensionFactory<T> : TypeExtensionFactoryBase<T>
    where T : class
{
    // ReSharper disable once MemberCanBeProtected.Global
    public TypeExtensionFactory( Type genericInterfaceType, Type converterType ) : base( genericInterfaceType, converterType ) { }

    // ReSharper disable once UnusedMember.Global
    public T? Convert( T? o, Type targetObjectType ) => this.Convert( o, targetObjectType, null );

    // ReSharper disable once UnusedMember.Global
    public void RegisterTypeExtension( Type targetType, Type typeExtensionType ) => this.RegisterTypeExtension( targetType, typeExtensionType, null );

    // ReSharper disable once UnusedMember.Global
    public TypeExtensionInfo<T> GetTypeExtension(
        Type objectType,
        TypeExtensionCacheUpdateCallback<T> cacheUpdateCallback,
        Func<T> createDefault,
        Action<Exception>? onExceptionWhileCreatingTypeExtension = null )
        => this.GetTypeExtension( objectType, null, cacheUpdateCallback, createDefault, onExceptionWhileCreatingTypeExtension );
}