// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Caching.Aspects;

[CompileTime]
internal sealed record CachingAspectConfiguration : CachedMethodConfiguration
{
    public bool? UseDependencyInjection { get; init; }

    public CachingAspectConfiguration ApplyFallbackValues( CachingAspectConfiguration fallback )
    {
        return new CachingAspectConfiguration
        {
            AutoReload = this.AutoReload ?? fallback.AutoReload,
            AbsoluteExpiration = this.AbsoluteExpiration ?? fallback.AbsoluteExpiration,
            SlidingExpiration = this.SlidingExpiration ?? fallback.SlidingExpiration,
            Priority = this.Priority ?? fallback.Priority,
            ProfileName = this.ProfileName ?? fallback.ProfileName,
            IsEnabled = this.IsEnabled ?? fallback.IsEnabled,
            IgnoreThisParameter = this.IgnoreThisParameter ?? fallback.IgnoreThisParameter,
            UseDependencyInjection = this.UseDependencyInjection ?? fallback.UseDependencyInjection
        };
    }
}