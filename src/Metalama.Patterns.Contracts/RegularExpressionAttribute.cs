// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
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
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.RegularExpressionErrorMessage"/>.</para>
/// <para>Error message can use additional argument <value>{4}</value> to refer to the regular expression used.</para>
/// </remarks>
[Inheritable]
public class RegularExpressionAttribute : ContractAspect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegularExpressionAttribute"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression.</param>
    public RegularExpressionAttribute( string pattern )
        : this( pattern, RegexOptions.None )
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegularExpressionAttribute"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression.</param>
    /// <param name="options">Options.</param>
    public RegularExpressionAttribute( string pattern, RegexOptions options )
    {
        this.Pattern = pattern;
        this.Options = options | RegexOptions.Compiled;
    }

    /// <summary>
    /// Gets the regular expression to match.
    /// </summary>
    public string Pattern { get; }

    /// <summary>
    /// Gets the regular expression options.
    /// </summary>
    public RegexOptions Options { get; }

    public override void BuildEligibility( IEligibilityBuilder<IFieldOrPropertyOrIndexer> builder )
    {
        base.BuildEligibility( builder );
        builder.Type().MustBe<string>();
    }

    public override void BuildEligibility( IEligibilityBuilder<IParameter> builder )
    {
        base.BuildEligibility( builder );
        builder.Type().MustBe<string>();
    }

    // TODO: Vs PS, we don't have per-aspect-instance runtime state, so we can't so easily cache the regex object.
    // Note that Regex has OOTB caching of compiled expressions (see eg Regex.CacheSize).
    // For now, just using runtime eval.

    // TODO: PS allows derived aspects to throw exceptions with a specialized message ID, and typically
    // without adding the _pattern arg. I've tried various ways to do this elegantly with ML, but every
    // avenue is either blocked or undesirable. For now, derived aspects must override the whole
    // Validate method, and _pattern and _options have been promoted to properties.

    public override void Validate( dynamic? value )
    {
        CompileTimeHelpers.GetTargetKindAndName( meta.Target, out var targetKind, out var targetName );
        var info = this.GetExceptioninfo();

        if ( value != null && !Regex.IsMatch( value, this.Pattern, this.Options ) )
        {
            if ( info.IncludePatternArgument )
            {
                throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                    info.ExceptionType,
                    info.AspectType,
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    info.MessageIdExpression.Value,
                    this.Pattern ) );
            }
            else
            {
                throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                    info.ExceptionType,
                    info.AspectType,
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    info.MessageIdExpression.Value ) );
            }
        }
    }

    [CompileTime]
    protected virtual (Type ExceptionType, Type AspectType, IExpression MessageIdExpression, bool IncludePatternArgument) GetExceptioninfo()
        => (typeof( ArgumentException ),
            typeof( RegularExpressionAttribute ),
            CompileTimeHelpers.GetContractLocalizedTextProviderField( nameof( ContractLocalizedTextProvider.RegularExpressionErrorMessage ) ),
            true);
}