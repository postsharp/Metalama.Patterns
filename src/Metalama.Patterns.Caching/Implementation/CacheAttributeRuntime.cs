// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;
using static Flashtrace.FormattedMessageBuilder;

namespace Metalama.Patterns.Caching.Implementation;

[EditorBrowsable( EditorBrowsableState.Never )]
public static class CacheAttributeRuntime
{
    public static TResult OverrideMethod<TResult>( CachedMethodRegistration registration, object? instance, object?[] args )
    {
        var logSource = registration.Logger;

        object? result;

        // TODO: [Porting] Discuss: We could do this string interpolation at build time, but obfuscation/IL-rewriting could change the method signature before runtime. Best practice?
        using ( var activity = logSource.Default.OpenActivity( Formatted( "Processing invocation of method {Method}", registration.Method ) ) )
        {
            try
            {
                var mergedConfiguration = registration.MergedConfiguration;

                if ( !mergedConfiguration.IsEnabled.GetValueOrDefault() )
                {
                    logSource.Debug.EnabledOrNull?.Write( Formatted( "Ignoring the caching aspect because caching is disabled for this profile." ) );

                    result = registration.InvokeOriginalMethod( instance, args );
                }
                else
                {
                    var methodKey = CachingServices.DefaultKeyBuilder.BuildMethodKey(
                        registration,
                        args,
                        instance );

                    logSource.Debug.EnabledOrNull?.Write( Formatted( "Key=\"{Key}\".", methodKey ) );

                    // TODO: [Porting] Use ( delegate, TArgs ) pattern to avoid delegate creation on each call.

                    result = CachingFrontend.GetOrAdd(
                        registration.Method,
                        methodKey,
                        registration.Method.ReturnType,
                        mergedConfiguration,
                        registration.InvokeOriginalMethod,
                        instance,
                        args,
                        logSource );
                }

                activity.SetSuccess();
            }
            catch ( Exception e )
            {
                activity.SetException( e );
                throw;
            }
        }

        if ( registration.ReturnValueCanBeNull )
        {
            return (TResult) result;
        }
        else
        {
            return result == null ? default : (TResult) result;
        }
    }
}
