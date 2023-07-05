// @MinToolsVersion(15.0)

using PostSharp.Patterns.Caching.Backends;

namespace PostSharp.Patterns.Caching.BuildTests.LanguageFeatures
{
    namespace TupleSimple
    {
        public class Program
        {
            static int Main()
            {
                CachingServices.DefaultBackend = new MemoryCachingBackend();

                TestClass.SimpleMethod((4, 5));

                return 0;
            }
        }

        public class TestClass
        {
            [Cache]
            public static (int x, int y) SimpleMethod((int x, int y) a)
            {
                return (a.x, a.y);
            }
        }
    }
}
