// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters.TypeExtensions;

public readonly struct TypeExtensionInfo<T>
    where T : class
{
    internal TypeExtensionInfo( T? extension, Type objectType, bool isGeneric )
    {
        this.Extension = extension;
        this.ObjectType = objectType ?? throw new ArgumentNullException( nameof(objectType) );
        this.IsGeneric = isGeneric;
    }

    public T? Extension { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public Type ObjectType { get; }

    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsGeneric { get; }

    internal bool ShouldOverwrite( TypeExtensionInfo<T> typeExtension )
        => CovariantTypeExtensionFactory<T>.ShouldOverwrite( this.ObjectType, this.IsGeneric, typeExtension.ObjectType, this.IsGeneric );
}

public delegate void TypeExtensionCacheUpdateCallback<T>( TypeExtensionInfo<T> typeExtension )
    where T : class;