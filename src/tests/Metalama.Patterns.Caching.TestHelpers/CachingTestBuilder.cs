// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.TestHelpers;

public sealed class CachingTestBuilder
{
    private readonly CachingService.Builder _serviceBuilder;

    public CachingTestBuilder( CachingService.Builder serviceBuilder )
    {
        this._serviceBuilder = serviceBuilder;
    }

    public CachingTestBuilder WithProfile( string name ) => this.WithProfile( new CachingProfile( name ) );

    public CachingTestBuilder WithProfile( CachingProfile profile )
    {
        this._serviceBuilder.AddProfile( profile );

        return this;
    }

    public CachingTestBuilder WithKeyBuilder( Func<IFormatterRepository, CacheKeyBuilder> factory )
    {
        this._serviceBuilder.KeyBuilderFactory = factory;

        return this;
    }
}