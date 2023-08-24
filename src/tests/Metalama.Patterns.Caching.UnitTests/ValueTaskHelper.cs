// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Tests
{
    internal static class ValueTaskHelper
    {
        public static void Wait( this ValueTask task) => task.AsTask().Wait();
    }
}