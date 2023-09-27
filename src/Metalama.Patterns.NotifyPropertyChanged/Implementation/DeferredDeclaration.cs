// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A value that may be defined later, where the potential for <see cref="WillBeDefined"/> as specified at construction can be yes or no.
/// </summary>
/// <typeparam name="T"></typeparam>
[CompileTime]
internal class DeferredYesNo<T> : DeferredYesNoMaybe<T>, IReadOnlyDeferredYesNo<T>
    where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeferredYesNo{T}"/> class.
    /// </summary>
    /// <param name="willBeDefined">Indicates the future final state of <see cref="Value"/>.</param>
    /// <param name="mustBeSetBeforeGet"> If <see langword="true"/>, the <see cref="Value"/> getter will throw if the
    /// setter has not yet been called (regardless of whether the value was <see langword="null"/> or not). The default is <see langword="false"/>.
    /// Note that the <see cref="Value"/> getter also checks that the current value is compatible with <see cref="WillBeDefined"/>.</param>
    public DeferredYesNo( bool willBeDefined, bool mustBeSetBeforeGet = false )
        : base( willBeDefined, mustBeSetBeforeGet ) { }

    /// <summary>
    /// Gets a value indicating whether the future final state of <see cref="Value"/> will be defined (ie, not <see langword="null"/>).  
    /// </summary>
    public new bool WillBeDefined => base.WillBeDefined == true;

    /// <summary>
    /// Gets or sets the value. The value defaults to <see langword="null"/>, and can be
    /// set at most once. Code that expects to execute before the value will be set can make preliminary 
    /// decisions according to <see cref="WillBeDefined"/>.  Only get the value of <see cref="Value"/>
    /// from code is known to execute after the value should have been set.
    /// </summary>
    public new T? Value
    {
        get => base.Value!;
        set => base.Value = value;
    }
}