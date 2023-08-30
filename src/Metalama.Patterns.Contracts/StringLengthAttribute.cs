// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentException"/> if the target is assigned a string of invalid length.
/// Null strings are accepted and do not throw an exception.
/// </summary>
/// <remarks>
/// <para>Depending on supplied constructor arguments, one of the following holds:
/// <list type="bullet">
///     <item><description>
///         if there is no minimum specified, then the error message is identified by <see cref="ContractTextProvider.StringLengthMaxErrorMessage"/>
///         and can use additional argument <value>{4}</value> to refer to the maximum value specified,
///     </description></item>
///     <item><description>
///         or if there is maximum is equal to <see cref="int.MaxValue"/>, then the error message is identified by <see cref="ContractTextProvider.StringLengthMinErrorMessage"/>
///         and can use additional argument <value>{4}</value> to refer to the minimum value specified,
///     </description></item>
///     <item><description>
///         otherwise, the error message is identified by <see cref="ContractTextProvider.StringLengthRangeErrorMessage"/>
///         and can use additional arguments <value>{4}</value> to refer to the minimum value specified and <value>{5}</value> to refer to the maximum value specified.
///     </description></item>
/// </list></para>
/// </remarks>
[PublicAPI]
[Inheritable]
public sealed class StringLengthAttribute : ContractAspect
{
    // TODO: Add diagnostics if the aspect construction is invalid (eg, max < min).

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLengthAttribute"/> class.
    /// </summary>
    /// <param name="maximumLength">Maximum length.</param>
    public StringLengthAttribute( int maximumLength )
    {
        this.MaximumLength = maximumLength;
        this.MinimumLength = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringLengthAttribute"/> class.
    /// </summary>
    /// <param name="maximumLength">Maximum length.</param>
    /// <param name="minimumLength">Minimum length.</param>
    public StringLengthAttribute( int minimumLength, int maximumLength )
    {
        this.MaximumLength = maximumLength;
        this.MinimumLength = minimumLength;
    }

    /// <summary>
    /// Gets the maximum length.
    /// </summary>
    public int MaximumLength { get; }

    /// <summary>
    /// Gets the minimum length.
    /// </summary>
    public int MinimumLength { get; }

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

    /// <inheritdoc/>
    public override void Validate( dynamic? value )
    {
        // TODO: We assume that min and max are sensible (eg, non-negative) here. This should be validated ideally at compile time. See comment at head of class.

        if ( this.MinimumLength == 0 && this.MaximumLength != int.MaxValue )
        {
            if ( value != null && value!.Length > this.MaximumLength )
            {
                meta.Target.Project.ContractOptions().Templates.OnStringMaxLengthContractViolated( value, this.MaximumLength );
            }
        }
        else if ( this.MinimumLength > 0 && this.MaximumLength == int.MaxValue )
        {
            if ( value != null && value!.Length < this.MinimumLength )
            {
                meta.Target.Project.ContractOptions().Templates.OnStringMinLengthContractViolated( value, this.MinimumLength );
            }
        }
        else if ( this.MinimumLength > 0 && this.MaximumLength != int.MaxValue )
        {
            // Incorrect warning dereferencing `value` after checking for null.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if ( value != null && (value.Length < this.MinimumLength || value.Length > this.MaximumLength) )
            {
                meta.Target.Project.ContractOptions().Templates.OnStringLengthContractViolated( value, this.MinimumLength, this.MaximumLength );
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        // else: min is zero, max is int.MaxVal, all strings are valid, no need to check.
    }
}