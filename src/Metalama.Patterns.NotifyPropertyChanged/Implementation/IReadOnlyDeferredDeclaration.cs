// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal interface IReadOnlyDeferredDeclaration<T> : IReadOnlyUncertainDeferredDeclaration<T>
    where T : class, IDeclaration
{
    new bool WillBeDefined { get; }
}
