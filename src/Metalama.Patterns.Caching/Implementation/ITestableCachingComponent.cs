// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Implementation;

// TODO: [Porting] Interface needs to be common, but is testing-only implementation detail. Making public for now.
[ExplicitCrossPackageInternal]
public interface ITestableCachingComponent : IDisposable
{
    // ReSharper disable once UnusedMemberInSuper.Global
    Task DisposeAsync( CancellationToken cancellationToken = default );

    // ReSharper disable once UnusedMemberInSuper.Global
    int BackgroundTaskExceptions { get; }
}