using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Backends;
using System.Diagnostics;

namespace PostSharp.Patterns.Caching.BuildTests
{
    // Fixed as #20774
    namespace Issue16527
    {
        public class Program
        {
            static int Main()
            {
                CachingServices.DefaultBackend = new MemoryCachingBackend();

                // This shouldn't fail, even though the CachedClass type hasn't been touched yet.
                new InvalidatingClass().Invalidate();
                return 0;
            }
        }

        public class CachedClass
        {
            [Cache(IgnoreThisParameter=true)]
            public int GetValue()
            {
                return 0;
            }
        }

        public class InvalidatingClass
        {
            [InvalidateCache(typeof(CachedClass), "GetValue")]
            public void Invalidate()
            {
            }
        }
    }
}
