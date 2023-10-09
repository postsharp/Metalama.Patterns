// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts;

public sealed class InvariantSuspensionCounter : IDisposable
{
    private int _value;

    public void Dispose()
    {
        Interlocked.Decrement( ref this._value );
    }

    public void Increment()
    {
        Interlocked.Increment( ref this._value );
    }

    public bool AreInvariantsSuspended => this._value > 0;
}