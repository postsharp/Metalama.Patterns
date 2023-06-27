// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching.ValueAdapters
{
    internal sealed class EnumerableAdapter<T> : ValueAdapter<IEnumerable<T>>
    {

        public override object GetStoredValue( IEnumerable<T> value )
        {
            if (value == null)
            {
                return null;
            }

            return new List<T>(value);
        }

        public override IEnumerable<T> GetExposedValue(object storedValue)
        {
            return (IEnumerable<T>)storedValue;
        }
    }
}
