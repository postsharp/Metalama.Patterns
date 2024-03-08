internal class Target
{
  [Cache]
  public string GetResourceName1(int x, int y, [NotCacheKey] int z)
  {
    static object? Invoke(object? instance, object? [] args)
    {
      return ((Target)instance).GetResourceName1_Source((int)args[0], (int)args[1], (int)args[2]);
    }
    return _cachingService!.GetFromCacheOrExecute<string>(_cacheRegistration_GetResourceName1!, this, new object[] { x, y, z }, Invoke);
  }
  private string GetResourceName1_Source(int x, int y, int z) => "resource";
  [Cache]
  public string GetResourceName2(int y, [NotCacheKey] string z, int x)
  {
    static object? Invoke(object? instance, object? [] args)
    {
      return ((Target)instance).GetResourceName2_Source((int)args[0], (string)args[1], (int)args[2]);
    }
    return _cachingService!.GetFromCacheOrExecute<string>(_cacheRegistration_GetResourceName2!, this, new object[] { y, z, x }, Invoke);
  }
  private string GetResourceName2_Source(int y, string z, int x) => "resource";
  [InvalidateCache(nameof(GetResourceName1))]
  [InvalidateCache(nameof(GetResourceName2))]
  public async Task<ProtectedResource?> UpdateProtectedResourceAsync(int x, int y, UpdateProtectedResource update)
  {
    var result = await UpdateProtectedResourceAsync_Source(x, y, update);
    await CachingServiceExtensions.InvalidateAsync(this._cachingService!, Target._methodsInvalidatedBy_UpdateProtectedResourceAsync_23BEB20FE3CE3EBDD9C65F59C43F5632![0], this, new object[] { x, y, 0 }, default(CancellationToken));
    await CachingServiceExtensions.InvalidateAsync(this._cachingService!, Target._methodsInvalidatedBy_UpdateProtectedResourceAsync_23BEB20FE3CE3EBDD9C65F59C43F5632![1], this, new object[] { y, 0, x }, default(CancellationToken));
    return result;
  }
  private async Task<ProtectedResource?> UpdateProtectedResourceAsync_Source(int x, int y, UpdateProtectedResource update)
  {
    return new();
  }
  [InvalidateCache(nameof(GetResourceName1))]
  [InvalidateCache(nameof(GetResourceName2))]
  public async Task<ProtectedResource?> UpdateProtectedResource2Async(UpdateProtectedResource update, int y, int x)
  {
    var result = await UpdateProtectedResource2Async_Source(update, y, x);
    await CachingServiceExtensions.InvalidateAsync(this._cachingService!, Target._methodsInvalidatedBy_UpdateProtectedResource2Async_EF4B99F69BA2C549913F60A9CBDD6F66![0], this, new object[] { x, y, 0 }, default(CancellationToken));
    await CachingServiceExtensions.InvalidateAsync(this._cachingService!, Target._methodsInvalidatedBy_UpdateProtectedResource2Async_EF4B99F69BA2C549913F60A9CBDD6F66![1], this, new object[] { y, 0, x }, default(CancellationToken));
    return result;
  }
  private async Task<ProtectedResource?> UpdateProtectedResource2Async_Source(UpdateProtectedResource update, int y, int x)
  {
    return new();
  }
  private static readonly CachedMethodMetadata _cacheRegistration_GetResourceName1;
  private static readonly CachedMethodMetadata _cacheRegistration_GetResourceName2;
  private ICachingService _cachingService;
  private static MethodInfo[] _methodsInvalidatedBy_UpdateProtectedResource2Async_EF4B99F69BA2C549913F60A9CBDD6F66;
  private static MethodInfo[] _methodsInvalidatedBy_UpdateProtectedResourceAsync_23BEB20FE3CE3EBDD9C65F59C43F5632;
  static Target()
  {
    Target._cacheRegistration_GetResourceName1 = CachedMethodMetadata.Register(RunTimeHelpers.ThrowIfMissing(typeof(Target).GetMethod("GetResourceName1", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(int), typeof(int) }, null)!, "Target.GetResourceName1(int, int, int)"), new CachedMethodConfiguration() { AbsoluteExpiration = null, AutoReload = null, IgnoreThisParameter = null, Priority = null, ProfileName = (string? )null, SlidingExpiration = null }, true);
    Target._cacheRegistration_GetResourceName2 = CachedMethodMetadata.Register(RunTimeHelpers.ThrowIfMissing(typeof(Target).GetMethod("GetResourceName2", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string), typeof(int) }, null)!, "Target.GetResourceName2(int, string, int)"), new CachedMethodConfiguration() { AbsoluteExpiration = null, AutoReload = null, IgnoreThisParameter = null, Priority = null, ProfileName = (string? )null, SlidingExpiration = null }, true);
    Target._methodsInvalidatedBy_UpdateProtectedResourceAsync_23BEB20FE3CE3EBDD9C65F59C43F5632 = new MethodInfo[]
    {
      RunTimeHelpers.ThrowIfMissing(typeof(Target).GetMethod("GetResourceName1", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(int), typeof(int) }, null)!, "Target.GetResourceName1(int, int, int)"),
      RunTimeHelpers.ThrowIfMissing(typeof(Target).GetMethod("GetResourceName2", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string), typeof(int) }, null)!, "Target.GetResourceName2(int, string, int)")
    };
    Target._methodsInvalidatedBy_UpdateProtectedResource2Async_EF4B99F69BA2C549913F60A9CBDD6F66 = new MethodInfo[]
    {
      RunTimeHelpers.ThrowIfMissing(typeof(Target).GetMethod("GetResourceName1", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(int), typeof(int) }, null)!, "Target.GetResourceName1(int, int, int)"),
      RunTimeHelpers.ThrowIfMissing(typeof(Target).GetMethod("GetResourceName2", BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(int), typeof(string), typeof(int) }, null)!, "Target.GetResourceName2(int, string, int)")
    };
  }
  public Target(ICachingService? cachingService = default)
  {
    this._cachingService = cachingService ?? throw new System.ArgumentNullException(nameof(cachingService));
  }
}