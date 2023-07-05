// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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
            foreach ( var component in components )
            {
                component.Dispose();
            }

            foreach ( var component in components )
            {
                AssertEx.Equal( 0, component.BackgroundTaskExceptions, "Exceptions occurred when executing background tasks." );
            }
        }

        public static async Task DisposeAsync<ComponentT>( params ComponentT[] components )
            where ComponentT : ITestableCachingComponent
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