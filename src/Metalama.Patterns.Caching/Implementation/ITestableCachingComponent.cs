// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

// Was [ExplicitCrossPackageInternal]. Used by Redis backend.
public interface ITestableCachingComponent : IDisposable
{
    Task DisposeAsync( CancellationToken cancellationToken = default );

    int BackgroundTaskExceptions { get; }
}