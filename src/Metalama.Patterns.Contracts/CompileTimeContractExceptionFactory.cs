// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using System.Globalization;

namespace Metalama.Patterns.Contracts;

[CompileTime]
public static class CompileTimeContractExceptionFactory
{
    public static IExpression GetNewExceptionExpression( ContractLocalizedTextProvider localizedTextProvider,
        ContractExceptionInfo exceptionInfo )
    {
        var errorMessage = localizedTextProvider.GetFormattedMessage( exceptionInfo );
        var parameterName = exceptionInfo.TargetKind.GetParameterName( exceptionInfo.TargetName );

        var b = new ExpressionBuilder();

        if ( exceptionInfo.ExceptionType.Equals( typeof(ArgumentException) ) )
        {
            b.AppendExpression( new ArgumentException( errorMessage, parameterName ) );
        }
        else if ( exceptionInfo.ExceptionType.Equals( typeof(ArgumentNullException) ) )
        {
            b.AppendVerbatim( "new " );
            b.AppendTypeName( typeof(ArgumentNullException) );
            b.AppendVerbatim( "(" );
            b.AppendLiteral( parameterName );
            b.AppendVerbatim( "," );
            b.AppendLiteral( errorMessage );
            b.AppendVerbatim( ")" );

            // TODO: Review - why doesn't this work?
            // b.AppendExpression( new ArgumentNullException( parameterName, errorMessage ) );
        }
        else if ( exceptionInfo.ExceptionType.Equals( typeof(ArgumentOutOfRangeException) ) )
        {
            b.AppendExpression( new ArgumentOutOfRangeException( parameterName, errorMessage ) );
        }
        else if ( exceptionInfo.ExceptionType.Equals( typeof(PostconditionFailedException) ) )
        {
            b.AppendExpression( new PostconditionFailedException( errorMessage ) );
        }
        else
        {
            const string template =
                "The [{0}] contract failed with {1}, but the current ContractExceptionFactory is not configured to instantiate this exception type";
            var aspectName = exceptionInfo.AspectType.Name;
            const string attribute = "Attribute";

            if ( aspectName.EndsWith( attribute, StringComparison.Ordinal ) )
            {
                aspectName = aspectName.Substring( 0, aspectName.Length - attribute.Length );
            }

            throw new InvalidOperationException( string.Format(
                CultureInfo.InvariantCulture,
                template,
                aspectName,
                exceptionInfo.ExceptionType.Name ) );
        }

        return b.ToExpression();
    }
}