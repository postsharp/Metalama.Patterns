// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Text;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal class DependencyTypeNode
{
    private readonly Dictionary<IFieldOrProperty, DependencyPropertyNode> _properties = new();
    private readonly List<DependencyReferenceNode> _allReferences = new();

    public DependencyGraphBuilder Builder { get; }

    public INamedType Type { get; }

    public DependencyTypeNode( DependencyGraphBuilder builder, INamedType type )
    {
        this.Builder = builder;
        this.Type = type;
    }

    public override string ToString() => this.ToString( null );

    public string ToString( DependencyReferenceNode? highlightNode )
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine( "<root>" );

        foreach ( var member in this._properties.Values.OrderBy( x=>x.Name ) )
        {
            member.RootReferenceNode.ToString( stringBuilder, 2, x => x == highlightNode );
        }

        return stringBuilder.ToString();
    }

    public DependencyPropertyNode GetOrAddProperty( IFieldOrProperty fieldOrProperty )
    {
        if ( !this._properties.TryGetValue( fieldOrProperty, out var member ) )
        {
            member = this.Builder.CreatePropertyNode( fieldOrProperty, this );
            this._properties.Add( fieldOrProperty, member );
        }

        return member;
    }

    internal void AddReference( DependencyReferenceNode reference )
    {
        this._allReferences.Add( reference );
    }

    public IEnumerable<DependencyPropertyNode> Properties => this._properties.Values;

    public IReadOnlyList<DependencyReferenceNode> References => this._allReferences;
    
    
}