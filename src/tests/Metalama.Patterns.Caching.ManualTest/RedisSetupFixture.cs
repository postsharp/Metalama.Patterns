// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.ManualTest;

/// <summary>
/// Maintains a single instance of a Redis server that's disposed only when all tests finish executing. That way, we don't spend time starting the server
/// again at the beginning of each test.
/// </summary>
public sealed class RedisSetupFixture : IDisposable
{
    public RedisSetupFixture() { }

    void IDisposable.Dispose() => this.RedisCleanup();

    private readonly object _lock = new();
    private RedisTestInstance? _testInstance;

    /// <summary>
    /// Gets a reference to the shared Redis server running on this computer. The first time this method is called, the server process is launched first. 
    /// </summary>
    public RedisTestInstance TestInstance
    {
        get
        {
            lock ( this._lock )
            {
                this._testInstance ??= new RedisTestInstance( nameof(RedisSetupFixture) );

                if ( this._testInstance.IsDisposed )
                {
                    throw new ObjectDisposedException( nameof(RedisSetupFixture) + "." + nameof(this.TestInstance) );
                }
            }

            return this._testInstance;
        }
    }

    /// <summary>
    /// Kills the shared Redis server (the redis-test-8fgd53f4sgd.exe process).
    /// </summary>
    private void RedisCleanup()
    {
        lock ( this._lock )
        {
            if ( this._testInstance != null && !this._testInstance.IsDisposed )
            {
                this._testInstance.Dispose();
            }
        }

        foreach ( var instanceWR in RedisTestInstance.Instances )
        {
            if ( instanceWR.TryGetTarget( out var instance ) && !instance.IsDisposed )
            {
                // The exception is silently ignored when the tests are run with Rider's Live Test Runner.
                // If that's not the expected behavior, we should probably report it...
                throw new Exception( $"RedisTestInstance '{instance.Name}' was not disposed." );
            }
        }
    }
}