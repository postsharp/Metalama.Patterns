// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

internal readonly struct TypeExtensionInfo<T>
    where T : class
{
    internal TypeExtensionInfo( T extension, Type objectType, bool isGeneric )
    {
        this.Extension = extension ?? throw new ArgumentNullException( nameof(extension) );
        this.ObjectType = objectType ?? throw new ArgumentNullException( nameof(objectType) );
        this.IsGeneric = isGeneric;
    }

    public T Extension { get; }

    public Type ObjectType { get; }

    public bool IsGeneric { get; }

    public bool ShouldOverwrite( TypeExtensionInfo<T> typeExtension )
        => CovariantTypeExtensionFactory<T>.ShouldOverwrite( this.ObjectType, this.IsGeneric, typeExtension.ObjectType, this.IsGeneric );
}

internal delegate void TypeExtensionCacheUpdateCallback<T>( TypeExtensionInfo<T> typeExtension )
    where T : class;