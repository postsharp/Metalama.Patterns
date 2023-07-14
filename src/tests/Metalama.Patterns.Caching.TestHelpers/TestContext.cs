// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections;

namespace Metalama.Patterns.Caching.TestHelpers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestContext
    {
        public IDictionary Properties { get; }

        public TestContext()
        {
            this.Properties = new Hashtable();
        }
    }
}