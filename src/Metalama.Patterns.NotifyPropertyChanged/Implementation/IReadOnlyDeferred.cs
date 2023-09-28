// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A value that will certainly be defined later.
/// </summary>
[CompileTime]
internal interface IReadOnlyDeferred<T> : IReadOnlyDeferredYesNo<T>
    where T : class
{
    [Obsolete( "The value will always be true for " + nameof( IReadOnlyDeferred<T> ) + ", avoid unnecessary conditions." )]
    new bool WillBeDefined { get; }

    /// <summary>
    /// Gets the value. Code that expects to execute before the value will be set can assume
    /// that the value will be set later. Only get the actual value of <see cref="Value"/> from code that is known 
    /// to execute after the value should have been set.
    /// </summary>
    new T Value { get; }
}