// @MinToolsVersion(15.0)

using PostSharp.Patterns.Caching.Backends;

namespace PostSharp.Patterns.Caching.BuildTests.LanguageFeatures
{
    namespace LocalFunctionSimple
    {
        public class Program
        {
            static int Main()
            {
                CachingServices.DefaultBackend = new MemoryCachingBackend();

                TestClass.SimpleMethod(42);

                return 0;
            }
        }

        public class TestClass
        {
            [Cache]
            public static int SimpleMethod(int x)
            {
                int foo()
                {
                    return x + 1;
                }

                return foo();
            }
        }
    }
}
