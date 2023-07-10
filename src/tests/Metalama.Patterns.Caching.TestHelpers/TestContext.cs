// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Metalama.Patterns.Caching.TestHelpers
{
    public class TestContext
    {
        public IDictionary Properties { get; private set; }

        public TestContext()
        {
            this.Properties = new Hashtable();
        }
    }
}