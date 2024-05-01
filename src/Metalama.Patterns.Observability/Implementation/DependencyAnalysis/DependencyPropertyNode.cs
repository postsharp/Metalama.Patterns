// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Text;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal class DependencyPropertyNode
{
    public DependencyTypeNode DeclaringTypeNode { get; }

    public IFieldOrProperty FieldOrProperty { get; }

    public string Name => this.FieldOrProperty.Name;

    public DependencyReferenceNode RootReferenceNode { get; }

    public DependencyPropertyNode( IFieldOrProperty fieldOrProperty, DependencyTypeNode declaringTypeNode )
    {
        this.FieldOrProperty = fieldOrProperty;
        this.DeclaringTypeNode = declaringTypeNode;
        this.RootReferenceNode = declaringTypeNode.Builder.CreateReferenceNode( this, null );
    }

    public override string ToString() => this.FieldOrProperty.ToString();
}