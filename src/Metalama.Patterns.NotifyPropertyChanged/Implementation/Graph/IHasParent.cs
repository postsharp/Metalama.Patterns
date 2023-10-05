// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Graph;

[CompileTime]
internal interface IHasParent<T>
{
    bool IsRoot { get; }

    T Parent { get; }
}