// @Skipped(PS-specific)

/* Ported from PostSharp, addresed issue #4165
 * Runtime System.MissingMethodException when a method/field signature contains ValueType
 * 
 * Action: Skiping, PS specific.
 */

using System;
using PostSharp.Aspects;
using PostSharp.Constraints.Internals;
using Metalama.Patterns.Contracts;
using PostSharp.Reflection;

namespace PostSharp.Patterns.Model.BuildTests.Contracts
{
    namespace Issue4165.Dependency
    {
        [HasConstraint]
        public sealed class NotDefaultAttribute : LocationContractAttribute, ILocationValidationAspect<ValueType>, ILocationValidationAspect, IAspect
        {
            public NotDefaultAttribute()
            {
                ErrorMessage = "The {2} cannot be empty.";
            }

            public override Exception ValidateValueDynamic(object value, string locationName, LocationKind locationKind,
                LocationValidationContext context)
            {
                if (value is ValueType)
                {
                    return this.ValidateValue((ValueType) value, locationName, locationKind, context);
                }

                return null;
            }

            public Exception ValidateValue( ValueType value, string locationName, LocationKind locationKind, LocationValidationContext context )
            {
                if ( value == null )
                    return null;

                var type = value.GetType();
                bool isGenericType = type.IsGenericType();
                
                if ( isGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) )
                {
                    if ( value.Equals( Activator.CreateInstance( type.GetGenericArguments()[0] ) ) )
                        return CreateArgumentException( value, locationName, locationKind );
                }
                else if ( value.Equals( Activator.CreateInstance( type ) ) )
                    return CreateArgumentNullException( value, locationName, locationKind );

                return null;
            }
        }
    }
}