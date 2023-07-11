// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Fabrics;
using System.Linq;

namespace Metalama.Patterns.Caching.AspectTests.InvalidateCacheAttributeTests.InvalidateDependencyMethod;

public sealed class DependencyClass
{
    [Cache(IgnoreThisParameter = true)]
    public int CachedUsingAttribute() => 42;

    public int CachedUsingFabric() => 42;

    private sealed class Fabric : TypeFabric
    {
        public override void AmendType(ITypeAmender amender)
        {
            amender
                .Outbound
                .Select(t => t.Methods.Single(m => m.Name == nameof(DependencyClass.CachedUsingFabric)))
                .AddAspect(m => new CacheAttribute() { IgnoreThisParameter = true });
        }
    }
}