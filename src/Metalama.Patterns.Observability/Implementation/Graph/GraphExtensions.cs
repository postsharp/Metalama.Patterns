// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

namespace Metalama.Patterns.Observability.Implementation.Graph;

[CompileTime]
internal static class GraphExtensions
{
    public static Exception NewNotSupportedOnRootNodeException( [CallerMemberName] string? callerMemberName = null )
        => new InvalidOperationException( $"The operation is not supported on a root node ({callerMemberName})." );

    public static IEnumerable<T> DescendantsDepthFirst<T>( this T node )
        where T : IHasChildren<T>
        => DescendantsDepthFirst( node, false );

    public static IEnumerable<T> SelfAndDescendantsDepthFirst<T>( this T node )
        where T : IHasChildren<T>
        => DescendantsDepthFirst( node, true );

    private static IEnumerable<T> DescendantsDepthFirst<T>( this T node, bool includeSelf )
        where T : IHasChildren<T>
    {
        // NB: No loop detection.

        if ( includeSelf )
        {
            yield return node;
        }

        var stack = new Stack<T>( node.Children );

        while ( stack.Count > 0 )
        {
            var current = stack.Pop();

            yield return current;

            if ( current.HasChildren )
            {
                foreach ( var child in current.Children )
                {
                    stack.Push( child );
                }
            }
        }
    }

    /// <summary>
    /// Gets the ancestors of the current node in leaf-to-root order.
    /// </summary>
    public static IEnumerable<T> Ancestors<T>( this T node, bool includeRoot = false )
        where T : IHasParent<T>
        => AncestorsCore( node, includeRoot, false );

    /// <summary>
    /// Gets the current node and its ancestors in leaf-to-root order.
    /// </summary>
    /// <param name="includeRoot"></param>
    /// <returns></returns>
    public static IEnumerable<T> AncestorsAndSelf<T>( this T node, bool includeRoot = false )
        where T : IHasParent<T>
        => AncestorsCore( node, includeRoot, true );

    private static IEnumerable<T> AncestorsCore<T>( T node, bool includeRoot, bool includeSelf )
        where T : IHasParent<T>
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
            node = node.Parent;

            if ( !includeRoot && node.IsRoot )
            {
                break;
            }

            yield return node;
        }
    }

    public static T AncestorOrSelfAtDepth<T>( this T node, int depth )
        where T : IHasParent<T>, IHasDepth
    {
        if ( depth > node.Depth || depth < 0 )
        {
            throw new ArgumentOutOfRangeException( nameof(depth), "Must be greater than zero and less than or equal to the depth of the current node." );
        }

        while ( node.Depth != depth )
        {
            node = node.Parent;
        }

        return node;
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
    public static IReadOnlyCollection<T> AllReferencedBy<T>( this T node, Func<T, bool>? shouldIncludeImmediateChild = null )
        where T : IHasReferencedBy<T>, IHasChildren<T>
    {
        // TODO: This algorithm is naive, and will cause repeated work if GetAllReferences() is called on one of the nodes already visited.
        // However, it's not recursive so there's no risk of stack overflow. So safe, but potentially slow.

        if ( !node.HasReferencedBy && shouldIncludeImmediateChild == null )
        {
            return Array.Empty<T>();
        }

        var refsToFollow = new Stack<T>(
            shouldIncludeImmediateChild == null
                ? node.ReferencedBy
                : node.Children.Where( shouldIncludeImmediateChild ).SelectMany( n => n.ReferencedBy ).Concat( node.ReferencedBy ) );

        var refsFollowed = new HashSet<T>();

        while ( refsToFollow.Count > 0 )
        {
            var r = refsToFollow.Pop();

            if ( refsFollowed.Add( r ) )
            {
                if ( r.HasReferencedBy )
                {
                    foreach ( var indirectRef in r.ReferencedBy )
                    {
                        refsToFollow.Push( indirectRef );
                    }
                }
            }
        }

        return refsFollowed;
    }
}