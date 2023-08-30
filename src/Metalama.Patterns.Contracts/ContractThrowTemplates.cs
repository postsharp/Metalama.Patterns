// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.Contracts;

[PublicAPI]
public class ContractThrowTemplates : ITemplateProvider
{
    [Template]
    public virtual void OnCreditCardContractViolated( dynamic? value )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be a valid credit card number." );
    }

    [Template]
    public virtual void OnInvalidEnumValue( dynamic? value )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be a valid {meta.Target.GetTargetType().ToDisplayString()}." );
    }

    [Template]
    public virtual void OnNotEmptyContractViolated( dynamic? value )
    {
        throw new ArgumentException( ((INamedDeclaration) meta.Target.Declaration).Name, $"The {meta.Target.GetTargetName()} must not be null or empty." );
    }

    [Template]
    public virtual void OnNotNullContractViolated( dynamic? value )
    {
        throw new ArgumentNullException( ((INamedDeclaration) meta.Target.Declaration).Name, $"The {meta.Target.GetTargetName()} must not be null." );
    }

    [Template]
    public virtual void OnRegularExpressionContractViolated( dynamic? o, [CompileTime] string pattern )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must match the regular expression '{pattern}'." );
    }

    [Template]
    public virtual void OnPhoneContractViolated( dynamic? value )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be a valid phone number." );
    }

    [Template]
    public virtual void OnEmailAddressContractViolated( dynamic? value )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be a valid email address." );
    }

    [Template]
    public virtual void OnUrlContractViolated( dynamic? value )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be a valid URL." );
    }

    [Template]
    public virtual void OnStringMaxLengthContractViolated( dynamic? value, int maximumLength )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The  {meta.Target.GetTargetName()} must be a string with a maximum length of {maximumLength}." );
    }

    [Template]
    public virtual void OnStringMinLengthContractViolated( dynamic? value, int minimumLength )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The  {meta.Target.GetTargetName()} must be a string with a minimum length of {minimumLength}." );
    }

    [Template]
    public virtual void OnStringLengthContractViolated( dynamic? value, int minimumLength, int maximumLength )
    {
        throw new ArgumentException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The  {meta.Target.GetTargetName()} must be a string with length between {minimumLength} and {maximumLength}." );
    }

    [Template]
    public virtual void OnRangeContractViolated( dynamic? value, [CompileTime] object minValue, [CompileTime] object maxValue )
    {
        throw new ArgumentOutOfRangeException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be between {minValue} and {maxValue}" );
    }

    [Template]
    public virtual void OnGreaterThanContractViolated( dynamic? value, [CompileTime] object minValue )
    {
        throw new ArgumentOutOfRangeException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be greater than {minValue}." );
    }

    [Template]
    public virtual void OnLessThanContractViolated( dynamic? value, [CompileTime] object maxValue )
    {
        throw new ArgumentOutOfRangeException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be less than {maxValue}." );
    }

    [Template]
    public virtual void OnStrictlyGreaterThanContractViolated( dynamic? value, [CompileTime] object minValue )
    {
        throw new ArgumentOutOfRangeException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be strictly greater than {minValue}." );
    }

    [Template]
    public virtual void OnStrictlyLessThanContractViolated( dynamic? value, [CompileTime] object maxValue )
    {
        throw new ArgumentOutOfRangeException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be strictly less than {maxValue}." );
    }

    [Template]
    public virtual void OnStrictRangeContractViolated( dynamic? value, [CompileTime] object minValue, [CompileTime] object maxValue )
    {
        throw new ArgumentOutOfRangeException(
            ((INamedDeclaration) meta.Target.Declaration).Name,
            $"The {meta.Target.GetTargetName()} must be strictly between {minValue} and {maxValue}" );
    }

    [Template]
    public virtual void OnRequiredContractViolated( dynamic? value )
    {
        throw new ArgumentNullException( ((INamedDeclaration) meta.Target.Declaration).Name, $"The {meta.Target.GetTargetName()} is required." );
    }

    [Template]
    public virtual void OnRequiredContractViolatedBecauseOfEmptyString( dynamic value )
    {
        throw new ArgumentOutOfRangeException( ((INamedDeclaration) meta.Target.Declaration).Name, $"The {meta.Target.GetTargetName()} is required." );
    }
}