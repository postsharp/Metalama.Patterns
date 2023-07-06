// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: [Porting] Temporary, initial development only. Remove or adapt to proper tests.
// ReSharper disable all
#pragma warning disable

// TODO: Work around #33441 : Some method calls in scope via `using static` are not transformed.
using Metalama.Patterns.Caching.Implementation;
using System.Runtime.CompilerServices;
using static Flashtrace.FormattedMessageBuilder;
using static Metalama.Patterns.Caching.Experiments.InfoWriter;

namespace Metalama.Patterns.Caching.Experiments;

// Update: even simpler reproduction:

public sealed class TestSyncGenericCachingClass<T>
{
    [Cache]
    public T GetValue()
    {
        return default;
    }
}

// Original reproduction and hand-written DESIRED:

public sealed class TestAsyncGenericCachingClass<T>
{
    //[Cache]
    public async Task<T> GetValueAsync()
    {
        await Task.Delay( 1 );
        return default;
    }
}

#if false
public sealed class TestAsyncGenericCachingClass_DESIRED<T>
{
    static readonly CachedMethodRegistration _registration;

    static TestAsyncGenericCachingClass_DESIRED()
    {
        _registration = CachingServices.DefaultMethodRegistrationCache.Register(
            (System.Reflection.MethodInfo) typeof( TestAsyncGenericCachingClass<> ).GetMethod(
                "GetValueAsync",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance ),
            GetInvoker(),
            new CacheAttributeProperties(),
            true );
    }

    public Task<T> GetValueAsync()
    {
        return CacheAttributeRunTime.OverrideMethodAsyncTask<T>(
            _registration,
            this,
            new object[] { } );
    }

    private static Func<object?, object?[], Task<object?>> GetInvoker()
    {
        return Invoke;

        async Task<object?> Invoke( object? instance, object?[] args )
        {
            return await ((TestAsyncGenericCachingClass_DESIRED<T>)instance).GetValueAsync_Source();
        }
    }

    //[Cache]
    private async Task<T> GetValueAsync_Source()
    {
        await Task.Delay( 1 );
        return default;
    }
}
#endif