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

    // TODO: Unlike PostSharp, we don't have per-aspect-instance runtime state, so we can't so easily cache the regex object.
    // Note that Regex has OOTB caching of compiled expressions (see eg Regex.CacheSize).
    // For now, just using runtime evaluation.

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        CompileTimeHelpers.GetTargetKindAndName( meta.Target, out var targetKind, out var targetName );
        var info = this.GetExceptionInfo();
        var aspectType = meta.CompileTime( this.GetType() );

        if ( value != null && !Regex.IsMatch( value, this.Pattern, this.Options ) )
        {
            if ( info.IncludePatternArgument )
            {
                throw ContractsServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                    info.ExceptionType,
                    aspectType,
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    info.MessageIdExpression.Value,
                    this.Pattern ) );
            }
            else
            {
                throw ContractsServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                    info.ExceptionType,
                    aspectType,
                    value,
                    targetName,
                    targetKind,
                    meta.Target.ContractDirection,
                    info.MessageIdExpression.Value ) );
            }
        }
    }

    /// <summary>
    /// Describes exception information as returned by <see cref="GetExceptionInfo"/>.
    /// </summary>
    [CompileTime]
    protected record struct ExceptionInfo( Type ExceptionType, IExpression MessageIdExpression, bool IncludePatternArgument );

    /// <summary>
    /// Called by <see cref="Validate(object?)"/> to determine the message to emit, and whether the pattern
    /// should be provided as a formatting argument.
    /// </summary>
    [CompileTime]
    protected virtual ExceptionInfo GetExceptionInfo()
        => new(
            typeof(ArgumentException),
            CompileTimeHelpers.GetContractLocalizedTextProviderField( nameof(ContractLocalizedTextProvider
                .RegularExpressionErrorMessage) ),
            true);
}