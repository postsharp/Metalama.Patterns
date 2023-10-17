// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Implementation;

/// <summary>
/// A value that will be defined later.
/// </summary>
[CompileTime]
internal sealed class Deferred<T> : DeferredOptional<T>, IReadOnlyDeferred<T>
    where T : class
{
    public Deferred()
        : base( true ) { }

    // ReSharper disable once UnusedMember.Global
    [Obsolete( "The value will always be true for " + nameof(Deferred<T>) + ", avoid unnecessary conditions." )]
    public new bool WillBeDefined => true;

    /// <summary>
    /// Gets or sets the value. Code that expects to execute before the value will be set can assume
    /// that the value will be set later. Only get the actual value of <see cref="Value"/> from code that is known 
    /// to execute after the value should have been set.
    /// </summary>
    public new T Value
    {
        get => base.Value!;
        set => base.Value = value;
    }
}