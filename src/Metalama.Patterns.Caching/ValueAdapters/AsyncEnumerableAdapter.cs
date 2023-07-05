// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.RunTime;

namespace Metalama.Patterns.Caching.ValueAdapters;

#if NETCOREAPP3_0_OR_GREATER

internal sealed class AsyncEnumerableAdapter<T> : ValueAdapter<IAsyncEnumerable<T>>
{
    public override bool IsAsyncSupported => true;

    public override async Task<object?> GetStoredValueAsync( IAsyncEnumerable<T>? value, CancellationToken cancellationToken )
    {
        if ( value == null )
        {
            return null;
        }

        return await value.BufferAsync( cancellationToken );
    }

    public override IAsyncEnumerable<T>? GetExposedValue( object? storedValue )
    {
        return storedValue == null
            ? null
            : (IAsyncEnumerable<T>) storedValue;
    }

    public override object? GetStoredValue( IAsyncEnumerable<T>? value )
    {
        throw new NotSupportedException();
    }
}

#endif