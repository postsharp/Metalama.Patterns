// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts.Implementation;

[RunTimeOrCompileTime]
public readonly struct TypedRangeBound<T>
{
    public T Value { get; }

    public bool IsIncluded { get; }

    public TypedRangeBound( T value, bool isIncluded )
    {
        this.Value = value;
        this.IsIncluded = isIncluded;
    }
}