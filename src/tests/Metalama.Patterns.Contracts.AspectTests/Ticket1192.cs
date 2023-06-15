// @Skipped(Not a Contracts concern)

/* Ported from PostSharp.
 * 
 * Old PS issue 1192, addressed "It seems there's a bug in inheritance of parameter-level aspects accross assemblies."
 * 
 * Action: Skipping, not a Contracts concern.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Metalama.Patterns.Contracts;

namespace PostSharp.Patterns.Model.BuildTests.Contracts
{
    namespace Ticket1192
    {
        public static class Program
        {
            public static int Main()
            {
                return 0;
            }
        }

        public class ManagedXoomTypeCollection : XoomTypeCollection, ICollection<XoomManagedType>
        {
            public override bool Add(XoomType item)
            {
                return false;
            }

            public IEnumerator<XoomManagedType> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void Add( XoomManagedType item )
            {
                throw new NotImplementedException();
            }

            public bool Contains( XoomManagedType item )
            {
                throw new NotImplementedException();
            }

            public void CopyTo( XoomManagedType[] array, int arrayIndex )
            {
                throw new NotImplementedException();
            }

            public bool Remove( XoomManagedType item )
            {
                throw new NotImplementedException();
            }
        }

    }
}
