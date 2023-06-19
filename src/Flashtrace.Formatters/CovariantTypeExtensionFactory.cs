// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

internal static class CovariantTypeExtensionFactoryHelpers
{
    public static IEnumerable<Type> GetAssignableTypes( Type type )
    {
        yield return type;

        var baseType = type.BaseType;

        while ( baseType != null && baseType != typeof( object ) )
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }

        foreach ( var interfaceType in type.GetInterfaces() )
        {
            yield return interfaceType;
        }

        yield return typeof( object );
    }
}

internal class CovariantTypeExtensionFactory<T> : TypeExtensionFactory<T>
    where T : class
{
    public CovariantTypeExtensionFactory(Type genericInterfaceType, Type converterType )
        : base( genericInterfaceType, converterType )
    {
    }

    protected override IEnumerable<Type> GetAssignableTypes( Type type )
        => CovariantTypeExtensionFactoryHelpers.GetAssignableTypes( type );
}

internal class CovariantTypeExtensionFactory<T, TContext> : TypeExtensionFactory<T, TContext>
    where T : class
{
    public CovariantTypeExtensionFactory( Type genericInterfaceType, Type converterType, TContext? context )
        : base( genericInterfaceType, converterType, context )
    {
    }

    protected override IEnumerable<Type> GetAssignableTypes( Type type )
        => CovariantTypeExtensionFactoryHelpers.GetAssignableTypes( type );
}