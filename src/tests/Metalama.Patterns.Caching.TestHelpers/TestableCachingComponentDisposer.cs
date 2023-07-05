using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.Implementation;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.TestHelpers.Shared
{
    internal static class TestableCachingComponentDisposer
    {
        public static void Dispose<ComponentT>( params ComponentT[] components )
            where ComponentT : ITestableCachingComponent
        {
            foreach ( ComponentT component in components )
            {
                component.Dispose();
            }

            foreach ( ComponentT component in components )
            {
                AssertEx.Equal( 0, component.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
            }
        }

        public static async Task DisposeAsync<ComponentT>( params ComponentT[] components )
            where ComponentT : ITestableCachingComponent
        {
            
            foreach ( ComponentT component in components )
            {
                await component.DisposeAsync(new CancellationTokenSource(TimeSpan.FromSeconds( 10 )).Token);
            }

            foreach ( ComponentT component in components )
            {
                AssertEx.Equal( 0, component.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
            }
        }
    }
}
