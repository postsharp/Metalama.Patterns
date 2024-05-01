// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Microsoft.CodeAnalysis;
using System.Reflection.PortableExecutable;
using System.Text;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal class DependencyReferenceNode
{
    private string? _dottedPropertyPath;
    private string? _contiguousPropertyPath;
    private Dictionary<IFieldOrProperty, DependencyReferenceNode>? _childReferences;

    public DependencyReferenceNode( DependencyPropertyNode referencedPropertyNode, DependencyReferenceNode? parent, DependencyGraphBuilder builder )
    {
        this.ReferencedPropertyNode = referencedPropertyNode;
        this.Parent = parent;
        this.Builder = builder;
        this.Depth = parent == null ? 0 : parent.Depth + 1;
        referencedPropertyNode.DeclaringTypeNode.AddReference( this );
    }

    public bool IsRoot => this.Depth == 0;

    public int Depth { get; }

    public bool HasChildren => this._childReferences != null;

    public IEnumerable<DependencyReferenceNode> Children
        => (IReadOnlyCollection<DependencyReferenceNode>?) this._childReferences?.Values ?? Array.Empty<DependencyReferenceNode>();

    public DependencyPropertyNode ReferencedPropertyNode { get; }

    /// <summary>
    /// Gets the Metalama <see cref="IFieldOrProperty"/> for the node. Use <see cref="Symbol"/> for the Roslyn equivalent.
    /// </summary>
    public IFieldOrProperty FieldOrProperty => this.ReferencedPropertyNode.FieldOrProperty;

    public DependencyReferenceNode? Parent { get; }

    public DependencyGraphBuilder Builder { get; }

    private HashSet<DependencyPropertyNode>? _referencingProperties;

    public IEnumerable<DependencyPropertyNode> ReferencingProperties
        => (IEnumerable<DependencyPropertyNode>?) this._referencingProperties ?? Array.Empty<DependencyPropertyNode>();

    public bool HasReferencingProperties => this._referencingProperties != null;

    public void AddReferencingProperty( DependencyPropertyNode reference )
    {
        this._referencingProperties ??= new HashSet<DependencyPropertyNode>();
        this._referencingProperties.Add( reference );
    }

    /// <summary>
    /// Gets a property path like "A1" or "A1.B1".
    /// </summary>
    public string DottedPropertyPath
        => this._dottedPropertyPath ??=
            this.Parent == null
                ? this.FieldOrProperty.Name
                : $"{this.Parent.DottedPropertyPath}.{this.FieldOrProperty.Name}";

    /// <summary>
    /// Gets a property path like "A1" or "A1B1".
    /// </summary>
    public string ContiguousPropertyPath
        => this._contiguousPropertyPath ??=
            this.Parent == null
                ? this.FieldOrProperty.Name
                : $"{this.Parent.ContiguousPropertyPath}.{this.FieldOrProperty.Name}";

    public override string ToString() => this.DottedPropertyPath;

    public DependencyReferenceNode GetOrAddChildReference( DependencyPropertyNode propertyNode )
    {
        this._childReferences ??= new Dictionary<IFieldOrProperty, DependencyReferenceNode>();

        if ( !this._childReferences.TryGetValue( propertyNode.FieldOrProperty, out var childReference ) )
        {
            childReference = this.Builder.CreateReferenceNode( propertyNode, this );
            this._childReferences.Add( propertyNode.FieldOrProperty, childReference );
        }

        return childReference;
    }

    public void ToString( StringBuilder appendTo, int indent, Func<DependencyReferenceNode, bool>? shouldHighlight = null )
    {
        if ( shouldHighlight != null && shouldHighlight( this ) )
        {
            appendTo.Append( ' ', indent - 2 ).Append( "* " );
        }
        else
        {
            appendTo.Append( ' ', indent );
        }

        appendTo.Append( this.FieldOrProperty.Name );

        var allRefs = this.GetAllReferencingProperties();

        if ( allRefs.Count > 0 )
        {
            appendTo.Append( " [ " )
                .Append( string.Join( ", ", allRefs.OrderBy( b => b.Name ).Select( n => n.Name ).OrderBy( n => n ) ) )
                .Append( " ]" );
        }

        appendTo.AppendLine();

        if ( this._childReferences != null )
        {
            foreach ( var child in this._childReferences.Values.OrderBy( x => x.FieldOrProperty.Name ) )
            {
                child.ToString( appendTo, indent + 2, shouldHighlight );
            }
        }
    }
    
      /// <summary>
    /// Gets the distinct set of nodes which directly or indirectly reference the current node, and optionally also those which directly
    /// or indirectly reference selected children of the current node.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="node"></param>
    /// <param name="shouldIncludeImmediateChild">
    /// A predicate that selects the children of <paramref name="node"/> to which references should also be included and followed.
    /// If <see langword="null"/>, references to immediate children are not included. Note that references to <paramref name="node"/>
    /// itself are always included and followed, regardless of <paramref name="shouldIncludeImmediateChild"/>.
    /// </param>
    /// <returns></returns>
    public IReadOnlyCollection<DependencyPropertyNode> GetAllReferencingProperties(
        Func<DependencyReferenceNode, bool>? shouldIncludeImmediateChild = null )
    {
        // TODO: This algorithm is naive, and will cause repeated work if GetAllReferences() is called on one of the nodes already visited.
        // However, it's not recursive so there's no risk of stack overflow. So safe, but potentially slow.

        if ( !this.HasReferencingProperties && shouldIncludeImmediateChild == null )
        {
            return Array.Empty<DependencyPropertyNode>();
        }

        var properties = shouldIncludeImmediateChild == null
            ? this.ReferencingProperties
            : this.Children.Where( shouldIncludeImmediateChild )
                .SelectMany( n => n.ReferencingProperties )
                .Concat( this.ReferencingProperties );

        var propertiesToFollow = new Stack<DependencyPropertyNode>( properties );
        var analyzedProperties = new HashSet<DependencyPropertyNode>();

        while ( propertiesToFollow.Count > 0 )
        {
            var property = propertiesToFollow.Pop();

            if ( analyzedProperties.Add( property ) )
            {
                foreach ( var referencingProperty in property.RootReferenceNode.ReferencingProperties )
                {
                    propertiesToFollow.Push( referencingProperty );
                }
            }
        }

        return analyzedProperties;
    }
}