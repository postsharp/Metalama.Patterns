// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A declaration that will or will not be defined later. There is no maybe.
/// </summary>
/// <typeparam name="T"></typeparam>
[CompileTime]
internal class DeferredDeclaration<T> : UncertainDeferredDeclaration<T>, IReadOnlyDeferredDeclaration<T>
    where T : class, IDeclaration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeferredDeclaration{T}"/> class.
    /// </summary>
    /// <param name="willBeDefined">Indicates the future final state of <see cref="Declaration"/>.</param>
    /// <param name="mustBeSetBeforeGet"> If <see langword="true"/>, the <see cref="Declaration"/> getter will throw if the
    /// setter has not yet been called (regardless of whether the value was <see langword="null"/> or not). The default is <see langword="false"/>.
    /// Note that the <see cref="Declaration"/> getter also checks that the current value is compatible with <see cref="WillBeDefined"/>.</param>
    public DeferredDeclaration( bool willBeDefined, bool mustBeSetBeforeGet = false )
        : base( willBeDefined, mustBeSetBeforeGet ) { }

    public new bool WillBeDefined => base.WillBeDefined == true;

    /// <summary>
    /// Gets or sets the declaration. The declaration defaults to <see langword="null"/>, and can be
    /// set at most once. Code that expects to execute before the value will be set can make preliminary 
    /// decisions according to <see cref="WillBeDefined"/>.  Only get the value of <see cref="Declaration"/>
    /// from code is known to execute after the value should have been set.
    /// </summary>
    public new T? Declaration
    {
        get => base.Declaration!;
        set => base.Declaration = value;
    }
}