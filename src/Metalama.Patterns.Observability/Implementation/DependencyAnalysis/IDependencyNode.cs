// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

// ReSharper disable once RedundantTypeDeclarationBody
[CompileTime]
internal interface IDependencyNode<out T>
    where T : IDependencyNode<T>
{
    bool HasChildren { get; }

    IReadOnlyCollection<T> Children { get; }

    /// <summary>
    /// Gets the depth of a tree node. The root node has depth zero, the children of the root node have depth 1, and so on.
    /// </summary>
    int Depth { get; }

    bool IsRoot { get; }

    T Parent { get; }

    bool HasReferencedBy { get; }

    /// <summary>
    /// Gets the direct references to the current node. Indirect references are not included.
    /// </summary>
    IReadOnlyCollection<T> ReferencedBy { get; }
}

[CompileTime]
internal interface IDependencyNode : IDependencyNode<IDependencyNode> { }