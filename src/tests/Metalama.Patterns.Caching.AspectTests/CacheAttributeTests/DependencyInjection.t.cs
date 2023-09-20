using System.Reflection;
using Metalama.Patterns.Caching.Aspects;
using Metalama.Patterns.Caching.Aspects.Helpers;
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
        return _cachingService!.GetFromCacheOrExecute<int>(_cacheRegistration_M!, this, new object[] { }, Invoke);
    }
    private int M_Source() => 5;
    private static readonly CachedMethodMetadata _cacheRegistration_M;
    private ICachingService _cachingService;
    static C()
    {
        C._cacheRegistration_M = CachedMethodMetadata.Register(RunTimeHelpers.ThrowIfMissing(typeof(C).GetMethod("M", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null)!, "C.M()"), new CachedMethodConfiguration() { AbsoluteExpiration = null, AutoReload = false, IgnoreThisParameter = false, Priority = CacheItemPriority.Default, ProfileName = (string? )null, SlidingExpiration = null }, false);
    }
    public C(ICachingService? cachingService = default)
    {
        this._cachingService = cachingService ?? throw new System.ArgumentNullException(nameof(cachingService));
    }
}