﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Patterns.Contracts;

[CompileTime]
internal static class CompileTimeHelpers
{
    /// <summary>
    /// Gets an expression representing a field of <see cref="ContractLocalizedTextProvider"/>.
    /// </summary>
    /// <param name="fieldName">Should be like <c>nameof( ContractLocalizedTextProvider.CreditCardErrorMessage )</c>.</param>
    /// <returns></returns>
    internal static IExpression GetContractLocalizedTextProviderField( string fieldName )
        => ((INamedType) TypeFactory.GetType( typeof(ContractLocalizedTextProvider) )).Fields
            .OfName( fieldName ).Single();

    public static IExpression ToTypeOf( this Type type )
    {
        var expressionBuilder = new ExpressionBuilder();
        expressionBuilder.AppendVerbatim( "typeof(" );
        expressionBuilder.AppendTypeName( type );
        expressionBuilder.AppendVerbatim( ")" );

        return expressionBuilder.ToExpression();
    }

    public static void GetTargetKindAndName( IMetaTarget target, out ContractTargetKind kind, out string? name )
    {
        if ( target == null )
        {
            throw new ArgumentNullException( nameof(target) );
        }

        switch ( target.Declaration.DeclarationKind )
        {
            case DeclarationKind.Parameter:
                if ( target.Parameter.IsReturnParameter )
                {
                    kind = ContractTargetKind.ReturnValue;
                    name = null;
                }
                else
                {
                    kind = ContractTargetKind.Parameter;
                    name = target.Parameter.Name;
                }

                break;

            case DeclarationKind.Property:
                kind = ContractTargetKind.Property;
                name = target.Property.Name;
                break;

            case DeclarationKind.Field:
                kind = ContractTargetKind.Field;
                name = target.Field.Name;
                break;

            default:
                throw new ArgumentOutOfRangeException( nameof(target) + "." + nameof(target.Declaration) + "." +
                                                       nameof(target.Declaration.DeclarationKind) );
        }
    }

    public static IType GetTargetType( IMetaTarget target )
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
            _ => throw new ArgumentOutOfRangeException( nameof(target) + "." + nameof(target.Declaration) + "." +
                                                        nameof(target.Declaration.DeclarationKind) )
        };
    }

    public static IEnumerable<INamedType> GetSelfAndAllImplementedInterfaces( INamedType type )
    {
        if ( type.TypeKind == TypeKind.Interface )
        {
            yield return type;
        }

        foreach ( var i in type.AllImplementedInterfaces )
        {
            yield return i;
        }
    }

    // TODO: Remove the block below if it remains unwanted.
#if false // Probably overkill.
        public static string GetTargetKindDisplayName( this IDescribedObject<IFieldOrPropertyOrIndexer> describedObject )
        {
            if ( describedObject == null )
            {
                throw new ArgumentNullException( nameof( describedObject ) );
            }

            return GetTargetKindDisplayName( describedObject.Object );
        }

        public static string GetTargetKindDisplayName( this IFieldOrPropertyOrIndexer member )
        {
            if ( member == null )
            {
                throw new ArgumentNullException( nameof( member ) );
            }

            return member.DeclarationKind switch
            {
                DeclarationKind.Field => "field",
                DeclarationKind.Property => "property",
                _ => throw new ArgumentOutOfRangeException( nameof( member ) + "." + nameof( member.DeclarationKind ) )
            };
        }

        public static string GetTargetKindDisplayName( this IDescribedObject<IParameter> describedObject)
        {
            if ( describedObject == null )
            {
                throw new ArgumentNullException( nameof( describedObject ) );
            }

            return GetTargetKindDisplayName( describedObject.Object );
        }

        public static string GetTargetKindDisplayName( this IParameter member )
        {
            if ( member == null )
            {
                throw new ArgumentNullException( nameof( member ) );
            }

            return member.IsReturnParameter ? "return value" : "parameter";
        }
#endif
}