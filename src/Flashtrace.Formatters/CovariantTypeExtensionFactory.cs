// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Text;

namespace Flashtrace.Formatters;

[Obsolete( "Maybe not required post-TRole?", true )]
internal class CovariantTypeExtensionFactory<T> : TypeExtensionFactory<T>
    where T : class
{
    public CovariantTypeExtensionFactory(Type genericInterfaceType, Type converterType )
        : base( genericInterfaceType, converterType )
    {
    }

    protected override IEnumerable<Type> GetAssignableTypes(Type type)
    {
        yield return type;

        var baseType = type.BaseType;

        while (baseType != null && baseType != typeof(object))
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }

        foreach (var interfaceType in type.GetInterfaces())
        {
            yield return interfaceType;
        }

        yield return typeof(object);
    }

}

internal delegate void TypeExtensionCacheUpdateCallback<T>(TypeExtensionInfo<T> typeExtension) 
    where T : class;