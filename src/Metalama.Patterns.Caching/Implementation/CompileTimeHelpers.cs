// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Security.Cryptography;
using System.Text;

namespace Metalama.Patterns.Caching.Implementation;

// TODO: Consider moving CompileTimeHelpers to common project and/or Metalama.Extensions
[CompileTime]
internal static class CompileTimeHelpers
{
    /// <summary>
    /// Gets a string that can be used as a C# identifier based on a stable hash of the given <see cref="SerializableDeclarationId"/>
    /// with an optional prefix.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <param name="purpose">
    /// Typically a unique hard-coded GUID corresponding to the purpose for which an identifier associated with the
    /// given ID is required. This is to avoid collisions, for example when multiple unrelated aspects introduce members associated
    /// with the same ID. Alternatively, related aspects could use a common value for <paramref name="purpose"/> if desired.
    /// </param>
    /// <param name="prefix">An optional prefix for the identifier.</param>
    /// <returns></returns>
    public static string MakeAssociatedIdentifier( this SerializableDeclarationId id, string purpose, string? prefix = null )
    {
        if ( id == null )
        {
            throw new ArgumentNullException( nameof(id) );
        }

        if ( purpose == null )
        {
            throw new ArgumentNullException( nameof(purpose) );
        }

        // TODO: !!! Don't use MD5, will throw on some platforms.
        // Not used for cryptographic purposes.
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
        var md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
        var bytes = md5.ComputeHash( Encoding.UTF8.GetBytes( id.ToString() + purpose ) );

        // TODO: !!! use base62 or something else more compact than hex.
        return prefix + "_" + BitConverter.ToString( bytes ).Replace( "-", string.Empty );
    }

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// Throws the current exception at compile time. Useful when called from a template.
    /// </summary>    
    public static void ThrowAtCompileTime( this Exception e ) => throw e;

    /// <summary>
    /// Determines if the current <see cref="IType"/> is <see cref="Task"/>, <see cref="Task{TResult}"/>, <see cref="ValueTask"/> or <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="hasResult">If <see langword="null"/>, matches types with our without <c>TResult</c>. If <see langword="false"/>, only matches
    /// types without <c>TResult</c>. If <see langword="true"/>, only matches types with <c>TResult</c>.</param>
    public static bool IsTaskOrValueTask( this IType type, bool? hasResult = default )
    {
        var unboundType = (type as INamedType)?.GetOriginalDefinition();

        if ( unboundType == null )
        {
            return false;
        }

        var isWithValue = unboundType.SpecialType is SpecialType.Task_T or SpecialType.ValueTask_T;
        var isWithoutValue = unboundType.SpecialType is SpecialType.Task or SpecialType.ValueTask;

        return hasResult switch
        {
            true => isWithValue,
            false => isWithoutValue,
            _ => isWithValue || isWithoutValue
        };
    }

    /// <summary>
    /// Determines if the current <see cref="IType"/> is <see cref="Task"/> or <see cref="Task{TResult}"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="withResult">If <see langword="null"/>, matches types with our without <c>TResult</c>. If <see langword="false"/>, only matches
    /// types without <c>TResult</c>. If <see langword="true"/>, only matches types with <c>TResult</c>.</param>
    public static bool IsTask( this IType type, bool? withResult = default )
    {
        var unboundType = (type as INamedType)?.GetOriginalDefinition();

        if ( unboundType == null )
        {
            return false;
        }

        var isWithValue = unboundType.SpecialType == SpecialType.Task_T;
        var isWithoutValue = unboundType.SpecialType == SpecialType.Task;

        return withResult switch
        {
            true => isWithValue,
            false => isWithoutValue,
            _ => isWithValue || isWithoutValue
        };
    }

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// Determines if the current <see cref="IType"/> is <see cref="ValueTask"/> or <see cref="ValueTask{TResult}"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="withResult">If <see langword="null"/>, matches types with our without <c>TResult</c>. If <see langword="false"/>, only matches
    /// types without <c>TResult</c>. If <see langword="true"/>, only matches types with <c>TResult</c>.</param>
    public static bool IsValueTask( this IType type, bool? withResult = default )
    {
        var unboundType = (type as INamedType)?.GetOriginalDefinition();

        if ( unboundType == null )
        {
            return false;
        }

        var isWithValue = unboundType.SpecialType == SpecialType.ValueTask_T;
        var isWithoutValue = unboundType.SpecialType == SpecialType.ValueTask;

        return withResult switch
        {
            true => isWithValue,
            false => isWithoutValue,
            _ => isWithValue || isWithoutValue
        };
    }

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    /// Determines if the current <see cref="IType"/> is <see cref="System.Collections.IEnumerator"/>, <see cref="IEnumerator{T}"/> or <c>IAsyncEnumerator{T}</c>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEnumerator( this IType type )
    {
        var unboundType = (type as INamedType)?.GetOriginalDefinition();

        if ( unboundType == null )
        {
            return false;
        }

        return unboundType.SpecialType is SpecialType.IEnumerator or SpecialType.IEnumerator_T or SpecialType.IAsyncEnumerator_T;
    }
}