// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;
using Metalama.Patterns.Contracts.Numeric;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Provides default implementations for the code templates used by code contract aspects (derived from <see cref="ContractBaseAttribute"/>).
/// This class can be derived and templates can be overridden. To register the new template implementations, use the <see cref="ContractOptions.Templates"/>
/// contract options.
/// </summary>
/// <seealso cref="@configuring-contracts"/>
[PublicAPI]
public class ContractTemplates : ITemplateProvider, ICompileTimeSerializable
{
    /// <summary>
    /// Gets the name of the target parameter.
    /// </summary>
    [CompileTime]
    protected static string TargetParameterName => meta.Target.GetTargetParameterName();

    /// <summary>
    /// Gets a human-readable name of the target declaration.
    /// </summary>
    [CompileTime]
    protected static string TargetDisplayName => meta.Target.GetTargetDisplayName();

    /// <summary>
    /// Template used by the <see cref="CreditCardAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnCreditCardContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException( $"The {TargetDisplayName} must be a valid credit card number.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be a valid credit card number." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="EnumDataTypeAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnInvalidEnumValue( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                $"The {TargetDisplayName} must be a valid {meta.Target.GetTargetType().ToDisplayString()}.",
                TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be a valid {meta.Target.GetTargetType().ToDisplayString()}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="NotEmptyAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnNotEmptyContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                $"The {TargetDisplayName} must not be null or empty.",
                TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must not be null or empty." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="NotNullAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnNotNullContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentNullException(
                TargetParameterName,
                $"The {TargetDisplayName} must not be null." );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must not be null." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="RegexPatternAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnRegularExpressionContractViolated( dynamic? value, dynamic? pattern )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException( $"The {TargetDisplayName} must match the regular expression '{pattern}'.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must match the regular expression '{pattern}'." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="PhoneAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnPhoneContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException( $"The {TargetDisplayName} must be a valid phone number.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be a valid phone number." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="EmailAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnEmailAddressContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException( $"The {TargetDisplayName} must be a valid email address.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be a valid email address." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="UrlAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnUrlContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException( $"The {TargetDisplayName} must be a valid URL.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be a valid URL." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="StringLengthAttribute"/> contract when only the upper bound is specified.
    /// </summary>
    [Template]
    public virtual void OnStringMaxLengthContractViolated( dynamic? value, int maximumLength )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException( $"The  {TargetDisplayName} must be a string with a maximum length of {maximumLength}.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The  {TargetDisplayName} must be a string with a maximum length of {maximumLength}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="StringLengthAttribute"/> contract when only the lower bound is specified.
    /// </summary>
    [Template]
    public virtual void OnStringMinLengthContractViolated( dynamic? value, int minimumLength )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException( $"The  {TargetDisplayName} must be a string with a minimum length of {minimumLength}.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The  {TargetDisplayName} must be a string with a minimum length of {minimumLength}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="StringLengthAttribute"/> contract when both the lower and the upper bounds are specified.
    /// </summary>
    [Template]
    public virtual void OnStringLengthContractViolated( dynamic? value, int minimumLength, int maximumLength )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                $"The  {TargetDisplayName} must be a string with length between {minimumLength} and {maximumLength}.",
                TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The  {TargetDisplayName} must be a string with length between {minimumLength} and {maximumLength}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="RangeAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnRangeContractViolated( dynamic? value, [CompileTime] NumericRange range )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException( $"The {TargetDisplayName} must be in the range {range}.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be in the range {range}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="GreaterThanAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnGreaterThanContractViolated( dynamic? value, [CompileTime] object minValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be greater than or equal to {minValue}." );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be greater than or equal to {minValue}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="LessThanAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnLessThanContractViolated( dynamic? value, [CompileTime] object maxValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be less than or equal to {maxValue}." );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be less than or equal to {maxValue}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="StrictlyGreaterThanAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnStrictlyGreaterThanContractViolated( dynamic? value, [CompileTime] object minValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be strictly greater than {minValue}." );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be strictly greater than {minValue}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="StrictlyLessThanAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnStrictlyLessThanContractViolated( dynamic? value, [CompileTime] object maxValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be strictly less than {maxValue}." );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be strictly less than {maxValue}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="StrictRangeAttribute"/> contract.
    /// </summary>
    [Template]
    public virtual void OnStrictRangeContractViolated( dynamic? value, [CompileTime] NumericRange range )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException( $"The {TargetDisplayName} must be strictly in the range {range}.", TargetParameterName );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} must be strictly  in the range {range}." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="RequiredAttribute"/> contract when the value is null.
    /// </summary>
    [Template]
    public virtual void OnRequiredContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentNullException( TargetParameterName, $"The {TargetDisplayName} is required." );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} is required." );
        }
    }

    /// <summary>
    /// Template used by the <see cref="RequiredAttribute"/> contract when the value is an empty string.
    /// </summary>
    [Template]
    public virtual void OnRequiredContractViolatedBecauseOfEmptyString( dynamic value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} is required." );
        }
        else
        {
            throw new PostconditionViolationException( $"The {TargetDisplayName} is required." );
        }
    }
}