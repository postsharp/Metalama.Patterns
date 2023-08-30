// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

// ReSharper disable UnusedMember.Global

namespace Metalama.Patterns.Contracts;

[CompileTime]
internal static class CompileTimeHelpers
{
    public static IExpression ToTypeOf( this Type type )
    {
        var expressionBuilder = new ExpressionBuilder();
        expressionBuilder.AppendVerbatim( "typeof(" );
        expressionBuilder.AppendTypeName( type );
        expressionBuilder.AppendVerbatim( ")" );

        return expressionBuilder.ToExpression();
    }

    public static string? GetTargetName( this IMetaTarget target )
    {
        if ( target == null )
        {
            throw new ArgumentNullException( nameof(target) );
        }

        return target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter => target.Parameter.IsReturnParameter ? null : target.Parameter.Name,
            DeclarationKind.Property => target.Property.Name,
            DeclarationKind.Field => target.Field.Name,
            _ => throw new ArgumentOutOfRangeException(
                nameof(target) + "." + nameof(target.Declaration) + "." +
                nameof(target.Declaration.DeclarationKind) )
        };
    }

    public static ContractTargetKind GetTargetKind( this IMetaTarget target )
    {
        if ( target == null )
        {
            throw new ArgumentNullException( nameof(target) );
        }

        return target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter => target.Parameter.IsReturnParameter ? ContractTargetKind.ReturnValue : ContractTargetKind.Parameter,
            DeclarationKind.Property => ContractTargetKind.Property,
            DeclarationKind.Field => ContractTargetKind.Field,
            _ => throw new ArgumentOutOfRangeException(
                nameof(target) + "." + nameof(target.Declaration) + "." +
                nameof(target.Declaration.DeclarationKind) )
        };
    }

    public static IType GetTargetType( this IMetaTarget target )
    {
        if ( target == null )
        {
            throw new ArgumentNullException( nameof(target) );
        }

        return target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter => target.Parameter.Type,
            DeclarationKind.Property => target.Property.Type,
            DeclarationKind.Field => target.Field.Type,
            _ => throw new ArgumentOutOfRangeException(
                nameof(target) + "." + nameof(target.Declaration) + "." +
                nameof(target.Declaration.DeclarationKind) )
        };
    }

    public static IEnumerable<INamedType> GetSelfAndAllImplementedInterfaces( this INamedType type )
    {
        if ( type == null )
        {
            throw new ArgumentNullException( nameof(type) );
        }

        if ( type.TypeKind == TypeKind.Interface )
        {
            yield return type;
        }

        foreach ( var i in type.AllImplementedInterfaces )
        {
            yield return i;
        }
    }
}