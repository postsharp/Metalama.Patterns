// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A value that may be defined later, where the potential for <see cref="WillBeDefined"/> as specified at construction can be yes or no.
/// </summary>
[CompileTime]
internal interface IReadOnlyDeferredYesNo<T> : IReadOnlyDeferredYesNoMaybe<T>
    where T : class
{
    /// <summary>
    /// Gets a value indicating whether the future final state of <see cref="IReadOnlyDeferredYesNoMaybe{T}.Value"/> will be defined (ie, not <see langword="null"/>).  
    /// </summary>
    new bool WillBeDefined { get; }
}