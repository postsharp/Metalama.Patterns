// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
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
/// <para>Error message can use additional argument <value>{4}</value> to refer to the regular expression used.</para>
/// </remarks>
[PublicAPI]
[Inheritable]
public class RegularExpressionAttribute : ContractAspect
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegularExpressionAttribute"/> class.
    /// </summary>
    /// <param name="pattern">The regular expression.</param>
    /// <param name="options">Options.</param>
    public RegularExpressionAttribute( string pattern, RegexOptions options = RegexOptions.None )
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
    // Note that Regex has built-in caching of compiled expressions (see eg Regex.CacheSize).
    // For now, just using runtime evaluation.

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        if ( value != null && !Regex.IsMatch( value, this.Pattern, this.Options ) )
        {
            this.OnContractViolated( value );
        }
    }

    [Template]
    protected virtual void OnContractViolated( dynamic? value )
    {
        meta.Target.Project.ContractOptions().Templates.OnRegularExpressionContractViolated( value, this.Pattern );
    }
}