// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

internal struct TypeExtensionInfo<T>
    where T : class
{
    internal TypeExtensionInfo( T extension, Type objectType, bool isGeneric )
    {
        this.Extension = extension;
        this.ObjectType = objectType;
        this.IsGeneric = isGeneric;
    }

    public T Extension { get; private set; }

    public Type ObjectType { get; private set; }

    public bool IsGeneric { get; private set; }

    public bool ShouldOverwrite( TypeExtensionInfo<T> typeExtension )
    {
        return CovariantTypeExtensionFactory<T>.ShouldOverwrite( this.ObjectType, this.IsGeneric, typeExtension.ObjectType, this.IsGeneric );
    }
}