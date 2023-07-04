// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.RunTime;

namespace Metalama.Patterns.Caching.ValueAdapters;

#if NETCOREAPP3_0_OR_GREATER

public static class AsyncEnumeratorExposedValue
{
    public static AsyncEnumeratorExposedValue<T> Create<T>( bool moveNextResult, IAsyncEnumerator<T> enumerator )
        => new( moveNextResult, enumerator, null );

    public static AsyncEnumeratorExposedValue<T> Create<T>( AsyncEnumerableList<T> buffer )
        => new( false, null, buffer );
}

public sealed record AsyncEnumeratorExposedValue<T>( bool MoveNextResult, IAsyncEnumerator<T>? Enumerator, AsyncEnumerableList<T>? Buffer );

#endif