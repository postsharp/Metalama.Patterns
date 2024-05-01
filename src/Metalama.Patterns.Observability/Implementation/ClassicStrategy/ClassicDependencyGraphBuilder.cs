// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Observability.Implementation.DependencyAnalysis;
using System.Text;

namespace Metalama.Patterns.Observability.Implementation.ClassicStrategy;

[CompileTime]
internal class ClassicDependencyGraphBuilder : DependencyGraphBuilder
{
    private ClassicProcessingNodeInitializationContext _context;

    public ClassicDependencyGraphBuilder( ClassicProcessingNodeInitializationContext context )
    {
        this._context = context;
    }

    public override DependencyTypeNode CreateTypeNode( INamedType type ) => new ClassicDependencyTypeNode( this, type );

    public override DependencyPropertyNode CreatePropertyNode( IFieldOrProperty fieldOrProperty, DependencyTypeNode parent )
        => new ClassicDependencyPropertyNode( fieldOrProperty, parent, this._context );

    public override DependencyReferenceNode CreateReferenceNode(
        DependencyPropertyNode propertyNode,
        DependencyReferenceNode? parent )
        => new ClassicDependencyReferenceNode( propertyNode, parent, this, this._context );
}