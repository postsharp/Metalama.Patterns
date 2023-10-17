// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

/* TODO: Detect invalid constructs, including:
 * - Reference to non-INPC property (also INPC property with [IgnoreAutoChangeNotification]), unless the property has [SafeForDependencyAnalysis].
 */

[CompileTime]
internal static partial class DependencyGraph
{
    /* Dependency Graph Structure
     * --------------------------
     *
     * The graph is conceptually rooted in the target type, although the root node has no associated symbol and just acts as
     * a holder of child nodes.
     *
     * Immediate children of the root node are the members of the target type (and base types) that have been encountered during analysis.
     * During initial development, those members are limited to being properties. In the future, fields and methods would also be supported.
     *
     * Children of the root node which represent properties (and fields?) of INPC types may have descendants. These descendants
     * represent chains of INPC property access (such as A.B.C) that were encountered during analysis. In the classic strategy, this information is used to
     * create the UpdateABC methods and maintain subscriptions to the chain of INPC properties.
     *
     * For each node in the graph (other than the root node), the ReferencedBy collection indicates which members of the target type reference
     * that node. This means that ReferencedBy only contains nodes that appear as immediate children of the root node. This information is used
     * to determine which properties are affected by a change to a given node.
     */

    public static Node GetDependencyGraph(
        INamedType type,
        IGraphBuildingContext context,
        Action<string>? trace = null,
        CancellationToken cancellationToken = default )
    {
        var tree = new Node();

        foreach ( var p in type.Properties )
        {
            var propertySymbol = p.GetSymbol();

            if ( propertySymbol == null )
            {
                continue;
            }

            AddReferencedProperties(
                type.Compilation,
                tree,
                propertySymbol,
                context,
                trace,
                cancellationToken );
        }

        return tree;
    }
}