//@ExpectedMessage(CAC010//.*DoActionRef.*)
//@ExpectedMessage(CAC010//.*DoActionOut.*)
//@LangVersion(7.3)
using System;
using PostSharp.Patterns.Caching.Backends;

namespace PostSharp.Patterns.Caching.BuildTests.CacheAttributeTests
{
    namespace CacheRefOut
    {
        public class CachingClass
        {
            [Cache]
            public int DoActionRef( ref int a )
            {
                return 42;
            }
            [Cache]
            public int DoActionIn( in int a )
            {
                return 42;
            }
            [Cache]
            public int DoActionOut( out int a )
            {
                a = 8;
                return 42;
            }
            [Cache]
            public int DoActionControl( int a )
            {
                return 42;
            }
        }
        public class Program
        {

            public static int Main()
            {
                // The build should have failed
                return 1;
            }
        }
    }
}