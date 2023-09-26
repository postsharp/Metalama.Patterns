// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal interface IReadOnlyUncertainDeferredDeclaration<T>
    where T : class, IDeclaration
{
    /// <summary>
    /// Gets a value indicating whether the future final state of <see cref="Declaration"/>. <see langword="null"/> 
    /// indicates that the declaration may or may not be defined.
    /// </summary>
    bool? WillBeDefined { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="Declaration"/> setter has been successfully invoked.
    /// </summary>
    bool DeclarationIsSet { get; }

    /// <summary>
    /// Gets the declaration. Code that expects to execute before the value will be set can make preliminary 
    /// decisions according to <see cref="WillBeDefined"/>. Only get the value of <see cref="Declaration"/>
    /// from code is known to execute after the value should have been set.
    /// </summary>
    T? Declaration { get; }
}
