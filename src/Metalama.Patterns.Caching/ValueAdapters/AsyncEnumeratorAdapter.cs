// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.RunTime;

namespace Metalama.Patterns.Caching.ValueAdapters;

#if NETCOREAPP3_0_OR_GREATER

internal sealed class AsyncEnumeratorAdapter<T> : ValueAdapter<AsyncEnumeratorExposedValue<T>>
{
    public override async Task<object?> GetStoredValueAsync( AsyncEnumeratorExposedValue<T>? value, CancellationToken cancellationToken )
    {
        if ( value == null )
        {
            return null;
        }
        
        var list = new AsyncEnumerableList<T>();
        var enumerator = value.Enumerator!;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            if ( value.MoveNextResult )
            {
                list.Add( enumerator.Current );

                while ( await enumerator.MoveNextAsync() )
                {
                    list.Add( enumerator.Current );

                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
        finally
        {
            await enumerator.DisposeAsync();
        }

        return list;
    }

    public override AsyncEnumeratorExposedValue<T>? GetExposedValue( object? storedValue )
    {
        return storedValue == null
            ? null
            : AsyncEnumeratorExposedValue.Create( (AsyncEnumerableList<T>) storedValue );
    }

    public override object? GetStoredValue( AsyncEnumeratorExposedValue<T>? value )
    {
        throw new NotImplementedException();
    }
}

#endif