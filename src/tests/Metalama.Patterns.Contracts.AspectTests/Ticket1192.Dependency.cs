// @Skipped(PENDING)

using System;
using System.Collections;
using System.Collections.Generic;
using Metalama.Patterns.Contracts;

namespace PostSharp.Patterns.Model.BuildTests.Contracts
{
    namespace Ticket1192
    {
      

        public class XoomType
        {
        }
        public class XoomManagedType
        {
            
        }

        public class XoomTypeCollection : ICollection<XoomType>
        {
            virtual public bool Add( [NotNull] XoomType item )
            {
                throw new NotImplementedException();
            }
            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains( XoomType item )
            {
                throw new NotImplementedException();
            }

            public void CopyTo( XoomType[] array, int arrayIndex )
            {
                throw new NotImplementedException();
            }

            public bool Remove( XoomType item )
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public IEnumerator<XoomType> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            void ICollection<XoomType>.Add( XoomType item )
            {
                throw new NotImplementedException();
            }
        }

      

    }
}
