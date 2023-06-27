// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections;

namespace Metalama.Patterns.Caching.ValueAdapters
{
    internal sealed class EnumeratorAdapter<T> : ValueAdapter<IEnumerator<T>>
    {
        public override object GetStoredValue( IEnumerator<T> value )
        {
            List<T> list = new();

            while ( value.MoveNext() )
            {
                list.Add( value.Current );
            }

            return list;
        }

        public override IEnumerator<T> GetExposedValue( object storedValue )
        {
            return new Enumerator( (List<T>) storedValue );
        }

        private class Enumerator : IEnumerator<T>
        {
            private int index = -1;
            private readonly List<T> list;

            public Enumerator( List<T> list )
            {
                this.list = list;
            }

            public T Current
            {
                get
                {
                    if ( this.index < 0 || this.list.Count <= this.index )
                    {
                        throw new InvalidOperationException();
                    }

                    return this.list[this.index];
                }
            }

            object IEnumerator.Current => this.Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                this.index++;

                return this.list.Count > this.index;
            }

            public void Reset()
            {
                throw new NotSupportedException( "Cannot reset a cached enumerator." );
            }
        }
    }
}