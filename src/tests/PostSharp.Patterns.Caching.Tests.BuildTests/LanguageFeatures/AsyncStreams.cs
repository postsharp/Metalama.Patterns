//@LangVersion(8.0)
//@ExcludeFramework(NET)
//@MinFrameworkVersion(NETCore,3.0)
using System;
using System.Threading.Tasks;
using PostSharp.Patterns.Caching.Backends;
using System.Collections.Generic;
using PostSharp.Aspects;
using PostSharp.Aspects.Configuration;

namespace PostSharp.Patterns.Caching.BuildTests.LanguageFeatures
{
    namespace AsyncStreams
    {
        public class Program
        {
            public static int TimesCalled = 0;
            static int Main()
            {
                CachingServices.DefaultBackend = new MemoryCachingBackend();

                IAsyncEnumerable<int> sequence = TestClass.GenerateSequence();
                IAsyncEnumerator<int> sequence2 = TestClass.GenerateSequence2();
                IAsyncEnumerator<int> one = sequence.GetAsyncEnumerator();
                IAsyncEnumerator<int> two = sequence.GetAsyncEnumerator();

                Iterate( sequence2 );
                Iterate( one );
                Iterate( two );

                if ( TimesCalled != 3 )
                {
                    throw new Exception("Caching should have had no effect.");
                }
                
                return 0;
            }

            private static void Iterate( IAsyncEnumerator<int> iterator )
            {
                while ( iterator.MoveNextAsync().Result == true )
                {
                    Console.WriteLine("Yielding " + iterator.Current);
                }
            }
        }

        public class TestClass
        {
            [Cache(UnsupportedTargetAction = UnsupportedTargetAction.Fallback)]
            public static async System.Collections.Generic.IAsyncEnumerable<int> GenerateSequence()
            {
                Program.TimesCalled++;
                Console.WriteLine("Called.");
                await Task.FromResult( 0 );
                yield return 2;
            }
            
            
            [Cache(UnsupportedTargetAction = UnsupportedTargetAction.Fallback)]
            public static async System.Collections.Generic.IAsyncEnumerator<int> GenerateSequence2()
            {
                Program.TimesCalled++;
                Console.WriteLine("Called.");
                await Task.FromResult( 0 );
                yield return 2;
            }
        }
    }
}