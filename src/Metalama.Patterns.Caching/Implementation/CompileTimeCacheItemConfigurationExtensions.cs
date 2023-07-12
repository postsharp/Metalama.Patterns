// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Caching.Implementation;

[CompileTime]
internal static class CompileTimeCacheItemConfigurationExtensions
{
    public static CompileTimeCacheItemConfiguration ToCompileTimeCacheItemConfiguration( this IAttribute attribute ) => new( attribute );
}