// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Flashtrace.Formatters
{
    // TODO: Review use of ExplicitCrossPackageInternal
    //[ExplicitCrossPackageInternal]
    internal static class ReflectionHelpers
    {
        // TODO: Remove method.
        [Obsolete("Not supported, requires PostSharp.", true)]
        public static IList<TAttribute> GetCustomAttributesFromMethodOrProperty<TAttribute>(this MethodInfo methodInfo)
            where TAttribute : Attribute
        {
            throw new NotSupportedException();
#if false
            IList<TAttribute> attributes = ReflectionSearch.GetCustomAttributesOnTarget<TAttribute>(methodInfo);
            if (attributes.Count > 0)
            {
                return attributes;
            }

            PropertyInfo propertyInfo = methodInfo.GetAccessorProperty(false);

            if (propertyInfo != null)
            {
                return ReflectionSearch.GetCustomAttributesOnTarget<TAttribute>(propertyInfo);
            }

            return ArrayHelper.Empty<TAttribute>();
#endif
        }

        /// <summary>
        /// Gets fields with given attribute type and fields that hold value of property decorated with attribute of given type
        /// </summary>
        /// <param name="type">type which fields are to be returned</param>
        /// <param name="attributeType">type of the attribute</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        // TODO: Remove method.
        [Obsolete( "Not supported, requires PostSharp.", true )]
        public static IEnumerable<FieldInfo> GetFieldsWithAttributeOnFieldOrProperty(Type type, Type attributeType, bool inherit)
        {
            throw new NotSupportedException();
#if false
            foreach (FieldInfo fieldInfo in type.GetFields(BindingFlagsSet.AllInstanceDeclared))
            {
                if (IsDefinedOnFieldOrProperty(fieldInfo, attributeType, inherit))
                {
                    yield return fieldInfo;
                }
            }
#endif
        }

        // TODO: Remove method.
        [Obsolete( "Not supported, requires PostSharp.", true )]
        public static bool IsDefinedOnFieldOrProperty(FieldInfo fieldInfo, Type attributeType, bool inherit)
        {
            throw new NotSupportedException();
#if false
            if (fieldInfo.IsDefined(attributeType, inherit))
            {
                return true;
            }

            PropertyInfo propertyInfo = ReflectionExtensions.GetAutomaticProperty(fieldInfo, inherit);
            if (propertyInfo != null && propertyInfo.IsDefined(attributeType, inherit))
            {
                return true;
            }

            return false;
#endif
        }

        // TODO: Remove method.
        [Obsolete( "Not supported, requires PostSharp.", true )]
        public static bool IsDefinedOnMethodOrProperty(this MethodInfo methodInfo, Type attributeType, bool inherit)
        {
            throw new NotSupportedException();
#if false
            PropertyInfo propertyInfo;
            bool hasCustomAttribute;
            hasCustomAttribute = ReflectionApiWrapper.HasCustomAttribute(methodInfo, attributeType, inherit);

            if (hasCustomAttribute)
            {
                return true;
            }

            if ((propertyInfo = methodInfo.GetAccessorProperty()) != null)
            {
                return propertyInfo.IsDefined(attributeType, inherit);
            }

            return false;
#endif
        }

        public static bool IsAnonymous(this Type type)
        {
            return type.IsDefined( typeof( CompilerGeneratedAttribute ), false )
                && type.Name.Contains( "AnonymousType" )
                && (type.Name.StartsWith( "<>", StringComparison.OrdinalIgnoreCase ) || type.Name.StartsWith( "VB$", StringComparison.OrdinalIgnoreCase ));        
        }
    }
}