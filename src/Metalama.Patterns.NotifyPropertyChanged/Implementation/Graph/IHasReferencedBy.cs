// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Graph;

[CompileTime]
internal interface IHasReferencedBy<T>
{
    bool HasReferencedBy { get; }

    /// <summary>
    /// Gets the direct references to the current node. Indirect references are not included.
    /// </summary>
    IReadOnlyCollection<T> ReferencedBy { get; }
}