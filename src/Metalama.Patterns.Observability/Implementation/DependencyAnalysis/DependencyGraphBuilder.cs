﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Engine.CodeModel;

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

/* TODO: Detect invalid constructs, including:
 * - Reference to non-INPC property (also INPC property with [IgnoreAutoChangeNotification]), unless the property has [IgnoreUnobservableExpressions].
 */

[CompileTime]
internal partial class DependencyGraphBuilder
{
    private readonly Dictionary<INamedType, ObservableTypeInfo> _types = new();

    protected virtual ObservableTypeInfo CreateTypeInfo( INamedType type ) => new( this, type );

    public virtual ObservablePropertyInfo CreatePropertyInfo( IFieldOrProperty fieldOrProperty, ObservableTypeInfo parent ) => new( fieldOrProperty, parent );

    public virtual ObservableExpression CreateExpression( ObservablePropertyInfo propertyInfo, ObservableExpression? parent )
        => new( propertyInfo, parent, this );

    private ObservableTypeInfo GetOrAddTypeNode( INamedType type )
    {
        if ( !this._types.TryGetValue( type, out var typeNode ) )
        {
            typeNode = this.CreateTypeInfo( type );
            this._types.Add( type, typeNode );
        }

        return typeNode;
    }

    private ObservablePropertyInfo GetOrAddPropertyNode( IFieldOrProperty property )
    {
        var declaringTypeNode = this.GetOrAddTypeNode( property.DeclaringType );

        return declaringTypeNode.GetOrAddProperty( property );
    }

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

    public ObservableTypeInfo GetDependencyGraph(
        INamedType type,
        GraphBuildingContext context,
        Action<string>? trace = null,
        CancellationToken cancellationToken = default )
    {
        var graphType = this.GetOrAddTypeNode( type );

        foreach ( var p in type.Properties )
        {
            var propertySymbol = p.GetSymbol();

            if ( propertySymbol == null )
            {
                continue;
            }

            var graphMember = graphType.GetOrAddProperty( (IFieldOrProperty) type.Compilation.GetDeclaration( propertySymbol ) );

            AddReferencedProperties(
                type.Compilation,
                graphMember,
                context,
                trace,
                cancellationToken );
        }

        return graphType;
    }
}