using System.Reflection;
using Metalama.Patterns.Caching.Aspects;
using Metalama.Patterns.Caching.Aspects.Helpers;
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
        return ((ICachingService)CachingService.Default).GetFromCacheOrExecute<int>(_cacheRegistration_M!, null, new object[] { }, Invoke);
    }
    private static int M_Source() => 5;
    private static readonly CachedMethodMetadata _cacheRegistration_M;
    static C()
    {
        C._cacheRegistration_M = CachedMethodMetadata.Register(RunTimeHelpers.ThrowIfMissing(typeof(C).GetMethod("M", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null)!, "C.M()"), new CachedMethodConfiguration() { AbsoluteExpiration = null, AutoReload = null, IgnoreThisParameter = null, Priority = null, ProfileName = (string? )null, SlidingExpiration = null }, false);
    }
}