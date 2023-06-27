// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections;

namespace Metalama.Patterns.Caching.ValueAdapters;

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

    public override IEnumerator<T> GetExposedValue( object storedValue ) => new Enumerator( (List<T>) storedValue );

    private class Enumerator : IEnumerator<T>
    {
        private int _index = -1;
        private readonly List<T> _list;

        public Enumerator( List<T> list )
        {
            this._list = list;
        }

        public T Current
        {
            get
            {
                if ( this._index < 0 || this._list.Count <= this._index )
                {
                    throw new InvalidOperationException();
                }

                return this._list[this._index];
            }
        }

        object IEnumerator.Current => this.Current;

        public void Dispose() { }

        public bool MoveNext()
        {
            this._index++;

            return this._list.Count > this._index;
        }

        public void Reset() => throw new NotSupportedException( "Cannot reset a cached enumerator." );
    }
}