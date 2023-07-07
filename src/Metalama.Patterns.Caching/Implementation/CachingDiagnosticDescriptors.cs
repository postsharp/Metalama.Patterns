// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Diagnostics;
using static Metalama.Framework.Diagnostics.Severity;

namespace Metalama.Patterns.Caching;

[CompileTime]
internal static class CachingDiagnosticDescriptors
{
    private const string _cagtegory = "Metalama.Patterns.Caching";

    // Reserved range 5100-5120

    // Original PS CAC001 and CAC010 apply to [Cache] and are handled by eligibility.

    public static class InvalidateCache
    {
        
        /// <summary>
        /// Was PS CAC002:
        /// The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: this method is not cached by the [Cache] aspect.
        /// </summary>
        public static readonly DiagnosticDefinition<(IMethod InvalidatingMethod, IMethod InvalidatedMethod)> ErrorMethodIsNotCached =
            new(
                "LAMA5100",
                Error,
                "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: this method is not cached by the [Cache] aspect.",
                "Method is not cached.",
                _cagtegory );

        /// <summary>
        /// Was PS CAC003:
        /// The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the 'this' parameter cannot be mapped because (a) this is an instance method, (b) the method, class or assembly is not annotated with the [CacheConfiguration( IgnoreThisParameter = true )]] attribute and (c) the type {2} is not derived from {3}.
        /// </summary>
        public static readonly DiagnosticDefinition<(IMethod InvalidatingMethod, IMethod CachedMethod, IType InvalidatingMethodDeclaringType, IType CachedMethodDeclaringType)> ErrorThisParameterCannotBeMapped =
            new(
                "LAMA5101",
                Error,
                "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the 'this' parameter cannot be mapped because " +
                    "(a) this is an instance method, (b) the method, class or assembly is not annotated with the " +
                    "[CacheConfiguration( IgnoreThisParameter = true )]] attribute and (c) the type {2} is not derived from {3}.",
                "'this' parameter cannot be mapped.",
                _cagtegory );

        /// <summary>
        /// Was PS CAC004:
        /// The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the cached method does not contain a parameter named '{2}'. Make sure {0} contains a parameter named '{2}' or add the [NotCachedKey] attribute to the '{2}' parameter in {1}.
        /// </summary>
        public static readonly DiagnosticDefinition<(IMethod InvalidatingMethod, IMethod CachedMethod, string ParameterName)> ErrorMissingParameterInInvalidatingMethod =
            new(
                "LAMA5102",
                Error,
                "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the invalidating method does not contain a parameter named '{2}'. " +
                    "Make sure {0} contains a parameter named '{2}' or add the [NotCachedKey] attribute to the '{2}' parameter in {1}.",
                "No matching parameter in invalidating method.",
                _cagtegory );

        /// <summary>
        /// Was PS CAC005:
        /// The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the type of the '{2}' parameter of the cached is not compatible with the type of the '{2}' parameter in the invalidating method.
        /// </summary>
        public static readonly DiagnosticDefinition<(IMethod InvalidatingMethod, IMethod CachedMethod, string ParameterName)> ErrorParameterTypeIsNotCompatible =
            new(
                "LAMA5103",
                Error,
                "The [InvalidateCache] aspect applied to {0} cannot invalidate {1}: the type of the '{2}' parameter of the cached method is not compatible " +
                    "with the type of the '{2}' parameter in the invalidating method.",
                "Parameter type is not compatible.",
                _cagtegory );

        /// <summary>
        /// Was PS CAC006:
        /// Invalid [InvalidateCache] aspect on {0}: the constructor parameters cannot contain a null or empty string.
        /// </summary>
        public static readonly DiagnosticDefinition<IMethod> ErrorInvalidAspectConstructorNullOrEmptyString =
            new(
                "LAMA5104",
                Error,
                "Invalid [InvalidateCache] aspect on {0}: the constructor parameters cannot contain a null or empty string.",
                "[InvalidateCache] parameters cannot contain a null or empty string.",
                _cagtegory );

        /// <summary>
        /// Was PS CAC007:
        /// Invalid [InvalidateCache] aspect on {0}: the constructor parameters must contain at least one method name.
        /// </summary>
        public static readonly DiagnosticDefinition<IMethod> ErrorInvalidAspectConstructorNoMethodName =
            new(
                "LAMA5105",
                Error,
                "Invalid [InvalidateCache] aspect on {0}: the constructor parameters must contain at least one method name.",
                "[InvalidateCache] must contain at least one method name.",
                _cagtegory );

        /// <summary>
        /// Was PS CAC008:
        /// Invalid [InvalidateCache] aspect on {0}: there is no method named {1} in type {2}.
        /// </summary>
        public static readonly DiagnosticDefinition<(IMethod InvalidatingMethod, string MethodName, IType CachedMethodDeclaringType)> ErrorCachedMethodNotFound =
            new(
                "LAMA5106",
                Error,
                "Invalid [InvalidateCache] aspect on {0}: there is no method named {1} in type {2}.",
                "Cached method not found.",
                _cagtegory );

        /// <summary>
        /// Was PS CAC009:
        /// Invalid [InvalidateCache] aspect on {0}: there are several suitable overloads of the {1} method. Set the AllowMultipleOverloads property to \"true\" to allow invalidation of all of them.
        /// </summary>
        public static readonly DiagnosticDefinition<(IMethod InvalidatingMethod, string MethodName)> ErrorMultipleOverloadsFound =
            new(
                "LAMA5107",
                Error,
                "Invalid [InvalidateCache] aspect on {0}: there are several suitable overloads of the {1} method. Set the AllowMultipleOverloads property to \"true\" to allow invalidation of all of them.",
                "Multiple suitable overloads found.",
                _cagtegory );

        // TODO: [Porting] This is probably not the best way to annotate - it shows up as "Messages" in VS.

        public static readonly DiagnosticDefinition<IMethod> InfoMethodIsInvalidatedBy =
            new(
                "LAMA5108",
                Info,
                "This method is invalidated by {0}.",
                "Invalidated by an [InvalidateCache] method.",
                _cagtegory );

        public static readonly DiagnosticDefinition<IMethod> InfoMethodInvalidates =
            new(
                "LAMA5109",
                Info,
                "This method is invalidates {0}.",
                "Invalidates a [Cache] method.",
                _cagtegory );
    }
}