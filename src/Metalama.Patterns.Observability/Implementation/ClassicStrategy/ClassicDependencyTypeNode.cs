// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;
using Microsoft.CodeAnalysis;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal class ClassicDependencyTypeNode : DependencyTypeNode
{
    public ClassicDependencyTypeNode( DependencyGraphBuilder builder, INamedType type ) : base( builder, type ) { }

    public new IEnumerable<ClassicDependencyReferenceNode> AllReferences => base.AllReferences.Cast<ClassicDependencyReferenceNode>();

    public new IEnumerable<ClassicDependencyPropertyNode> Properties => base.Properties.Cast<ClassicDependencyPropertyNode>();
}