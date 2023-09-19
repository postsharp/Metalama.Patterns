using System.Reflection;
using Metalama.Patterns.Caching.Implementation;
namespace Metalama.Patterns.Caching.AspectTests.CacheAttributeTests.Static;
[CachingConfiguration(UseDependencyInjection = false)]
public class C
{
    [Cache]
    public static int M()
    {
        object? Invoke(object? instance, object? [] args)
        {
            return C.M_Source();
        }
        return CachingServices.Default.GetFromCacheOrExecute<int>(_cacheRegistration_M!, Invoke, null, new object[] { });
    }
    private static int M_Source() => 5;
    private static readonly CachedMethodMetadata _cacheRegistration_M;
    static C()
    {
        C._cacheRegistration_M = CachedMethodMetadata.Register(RunTimeHelpers.ThrowIfMissing(typeof(C).GetMethod("M", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null)!, "C.M()"), null, new CacheAttributeProperties() { AbsoluteExpiration = null, AutoReload = false, IgnoreThisParameter = null, Priority = CacheItemPriority.Default, ProfileName = (string? )null, SlidingExpiration = null }, false);
    }
}