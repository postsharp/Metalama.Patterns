// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.TestHelpers
{
    public static class TestProfileConfigurationFactory
    {
        [Obsolete( "", true )]
        public static CachingProfile CreateProfile( string name ) => CachingService.Default.Profiles[name];
    }
}