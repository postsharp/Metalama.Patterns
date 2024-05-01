// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal class ClassicDependencyPropertyNode : DependencyPropertyNode
{
    public ClassicDependencyPropertyNode(
        IFieldOrProperty fieldOrProperty,
        DependencyTypeNode declaringTypeNode,
        ClassicProcessingNodeInitializationContext ctx ) : base( fieldOrProperty, declaringTypeNode ) { }

    public new ClassicDependencyReferenceNode RootReferenceNode => (ClassicDependencyReferenceNode) base.RootReferenceNode;
}