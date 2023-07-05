using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Metalama.Patterns.Caching.Tests
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
