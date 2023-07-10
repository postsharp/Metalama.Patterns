using Metalama.Aspects;
using Metalama.Aspects.Advices;
using System;
using System.Collections;
using System.Diagnostics;

namespace Metalama.Patterns.Caching.Tests
{
    public class RedisSetup
    {
        // TODO: use AssemblyFixture when it's fixed.

        [ModuleInitializer( 0 )]
        public static void Initialize()
        {
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        private static void CurrentDomain_DomainUnload( object sender, EventArgs e )
        {
            RedisCleanup();
        }

        public static void RedisCleanup()
        {
            RedisPersistentInstance.KillServer();
            foreach ( WeakReference<RedisTestInstance> instanceWR in RedisTestInstance.Instances )
            {
                if ( instanceWR.TryGetTarget( out RedisTestInstance instance ) && !instance.IsDisposed )
                {
                    // The exception is silently ignored when the tests are run with Rider's Live Test Runner.
                    // If that's not the expected behavior, we should probably report it...
                    throw new Exception( $"RedisTestInstance {instance.Name} was not disposed." );
                }
            }
        }
    }
}
