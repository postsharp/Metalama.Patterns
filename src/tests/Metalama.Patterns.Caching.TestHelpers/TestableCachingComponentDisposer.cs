// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public static class TestableCachingComponentDisposer
    {
        public static void Dispose<TComponentT>( params TComponentT[] components )
            where TComponentT : ITestableCachingComponent
        {
            foreach ( var component in components )
            {
                component.Dispose();
            }

            foreach ( var component in components )
            {
                AssertEx.Equal( 0, component.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
            }
        }

        public static async Task DisposeAsync<TComponentT>( params TComponentT[] components )
            where TComponentT : ITestableCachingComponent
        {
            foreach ( var component in components )
            {
                await component.DisposeAsync( new CancellationTokenSource( TimeSpan.FromSeconds( 10 ) ).Token );
            }

            foreach ( var component in components )
            {
                AssertEx.Equal( 0, component.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
            }
        }
    }
}