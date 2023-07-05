using System;
using System.Collections.Generic;
using System.Text;

namespace Metalama.Patterns.Caching.TestHelpers
{
    [Serializable]
    public class CachedValueChildClass : CachedValueClass
    {
        public CachedValueChildClass() : base()
        {
        }

        public CachedValueChildClass( int id ) : base( id )
        {
        }
    }
}
