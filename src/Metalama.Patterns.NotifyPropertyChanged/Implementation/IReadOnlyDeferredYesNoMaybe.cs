// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal interface IReadOnlyDeferredYesNoMaybe<T>
    where T : class
{
    /// <summary>
    /// Gets a value indicating whether the future final state of <see cref="Value"/>. <see langword="null"/> 
    /// indicates that the value may or may not be defined.
    /// </summary>
    bool? WillBeDefined { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Value"/> setter has been successfully invoked. The
    /// set value may be <see langword="null"/> if allowed by <see cref="WillBeDefined"/>.
    /// </summary>
    bool ValueIsSet { get; }

    /// <summary>
    /// Gets the value. Code that expects to execute before the value will be set can make preliminary 
    /// decisions according to <see cref="WillBeDefined"/>. Only get the value of <see cref="Value"/>
    /// from code is known to execute after the value should have been set.
    /// </summary>
    T? Value { get; }
}