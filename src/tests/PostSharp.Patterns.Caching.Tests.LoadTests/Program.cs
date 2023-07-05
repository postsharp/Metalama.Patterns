using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Backends;
using PostSharp.Patterns.Caching.Implementation;
using PostSharp.Patterns.Caching.Tests.LoadTests.Tests;
using PostSharp.Patterns.Utilities;

namespace PostSharp.Patterns.Caching.Tests.LoadTests
{
    class Program
    { 
        static void Main( string[] args )
        {
            Console.WriteLine( "create" );

            RedisLoadTestConfiguration configuration =
                new RedisLoadTestConfiguration()
                {
                    CollectorsCount = 2,
                    ClientsCount = 4,
                    ValueKeyLenght = new Interval( 20, 30 ),
                    ValueKeysCount = 100,
                    ValueKeyExpiry = new Interval( 5, 15 ),
                    ValueLength = new Interval( 50, 100 ),
                    DependencyKeyLenght = new Interval( 20, 30 ),
                    DependencyKeysCount = 10000,
                    DependenciesPerValueCount = new Interval( 0, 3 ),
                    ValuesPerSharedDependency = new Interval( 0, 5 )
                };

            //TimeSpan testDuration = TimeSpan.FromHours( 1 );
            TimeSpan duration = TimeSpan.FromSeconds( 30 );

            RedisLoadTest test = new RedisLoadTest();

            test.Test( configuration, duration );
        }
    }
}
