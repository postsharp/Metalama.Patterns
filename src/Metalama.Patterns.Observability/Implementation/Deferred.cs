// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Diagnostics.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation;

/// <summary>
/// A value that may be defined later.
/// </summary>
[CompileTime]
internal sealed class Deferred<T> : IDeferred<T>
{
    private T? _value;

    /// <summary>
    /// Gets a value indicating whether the <see cref="Value"/> setter has been successfully invoked.
    /// </summary>
    public bool HasBeenSet { get; private set; }

    /// <summary>
    /// Gets or sets the deferred value. Getting the property throws an <see cref="InvalidOperationException"/> if it has not been set before.
    /// </summary>
    public T Value
    {
        get
        {
            if ( !this.HasBeenSet )
            {
                throw new InvalidOperationException( nameof(this.Value) + " must be set before it can be read." );
            }

            return this._value!;
        }

        set
        {
            if ( this.HasBeenSet )
            {
                throw new InvalidOperationException( "The value has already been set, the value cannot be changed." );
            }

            this._value = value;
            this.HasBeenSet = true;
        }
    }

    public bool TryGetValue( [MaybeNullWhen( false )] out T value )
    {
        value = this._value;

        return this.HasBeenSet;
    }
}