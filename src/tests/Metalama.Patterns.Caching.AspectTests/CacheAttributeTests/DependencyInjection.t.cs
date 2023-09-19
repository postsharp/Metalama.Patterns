using System.Reflection;
using Metalama.Patterns.Caching.Implementation;
namespace Metalama.Patterns.Caching.AspectTests.CacheAttributeTests.DependencyInjection;
public class C
{
    [Cache]
    public int M()
    {
        object? Invoke(object? instance, object? [] args)
        {
            return ((C)instance).M_Source();
        }
        return _cachingService!.GetFromCacheOrExecute<int>(_cacheRegistration_M!, Invoke, this, new object[] { });
    }
    private int M_Source() => 5;
    private static readonly CachedMethodMetadata _cacheRegistration_M;
    private CachingService _cachingService;
    static C()
    {
        C._cacheRegistration_M = CachedMethodMetadata.Register(RunTimeHelpers.ThrowIfMissing(typeof(C).GetMethod("M", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)!, "C.M()"), null, new CacheAttributeProperties() { AbsoluteExpiration = null, AutoReload = false, IgnoreThisParameter = null, Priority = CacheItemPriority.Default, ProfileName = (string? )null, SlidingExpiration = null }, false);
    }
    public C(CachingService? cachingService = default)
    {
        this._cachingService = cachingService ?? throw new System.ArgumentNullException(nameof(cachingService));
    }
}