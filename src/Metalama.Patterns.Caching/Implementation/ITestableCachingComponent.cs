// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching.Implementation
{
    // TODO: [Porting] Interface needs to be common, but is testing-only implementation detail. Making public for now.
    [ExplicitCrossPackageInternal]
    public interface ITestableCachingComponent : IDisposable
    {
        Task DisposeAsync( CancellationToken cancellationToken = default(CancellationToken) );

        int BackgroundTaskExceptions { get; }
    }
}