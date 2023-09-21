// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Caching.Aspects;

[CompileTime]
internal sealed record CachingAspectConfiguration : CachedMethodConfiguration
{
    public bool? UseDependencyInjection { get; init; }

    public CachingAspectConfiguration() { }

#pragma warning disable IDE0051
    private CachingAspectConfiguration( CachingAspectConfiguration overrideValue, CachingAspectConfiguration baseValue ) : base(
        overrideValue,
        baseValue )
    {
        this.UseDependencyInjection = overrideValue.UseDependencyInjection ?? baseValue.UseDependencyInjection;
    }
#pragma warning restore IDE0051

    public CachingAspectConfiguration ApplyFallbackValues( CachingAspectConfiguration fallback ) => new( this, fallback );
}