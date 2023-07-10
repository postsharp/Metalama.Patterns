// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;

namespace Metalama.Patterns.Caching.ManualTest;

/// <summary>
/// Maintains a single instance of a Redis server that's disposed only when all tests finish executing. That way, we don't spend time starting the server
/// again at the beginning of each test.
/// </summary>
public static class RedisPersistentInstance
{
    private static RedisTestInstance runningServer;

    /// <summary>
    /// Gets a reference to the shared Redis server running on this computer. The first time this method is called, the server process is launched first. 
    /// </summary>
    public static RedisTestInstance GetOrLaunchRedisInstance()
    {
        if ( runningServer == null )
        {
            runningServer = new RedisTestInstance();
        }

        return runningServer;
    }

    /// <summary>
    /// Kills the shared Redis server (the redis-test-8fgd53f4sgd.exe process).
    /// </summary>
    public static void KillServer()
    {
        if ( runningServer != null && !runningServer.IsDisposed )
        {
            runningServer.Dispose();
        }
    }
}