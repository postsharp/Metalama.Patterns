// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal interface IGraphBuildingNode
{
    DependencyNode GetOrAddChild( ISymbol childSymbol );

    void AddReferencedBy( DependencyNode node );
}