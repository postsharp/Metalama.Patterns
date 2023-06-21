// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters;

internal sealed class CovariantTypeExtensionFactory<T, TContext> : TypeExtensionFactory<T, TContext>
    where T : class
{
    public CovariantTypeExtensionFactory( Type genericInterfaceType, Type converterType, TContext? context )
        : base( genericInterfaceType, converterType, context ) { }

    protected override IEnumerable<Type> GetAssignableTypes( Type type ) => CovariantTypeExtensionFactoryHelpers.GetAssignableTypes( type );
}