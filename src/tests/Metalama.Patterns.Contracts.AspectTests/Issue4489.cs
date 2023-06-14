// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Skipped(PS-specific)

/* Ported from PostSharp, addresed issue #4489
 * Assignment distance from value type to ValueType is different for intrinsic value types and
 * other value types (DateTime, enums, ...)
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
    namespace Issue4489
    {
        public static class Program
        {
            public static int Main()
            {
                TestClass test = new TestClass();
                test.IntA = 1;
                test.IntB = 1;
                test.TimestampA = DateTime.UtcNow;
                test.TimestampB = DateTime.UtcNow;

                return 0;
            }
        }

        
        public class TestClass
        {
            [NotDefaultValueType]
            public int IntA { get; set; }

            [NotDefaultObject]
            public int IntB { get; set; }

            [NotDefaultValueType]
            public DateTime TimestampA { get; set; }
            
            [NotDefaultObject]
            public DateTime TimestampB { get; set; }
            
        }


        [HasConstraint]
        public sealed class NotDefaultValueTypeAttribute : BaseTestContract, ILocationValidationAspect<ValueType>
        {
            public Exception ValidateValue( ValueType value, string locationName, LocationKind locationKind, LocationValidationContext context )
            {
                return this.ValidateImpl( value, locationName, locationKind );
            }
        }


        [HasConstraint]
        public sealed class NotDefaultObjectAttribute : BaseTestContract, ILocationValidationAspect<object>
        {
            public Exception ValidateValue(object value, string locationName, LocationKind locationKind, LocationValidationContext context)
            {
                return this.ValidateImpl(value, locationName, locationKind);
            }
        }


        public class BaseTestContract : LocationContractAttribute
        {
            public override Exception ValidateValueDynamic(object value, string locationName, LocationKind locationKind, LocationValidationContext context)
            {
                if (value is ValueType)
                {
                    return this.ValidateImpl((ValueType)value, locationName, locationKind);
                }

                return null;
            }

            public Exception ValidateImpl( object value, string locationName, LocationKind locationKind )
            {
                if ( value == null )
                    return null;

                Type type = value.GetType();
                bool isGenericType = type.IsGenericType();
                
                if ( isGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) )
                {
                    if ( value.Equals( Activator.CreateInstance( type.GetGenericArguments()[0] ) ) )
                        return this.CreateArgumentException( value, locationName, locationKind );
                }
                else if ( value.Equals( Activator.CreateInstance( type ) ) )
                    return this.CreateArgumentNullException( value, locationName, locationKind );

                return null;
            }
        }
    }
}