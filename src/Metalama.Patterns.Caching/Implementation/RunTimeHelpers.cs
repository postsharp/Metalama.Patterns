// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Reflection;

namespace Metalama.Patterns.Caching.Implementation;

public static class RunTimeHelpers
{
    public static MethodInfo ThrowIfMissing( this MethodInfo? methodInfo, string methodSignature )
        => methodInfo ?? throw new MissingMethodException( $"The method could not be found: {methodSignature}" );
}