// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching;

// ReSharper disable UnusedType.Global
[Obsolete( "Porting TODO" )]
internal class CachingMessageSource
{
#if TODO
        public static readonly MessageSource Instance = new MessageSource( "PostSharp.Patterns.Caching", new CachingMessageDispenser() );

        private class CachingMessageDispenser : MessageDispenser
        {
            public CachingMessageDispenser()
                : base( "CAC" )
            {
            }

            protected override string GetMessage( int number )
            {
                switch ( number )
                {
                    case 1:
                        return "Cannot add the [Cache] aspect to the {0} method because its return type is \"void\".";

                    case 2:
                        return
                            "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: this method is not cached by the [Cache] aspect.";

                    case 3:
                        return
                            "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the 'this' parameter cannot be mapped because (a) this is an instance method, (b) the method, class or assembly is not annotated with the [CacheConfiguration( IgnoreThisParameter = true )]] attribute and (c) the type {2} is not derived from {3}.";

                    case 4:
                        return
                            "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the cached method does not contain a parameter named '{2}'. Make sure {0} contains a parameter named '{2}' or add the [NotCachedKey] attribute to the '{2}' parameter in {1}.";

                    case 5:
                        return
                            "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the type of the '{2}' parameter of the cached is not compatible with the type of the '{2}' parameter in the invalidating method.";

                    case 6:
                        return "Invalid [InvalidateCache] aspect on {0}: the constructor parameters cannot contain a null or empty string.";

                    case 7:
                        return "Invalid [InvalidateCache] aspect on {0}: the constructor parameters should contain at least one method name.";

                    case 8:
                        return "Invalid [InvalidateCache] aspect on {0}: there is no method named {1} in type {2}.";

                    case 9:
                        return "Invalid [InvalidateCache] aspect on {0}: there are several suitable overloads of the {1} method. Set the AllowMultipleOverloads property to \"true\" to allow invalidation of all of them.";

                    case 10:
                        return "Cannot add the [Cache] aspect to the {0} method because it has a ref or out parameter.";
                    default:
                        return null;
                }
            }
        }
#endif
}