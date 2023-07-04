// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using System.Security.Cryptography;
using System.Text;
namespace Metalama.Patterns.Caching;

[CompileTime]
internal static class CompileTimeHelpers
{
    public static void Append( this ExpressionBuilder builder, bool condition, Action<ExpressionBuilder> build )
    {
        if ( condition )
        {
            build( builder );
        }
    }

    public static void AppendVerbatim( this ExpressionBuilder builder, bool condition, string rawCode )
    {
        if ( condition )
        {
            builder.AppendVerbatim( rawCode );
        }
    }

    public static void AppendList( this ExpressionBuilder builder, string separator, params (bool Condition, Action<ExpressionBuilder> Build)[] items )
    {
        bool isFirst = true;
        AppendList( builder, ref isFirst, separator, items );
    }

    public static void AppendList( this ExpressionBuilder builder, ref bool isFirst, string separator, params (bool Condition, Action<ExpressionBuilder> Build)[] items )
    {
        foreach ( var item in items )
        {
            if ( item.Condition )
            {
                if ( !isFirst && !string.IsNullOrEmpty( separator ) )
                {
                    builder.AppendVerbatim( separator );
                }

                item.Build( builder );

                isFirst = false;
            }
        }
    }

#if false
    public static void AppendVerbatimListItem( this ExpressionBuilder builder, bool condition, string rawCode )
    {
        if ( condition )
        {
            builder.AppendVerbatim( rawCode );
        }
    }

    public static void AppendVerbatimListItem( this ExpressionBuilder builder, bool condition, string rawCode, ref bool prependComma )
    {
        if ( condition )
        {
            if ( prependComma )
            {
                builder.AppendVerbatim( ", " );
            }

            builder.AppendVerbatim( rawCode );

            prependComma = true;
        }
    }
#endif

    /// <summary>
    /// Gets a string that can be used as a C# identifier based on a stable hash of the given <see cref="SerializableDeclarationId"/>
    /// with an optional prefix.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <param name="prefix">An optional prefix for the identifier.</param>
    /// <param name="purpose">
    /// Typically a unique hard-coded GUID corresponding to the purpose for which an identifier associated with the
    /// given ID is required. This is to avoid collisions, for example when multiple unrelated aspects introduce members associated
    /// with the same ID. Alternatively, related aspects could use a common value for <paramref name="purpose"/> if desired.
    /// </param>
    /// <returns></returns>
    public static string MakeAssociatedIdentifier( this SerializableDeclarationId id, string purpose, string? prefix = null )
    {
        if ( id == null )
        {
            throw new ArgumentNullException( nameof( id ) );
        }    

        if ( purpose == null )
        {
            throw new ArgumentNullException( nameof( purpose ) );
        }

        // Not used for cryptographic purposes.
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
        var md5 = MD5.Create();
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
        var bytes = md5.ComputeHash( Encoding.UTF8.GetBytes( id.ToString() + purpose ) );

        // TODO: use base62 or something else
        return prefix + "_" + BitConverter.ToString( bytes ).Replace( "-", string.Empty );
    }

    /// <summary>
    /// Throws the current exception at compile time.
    /// </summary>    
    public static void ThrowAtCompileTime( this Exception e ) => throw e;

    /// <summary>
    /// Determines if the current <see cref="IType"/> is <see cref="Task"/> or <see cref="ValueTask"/> (when <paramref name="withResult"/> is <see langword="false"/>),
    /// or <see cref="Task{TResult}"/> or <see cref="ValueTask{TResult}"/> (when <paramref name="withResult"/> is <see langword="true"/>).
    /// </summary>
    public static bool IsTaskOrValueTask( this IType type, bool withResult = false )
        => withResult
            ? type.SpecialType == SpecialType.Task_T || type.SpecialType == SpecialType.ValueTask_T
            : type.SpecialType == SpecialType.Task || type.SpecialType == SpecialType.ValueTask;
}