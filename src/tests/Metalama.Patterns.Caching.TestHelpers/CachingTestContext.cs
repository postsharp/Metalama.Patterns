// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.TestHelpers;

public sealed class CachingTestContext<T> : IDisposable, IAsyncDisposable
    where T : CachingBackend
{
    public T Backend { get; }

    public CachingTestContext( T backend )
    {
        this.Backend = backend;
    }

    public void Dispose()
    {
        TestableCachingComponentDisposer.Dispose( CachingService.Default.DefaultBackend );
        CachingService.Default = CachingService.CreateUninitialized();
    }

    public async ValueTask DisposeAsync()
    {
        await TestableCachingComponentDisposer.DisposeAsync( CachingService.Default.DefaultBackend );
        CachingService.Default = CachingService.CreateUninitialized();
    }
}