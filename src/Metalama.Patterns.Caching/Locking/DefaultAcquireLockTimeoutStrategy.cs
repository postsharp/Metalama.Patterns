// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Globalization;

namespace Metalama.Patterns.Caching.Locking
{
    internal sealed class DefaultAcquireLockTimeoutStrategy : IAcquireLockTimeoutStrategy
    {
        public void OnTimeout( string key )
        {
            throw new TimeoutException( string.Format(CultureInfo.InvariantCulture, "Timeout when attempting to acquire a lock on the cache item ${0}.", key ) );
        }
    }
}
