using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Implementation;

namespace PostSharp.Patterns.Caching.Tests.LoadTests.Tests
{
    abstract class BaseTestClass<LoadTestConfigurationT>
        where LoadTestConfigurationT : LoadTestConfiguration
    {
        public virtual void Test( LoadTestConfigurationT configuration, TimeSpan duration )
        {
            using ( LoadTest test = new LoadTest( configuration ) )
            {
                Console.WriteLine( "test init" );

                test.Initialize( CreateCachingBackend );

                Console.WriteLine( "start" );

                test.Start();

                Console.WriteLine( "run" );

                Thread.Sleep( duration );

                Console.WriteLine( "stop" );

                test.Stop();

                test.Report();

                Console.WriteLine( "dispose" );
            }

            Console.WriteLine( "end" );
        }

        protected abstract CachingBackend CreateCachingBackend();
    }
}
