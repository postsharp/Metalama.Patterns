﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

namespace Metalama.Patterns.Observability.Implementation.DependencyAnalysis;

[CompileTime]
internal static class DependencyNodeExtensions
{
    public static Exception NewNotSupportedOnRootNodeException( [CallerMemberName] string? callerMemberName = null )
        => new InvalidOperationException( $"The operation is not supported on a root node ({callerMemberName})." );

    public static IEnumerable<T> DescendantsDepthFirst<T>( this T node )
        where T : DependencyReferenceNode
        => DescendantsDepthFirst( node, false );

    public static IEnumerable<T> SelfAndDescendantsDepthFirst<T>( this T node )
        where T : DependencyReferenceNode
        => DescendantsDepthFirst( node, true );

    private static IEnumerable<T> DescendantsDepthFirst<T>( this T node, bool includeSelf )
        where T : DependencyReferenceNode
    {
        // NB: No loop detection.

        if ( includeSelf )
        {
            yield return node;
        }

        var stack = new Stack<T>( (IEnumerable<T>) node.Children );

        while ( stack.Count > 0 )
        {
            var current = stack.Pop();

            yield return current;

            if ( current.HasChildren )
            {
                foreach ( var child in current.Children )
                {
                    stack.Push( (T) child );
                }
            }
        }
    }

    /// <summary>
    /// Gets the ancestors of the current node in leaf-to-root order.
    /// </summary>
    public static IEnumerable<T> Ancestors<T>( this T node, bool includeRoot = false )
        where T : DependencyReferenceNode
        => AncestorsCore( node, includeRoot, false );

    /// <summary>
    /// Gets the current node and its ancestors in leaf-to-root order.
    /// </summary>
    /// <param name="includeRoot"></param>
    /// <returns></returns>
    public static IEnumerable<T> AncestorsAndSelf<T>( this T node, bool includeRoot = false )
        where T : DependencyReferenceNode
        => AncestorsCore( node, includeRoot, true );

    private static IEnumerable<T> AncestorsCore<T>( T node, bool includeRoot, bool includeSelf )
        where T : DependencyReferenceNode
    {
        if ( includeSelf )
        {
            if ( !node.IsRoot || includeRoot )
            {
                yield return node;
            }
        }

        while ( !node.IsRoot )
        {
            node = (T) node.Parent!;

            if ( !includeRoot && node.IsRoot )
            {
                break;
            }

            yield return node;
        }
    }

    public static T AncestorOrSelfAtDepth<T>( this T node, int depth )
        where T : DependencyReferenceNode
    {
        if ( depth > node.Depth || depth < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof(depth), "Must be greater than zero and less than or equal to the depth of the current node." );
        }

        while ( node.Depth != depth )
        {
            node = (T) node.Parent!;
        }

        return node;
    }
}