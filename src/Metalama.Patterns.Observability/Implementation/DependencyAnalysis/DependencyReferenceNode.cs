// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Text;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

/// <summary>
/// Represents a reference to a property in an observability dependency graph.
/// Refererences can be in the recursive form <c>A.B.C.D</c>. In this case, <c>A</c> is said to be the root
/// of the reference path, <c>B</c> is the parent of <c>C</c>, <c>C</c> is the child of <c>B</c>.
/// </summary>
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
    }

    public bool IsRoot => this.Depth == 0;

    public DependencyReferenceNode Root
    {
        get
        {
            for ( var node = this; node != null; node = node.Parent )
            {
                if ( node.IsRoot )
                {
                    return node;
                }
            }

            throw new ObservabilityAssertionFailedException();
        }
    }

    /// <summary>
    /// Gets the depth of the current element in the dependency path, where the root has a depth of <c>0</c>.
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// Gets a value indicating whether the current node has direct children.
    /// </summary>
    public bool HasChildren => this._childReferences != null;

    /// <summary>
    /// Gets the collection of direct children.
    /// </summary>
    public IEnumerable<DependencyReferenceNode> Children
        => (IReadOnlyCollection<DependencyReferenceNode>?) this._childReferences?.Values ?? Array.Empty<DependencyReferenceNode>();

    /// <summary>
    /// Gets the node for the referenced property.
    /// </summary>
    public DependencyPropertyNode ReferencedPropertyNode { get; }

    /// <summary>
    /// Gets the Metalama <see cref="IFieldOrProperty"/> for the node. 
    /// </summary>
    public IFieldOrProperty ReferencedFieldOrProperty => this.ReferencedPropertyNode.FieldOrProperty;

    /// <summary>
    /// Gets the parent node. In <c>A.B.C</c>, <c>A</c> is the parent of <c>B</c>.
    /// </summary>
    public DependencyReferenceNode? Parent { get; }

    public DependencyGraphBuilder Builder { get; }

    private HashSet<DependencyPropertyNode>? _leafReferencingProperties;

    /// <summary>
    /// Gets the list of properties referencing the current node as a leaf. For instance, if the current node path is <c>A.B</c> and we have two properties
    /// <c>P1 => A.B</c> and <c>P2 => A.B.C</c>, this property would only return <c>P1</c>.
    /// </summary>
    public IEnumerable<DependencyPropertyNode> LeafReferencingProperties
        => (IEnumerable<DependencyPropertyNode>?) this._leafReferencingProperties ?? Array.Empty<DependencyPropertyNode>();

    /// <summary>
    /// Gets a value indicating whether the current node is referenced by some property as a leaf of a property path, not as an intermediate node.
    /// </summary>
    public bool HasLeafReferencingProperties => this._leafReferencingProperties != null;

    /// <summary>
    /// Gets a value indicating whether this node, or any descedent, is referenced from some property.
    /// </summary>
    public bool HasAnyReferencingProperties { get; private set; }

    /// <summary>
    /// Adds a property referencing the current node as a leaf.
    /// </summary>
    public void AddLeafReferencingProperty( DependencyPropertyNode referencingProperty )
    {
        this._leafReferencingProperties ??= new HashSet<DependencyPropertyNode>();

        if ( !this._leafReferencingProperties.Add( referencingProperty ) )
        {
            // Duplicate.
            return;
        }

        referencingProperty.DeclaringTypeNode.AddReference( this );

        for ( var node = this; node is { HasAnyReferencingProperties: false }; node = node.Parent )
        {
            node.HasAnyReferencingProperties = true;
        }
    }

    /// <summary>
    /// Gets a property path like "A1" or "A1.B1".
    /// </summary>
    public string DottedPropertyPath
        => this._dottedPropertyPath ??=
            this.Parent == null
                ? this.ReferencedFieldOrProperty.Name
                : $"{this.Parent.DottedPropertyPath}.{this.ReferencedFieldOrProperty.Name}";

    /// <summary>
    /// Gets a property path like "A1" or "A1B1".
    /// </summary>
    public string ContiguousPropertyPath
        => this._contiguousPropertyPath ??=
            this.Parent == null
                ? this.ReferencedFieldOrProperty.Name
                : $"{this.Parent.ContiguousPropertyPath}{this.ReferencedFieldOrProperty.Name}";

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

        appendTo.Append( this.ReferencedFieldOrProperty.Name );

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
            foreach ( var child in this._childReferences.Values.OrderBy( x => x.ReferencedFieldOrProperty.Name ) )
            {
                child.ToString( appendTo, indent + 2, shouldHighlight );
            }
        }
    }

    public IReadOnlyCollection<DependencyPropertyNode> GetAllReferencingProperties( Func<DependencyReferenceNode, bool>? shouldIncludeImmediateChild = null )
        => this.GetAllReferencingProperties<DependencyPropertyNode>( shouldIncludeImmediateChild );

    /// <summary>
    /// Gets the distinct set of nodes which directly or indirectly reference the current node, and optionally also those which directly
    /// or indirectly reference selected children of the current node.
    /// </summary>
    /// <param name="shouldIncludeImmediateChild">
    /// A predicate that selects the children of the current to which references should also be included and followed.
    /// If <see langword="null"/>, references to immediate children are not included. Note that references to the current node
    /// itself are always included and followed, regardless of <paramref name="shouldIncludeImmediateChild"/>.
    /// </param>
    /// <returns></returns>
    public IReadOnlyCollection<T> GetAllReferencingProperties<T>( Func<DependencyReferenceNode, bool>? shouldIncludeImmediateChild = null )
        where T : DependencyPropertyNode
    {
        // TODO: This algorithm is naive, and will cause repeated work if GetAllReferences() is called on one of the nodes already visited.
        // However, it's not recursive so there's no risk of stack overflow. So safe, but potentially slow.

        if ( !this.HasAnyReferencingProperties && shouldIncludeImmediateChild == null )
        {
            return Array.Empty<T>();
        }

        var properties = shouldIncludeImmediateChild == null
            ? this.LeafReferencingProperties
            : this.Children.Where( shouldIncludeImmediateChild )
                .SelectMany( n => n.LeafReferencingProperties )
                .Concat( this.LeafReferencingProperties );

        var propertiesToFollow = new Stack<DependencyPropertyNode>( properties );
        var analyzedProperties = new HashSet<T>();

        while ( propertiesToFollow.Count > 0 )
        {
            var property = propertiesToFollow.Pop();

            if ( analyzedProperties.Add( (T) property ) )
            {
                foreach ( var referencingProperty in property.RootReferenceNode.LeafReferencingProperties )
                {
                    propertiesToFollow.Push( referencingProperty );
                }
            }
        }

        return analyzedProperties;
    }
}