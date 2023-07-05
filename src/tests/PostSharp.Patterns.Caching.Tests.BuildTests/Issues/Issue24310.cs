using System.Threading.Tasks;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Backends;

namespace PostSharp.Patterns.Caching.BuildTests
{
    namespace Issue24310
    {
        public class Program
        {
            static void Main(string[] args)
            {
                CachingServices.DefaultBackend = new MemoryCachingBackend();

                TestAsync().Wait();
            }

            [Cache]
            static async Task<int> TestAsync()
            {
                await Task.Delay(1);
                await Task.Yield();

                return await Task.FromResult(1);
            }
        }
    }
}
