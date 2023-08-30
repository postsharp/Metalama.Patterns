// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Eligibility;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentException"/> if the target is assigned a value that
/// does not match a given regular expression. Null strings are accepted and do not
/// throw an exception.
/// </summary>
/// <remarks>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the regular expression used.</para>
/// </remarks>
[PublicAPI]
[Inheritable]
public abstract class BaseRegularExpressionAttribute : ContractAspect
{
    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );
        builder.Type().MustBe<string>();
    }

    /// <inheritdoc/>
    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );
        builder.Type().MustBe<string>();
    }
    
    protected abstract IExpression GetRegex();

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        var regex = (Regex) this.GetRegex().Value!;

        if ( value != null && !regex.IsMatch( (string) value! ) )
        {
            this.OnContractViolated( value, regex );
        }
    }

    [Template]
    protected virtual void OnContractViolated( dynamic? value, dynamic regex )
    {
        meta.Target.Project.ContractOptions().Templates.OnRegularExpressionContractViolated( value, regex );
    }
}


public class RegularExpressionAttribute : BaseRegularExpressionAttribute
{
    public string Pattern { get; }

    public RegexOptions Options { get; }

    public RegularExpressionAttribute( string pattern, RegexOptions options = RegexOptions.None )
    {
        this.Pattern = pattern;
        this.Options = options;
    }

    protected override IExpression GetRegex()
    {
        var builder = new ExpressionBuilder();
        builder.AppendTypeName( typeof(ContractHelpers) );
        builder.AppendVerbatim( "." );
        builder.AppendVerbatim( nameof(ContractHelpers.GetRegex) );
        builder.AppendVerbatim( "(" );
        builder.AppendLiteral( this.Pattern );
        builder.AppendVerbatim( ", " );
        builder.AppendLiteral( (int) this.Options );
        builder.AppendVerbatim( ")" );

        return builder.ToExpression();
    }
}