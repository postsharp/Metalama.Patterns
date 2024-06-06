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

    public static string GetTargetDisplayName( this IMetaTarget target )
    {
        return target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter when target.Parameter.IsReturnParameter => "return value",
            DeclarationKind.Parameter => $"'{target.Parameter.Name}' parameter",
            DeclarationKind.Property => $"'{target.Property.Name}' property",
            DeclarationKind.Field => $"'{target.Field.Name}' field",
            DeclarationKind.Indexer => "indexer",
            _ => throw new ArgumentOutOfRangeException(
                nameof(target) + "." + nameof(target.Declaration) + "." +
                nameof(target.Declaration.DeclarationKind) )
        };
    }

    public static string GetTargetParameterName( this IMetaTarget target )
    {
        return target.Declaration.DeclarationKind switch
        {
            DeclarationKind.Parameter => target.Parameter.Name,
            DeclarationKind.Property or DeclarationKind.Field or DeclarationKind.Indexer => "value",
            _ => throw new ArgumentOutOfRangeException(
                nameof(target) + "." + nameof(target.Declaration) + "." +
                nameof(target.Declaration.DeclarationKind) )
        };
    }

    public static IType GetTargetType( this IMetaTarget target ) => ((IHasType) target.Declaration).Type;

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

    public static void WarnIfNullable<T>( this IAspectBuilder<T> aspectBuilder )
        where T : class, IDeclaration, IHasType
    {
        if ( aspectBuilder.Target.Type.IsNullable == true && aspectBuilder.Target.Type.TypeKind != TypeKind.TypeParameter &&
             (aspectBuilder.Target.GetContractOptions().WarnOnNotNullableOnNullable ?? true) )
        {
            aspectBuilder.Diagnostics.Report(
                ContractDiagnostics.NotNullableOnNullable.WithArguments( (aspectBuilder.Target, aspectBuilder.AspectInstance.AspectClass.ShortName) ) );
        }
    }
}