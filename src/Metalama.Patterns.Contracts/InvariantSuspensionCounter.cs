// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// A class used by the <see cref="InvariantAttribute"/> aspect to count the number of times the <c>SuspendInvariants</c>
/// method has been invoked.
/// </summary>
[PublicAPI]
public sealed class InvariantSuspensionCounter : IDisposable
{
    private int _value;

    /// <summary>
    /// Decrements the counter. Note that this does not verify invariants when the counter is back to zero.
    /// </summary>
    public void Dispose()
    {
        Interlocked.Decrement( ref this._value );
    }

    /// <summary>
    /// Increments the counter.
    /// </summary>
    public void Increment()
    {
        Interlocked.Increment( ref this._value );
    }

    /// <summary>
    /// Gets a value indicating whether the verification of invariants is currently suspended.
    /// </summary>
    public bool AreInvariantsSuspended => this._value > 0;
}