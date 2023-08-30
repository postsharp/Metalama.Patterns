// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentException"/> if the target is assigned a value that
/// is not a valid phone number. Null strings are accepted and do not
/// throw an exception.
/// </summary>
[PublicAPI]
public sealed class PhoneAttribute : RegularExpressionBaseAttribute
{
    protected override IExpression GetRegex()
    {
        var builder = new ExpressionBuilder();
        builder.AppendTypeName( typeof(ContractHelpers) );
        builder.AppendVerbatim( "." );
        builder.AppendVerbatim( nameof(ContractHelpers.PhoneRegex) );

        return builder.ToExpression();
    }
    
    protected override void OnContractViolated( dynamic? value, dynamic regex )
    {
        meta.Target.Project.ContractOptions().Templates.OnPhoneContractViolated( value );
    }
}