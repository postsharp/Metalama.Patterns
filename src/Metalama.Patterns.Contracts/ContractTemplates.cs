// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Serialization;

namespace Metalama.Patterns.Contracts;

[PublicAPI]
public class ContractTemplates : ITemplateProvider, ICompileTimeSerializable
{
    [CompileTime]
    protected static string TargetDisplayName => meta.Target.GetTargetDisplayName();

    [CompileTime]
    protected static string TargetParameterName => meta.Target.GetTargetParameterName();

    #region Public Methods
    [Template]
    public virtual void OnCreditCardContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException(
                $"The {TargetDisplayName} must be a valid credit card number.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be a valid credit card number.");
        }
    }

    [Template]
    public virtual void OnEmailAddressContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException($"The {TargetDisplayName} must be a valid email address.", TargetParameterName);
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be a valid email address.");
        }
    }

    [Template]
    public virtual void OnGreaterThanContractViolated(dynamic? value, [CompileTime] object minValue)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be greater than {minValue}.");
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be greater than {minValue}.");
        }
    }

    [Template]
    public virtual void OnInvalidEnumValue(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException(
                $"The {TargetDisplayName} must be a valid {meta.Target.GetTargetType().ToDisplayString()}.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException(
                $"The {TargetDisplayName} must be a valid {meta.Target.GetTargetType().ToDisplayString()}.");
        }
    }

    [Template]
    public virtual void OnLessThanContractViolated(dynamic? value, [CompileTime] object maxValue)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be less than {maxValue}.");
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be less than {maxValue}.");
        }
    }

    [Template]
    public virtual void OnNotEmptyContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException($"The {TargetDisplayName} must not be null or empty.", TargetParameterName);
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must not be null or empty.");
        }
    }

    [Template]
    public virtual void OnNotNullContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentNullException(TargetParameterName, $"The {TargetDisplayName} must not be null.");
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must not be null.");
        }
    }

    [Template]
    public virtual void OnPasswordContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException(
                $"The {TargetDisplayName} does not match the following requirements. A password must have at least 1 uppercase letter, 1 lowercase letter, 1 number and 1 special charater in the following range '!@#$%^&*()_+' and be of a minimum length of 8 characters overall.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be a valid password.");
        }
    }

    [Template]
    public virtual void OnPhoneContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException($"The {TargetDisplayName} must be a valid phone number.", TargetParameterName);
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be a valid phone number.");
        }
    }

    [Template]
    public virtual void OnRangeContractViolated(
        dynamic? value,
        [CompileTime] object minValue,
        [CompileTime] object maxValue)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentOutOfRangeException(
                $"The {TargetDisplayName} must be between {minValue} and {maxValue}.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException(
                $"The {TargetDisplayName} must be between {minValue} and {maxValue}.");
        }
    }

    [Template]
    public virtual void OnRegularExpressionContractViolated(dynamic? value, dynamic? pattern)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException(
                $"The {TargetDisplayName} must match the regular expression '{pattern}'.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException(
                $"The {TargetDisplayName} must match the regular expression '{pattern}'.");
        }
    }

    [Template]
    public virtual void OnRequiredContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentNullException(TargetParameterName, $"The {TargetDisplayName} is required.");
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} is required.");
        }
    }

    [Template]
    public virtual void OnRequiredContractViolatedBecauseOfEmptyString(dynamic value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentOutOfRangeException(TargetParameterName, $"The {TargetDisplayName} is required.");
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} is required.");
        }
    }

    [Template]
    public virtual void OnStrictlyGreaterThanContractViolated(dynamic? value, [CompileTime] object minValue)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be strictly greater than {minValue}.");
        } else
        {
            throw new PostconditionViolationException(
                $"The {TargetDisplayName} must be strictly greater than {minValue}.");
        }
    }

    [Template]
    public virtual void OnStrictlyLessThanContractViolated(dynamic? value, [CompileTime] object maxValue)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be strictly less than {maxValue}.");
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be strictly less than {maxValue}.");
        }
    }

    [Template]
    public virtual void OnStrictRangeContractViolated(
        dynamic? value,
        [CompileTime] object minValue,
        [CompileTime] object maxValue)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentOutOfRangeException(
                TargetParameterName,
                $"The {TargetDisplayName} must be strictly between {minValue} and {maxValue}.");
        } else
        {
            throw new PostconditionViolationException(
                $"The {TargetDisplayName} must be strictly between {minValue} and {maxValue}.");
        }
    }

    [Template]
    public virtual void OnStringLengthContractViolated(dynamic? value, int minimumLength, int maximumLength)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException(
                $"The  {TargetDisplayName} must be a string with length between {minimumLength} and {maximumLength}.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException(
                $"The  {TargetDisplayName} must be a string with length between {minimumLength} and {maximumLength}.");
        }
    }

    [Template]
    public virtual void OnStringMaxLengthContractViolated(dynamic? value, int maximumLength)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException(
                $"The  {TargetDisplayName} must be a string with a maximum length of {maximumLength}.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException(
                $"The  {TargetDisplayName} must be a string with a maximum length of {maximumLength}.");
        }
    }

    [Template]
    public virtual void OnStringMinLengthContractViolated(dynamic? value, int minimumLength)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException(
                $"The  {TargetDisplayName} must be a string with a minimum length of {minimumLength}.",
                TargetParameterName);
        } else
        {
            throw new PostconditionViolationException(
                $"The  {TargetDisplayName} must be a string with a minimum length of {minimumLength}.");
        }
    }

    [Template]
    public virtual void OnUrlContractViolated(dynamic? value)
    {
        if(meta.Target.ContractDirection == ContractDirection.Input)
        {
            throw new ArgumentException($"The {TargetDisplayName} must be a valid URL.", TargetParameterName);
        } else
        {
            throw new PostconditionViolationException($"The {TargetDisplayName} must be a valid URL.");
        }
    }

    #endregion
}