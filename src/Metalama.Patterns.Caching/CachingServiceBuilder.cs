// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Backends;
using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching;

[PublicAPI]
public sealed class CachingServiceBuilder
{
    internal CachingServiceBuilder( IServiceProvider serviceProvider )
    {
        this.ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; }

    public CachingBackend? Backend { get; set; }

    public List<CachingProfile> Profiles { get; } = new();

    public Func<IFormatterRepository, CacheKeyBuilder>? KeyBuilderFactory { get; set; }

    internal CachingService Build()
    {
        return new CachingService(
            this.Backend ?? new MemoryCachingBackend(),
            profiles: this.Profiles,
            keyBuilderFactory: this.KeyBuilderFactory,
            serviceProvider: this.ServiceProvider );
    }
}