// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;

internal static partial class DependencyGraph
{
    [CompileTime]
    private interface IGraphBuildingNode
    {
        Node GetOrAddChild( ISymbol childSymbol );

        void AddReferencedBy( Node node );
    }
}