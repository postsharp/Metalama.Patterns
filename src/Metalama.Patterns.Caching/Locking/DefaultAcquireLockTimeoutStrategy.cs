// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Globalization;

namespace Metalama.Patterns.Caching.Locking;

internal sealed class DefaultAcquireLockTimeoutStrategy : IAcquireLockTimeoutStrategy
{
    public void OnTimeout( string key )
        => throw new TimeoutException(
            string.Format( CultureInfo.InvariantCulture, "Timeout when attempting to acquire a lock on the cache item ${0}.", key ) );
}