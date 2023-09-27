// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/* TODO: Detect invalid constructs, including:
 * - Reference to non-INPC property (also INPC property with [IgnoreAutoChangeNotification]), unless the property has [SafeForDependencyAnalysis].
 */

[CompileTime]
internal interface IParentedTreeNode<TNode>
{
    bool IsRoot { get; }

    TNode Parent { get; }

    IEnumerable<TNode> Children { get; }
}
