// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;

namespace Metalama.Patterns.Contracts;

[PublicAPI]
public class ContractTemplates : ITemplateProvider, ICompileTimeSerializable
{
    [Template]
    public virtual void OnCreditCardContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be a valid credit card number." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be a valid credit card number." );
        }
    }

    [Template]
    public virtual void OnInvalidEnumValue( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be a valid {meta.Target.GetTargetType().ToDisplayString()}." );
        }
        else
        {
            throw new PostconditionFailedException(
                $"The {meta.Target.GetTargetDisplayName()} must be a valid {meta.Target.GetTargetType().ToDisplayString()}." );
        }
    }

    [Template]
    public virtual void OnNotEmptyContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must not be null or empty." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must not be null or empty." );
        }
    }

    [Template]
    public virtual void OnNotNullContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentNullException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must not be null." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must not be null." );
        }
    }

    [Template]
    public virtual void OnRegularExpressionContractViolated( dynamic? value, dynamic? pattern )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must match the regular expression '{pattern}'." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must match the regular expression '{pattern}'." );
        }
    }

    [Template]
    public virtual void OnPhoneContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be a valid phone number." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be a valid phone number." );
        }
    }

    [Template]
    public virtual void OnEmailAddressContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be a valid email address." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be a valid email address." );
        }
    }

    [Template]
    public virtual void OnUrlContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be a valid URL." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be a valid URL." );
        }
    }

    [Template]
    public virtual void OnStringMaxLengthContractViolated( dynamic? value, int maximumLength )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The  {meta.Target.GetTargetDisplayName()} must be a string with a maximum length of {maximumLength}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The  {meta.Target.GetTargetDisplayName()} must be a string with a maximum length of {maximumLength}." );
        }
    }

    [Template]
    public virtual void OnStringMinLengthContractViolated( dynamic? value, int minimumLength )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The  {meta.Target.GetTargetDisplayName()} must be a string with a minimum length of {minimumLength}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The  {meta.Target.GetTargetDisplayName()} must be a string with a minimum length of {minimumLength}." );
        }
    }

    [Template]
    public virtual void OnStringLengthContractViolated( dynamic? value, int minimumLength, int maximumLength )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentException(
                meta.Target.GetTargetParameterName(),
                $"The  {meta.Target.GetTargetDisplayName()} must be a string with length between {minimumLength} and {maximumLength}." );
        }
        else
        {
            throw new PostconditionFailedException(
                $"The  {meta.Target.GetTargetDisplayName()} must be a string with length between {minimumLength} and {maximumLength}." );
        }
    }

    [Template]
    public virtual void OnRangeContractViolated( dynamic? value, [CompileTime] object minValue, [CompileTime] object maxValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be between {minValue} and {maxValue}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be between {minValue} and {maxValue}." );
        }
    }

    [Template]
    public virtual void OnGreaterThanContractViolated( dynamic? value, [CompileTime] object minValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be greater than {minValue}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be greater than {minValue}." );
        }
    }

    [Template]
    public virtual void OnLessThanContractViolated( dynamic? value, [CompileTime] object maxValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be less than {maxValue}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be less than {maxValue}." );
        }
    }

    [Template]
    public virtual void OnStrictlyGreaterThanContractViolated( dynamic? value, [CompileTime] object minValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be strictly greater than {minValue}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be strictly greater than {minValue}." );
        }
    }

    [Template]
    public virtual void OnStrictlyLessThanContractViolated( dynamic? value, [CompileTime] object maxValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be strictly less than {maxValue}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be strictly less than {maxValue}." );
        }
    }

    [Template]
    public virtual void OnStrictRangeContractViolated( dynamic? value, [CompileTime] object minValue, [CompileTime] object maxValue )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} must be strictly between {minValue} and {maxValue}." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} must be strictly between {minValue} and {maxValue}." );
        }
    }

    [Template]
    public virtual void OnRequiredContractViolated( dynamic? value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentNullException( meta.Target.GetTargetParameterName(), $"The {meta.Target.GetTargetDisplayName()} is required." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} is required." );
        }
    }

    [Template]
    public virtual void OnRequiredContractViolatedBecauseOfEmptyString( dynamic value )
    {
        if ( meta.Target.ContractDirection == ContractDirection.Input )
        {
            throw new ArgumentOutOfRangeException(
                meta.Target.GetTargetParameterName(),
                $"The {meta.Target.GetTargetDisplayName()} is required." );
        }
        else
        {
            throw new PostconditionFailedException( $"The {meta.Target.GetTargetDisplayName()} is required." );
        }
    }
}