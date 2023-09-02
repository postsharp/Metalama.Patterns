// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Caching.Aspects;

[CompileTime]
internal sealed record CachingAspectConfiguration : CachedMethodConfiguration
{
    public bool? UseDependencyInjection { get; init; }

    public CachingAspectConfiguration() { }

    public CachingAspectConfiguration( CachingAspectConfiguration overrideValue, CachingAspectConfiguration fallbackValue ) : base(
        overrideValue,
        fallbackValue )
    {
        this.UseDependencyInjection = overrideValue.UseDependencyInjection ?? fallbackValue.UseDependencyInjection;
    }

    public CachingAspectConfiguration ApplyFallbackValues( CachingAspectConfiguration fallback ) => new( this, fallback );
}