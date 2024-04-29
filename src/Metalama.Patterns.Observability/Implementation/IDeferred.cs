// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

// ReSharper disable UnusedMemberInSuper.Global

namespace Metalama.Patterns.Observability.Implementation;

/// <summary>
/// A value that may be defined later.
/// </summary>
[CompileTime]
internal interface IDeferred<out T>
{
    /// <summary>
    /// Gets a value indicating whether the <see cref="Value"/> setter has been successfully invoked. 
    /// </summary>
    bool HasBeenSet { get; }

    /// <summary>
    /// Gets the value. Throws <see cref="InvalidOperationException"/> if <see cref="HasBeenSet"/> is <c>false</c>.
    /// </summary>
    T Value { get; }
}