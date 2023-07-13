// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Locking;

namespace Metalama.Patterns.Caching.Tests;

internal sealed class IgnoreLockStrategy : IAcquireLockTimeoutStrategy
{
    public void OnTimeout( string key ) { }
}