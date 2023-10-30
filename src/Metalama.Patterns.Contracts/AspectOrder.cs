// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts;

[assembly: AspectOrder(
    "Metalama.Patterns.Contracts." + nameof(SuspendInvariantsAttribute) + ":*",
    "Metalama.Patterns.Contracts." + nameof(CheckInvariantsAspect) + ":*",
    "Metalama.Patterns.Contracts." + nameof(InvariantAttribute) + ":*",
    "Metalama.Patterns.Contracts." + nameof(NotNullAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(RequiredAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(StringLengthAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(NotEmptyAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(RegularExpressionAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(PhoneAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(EmailAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(UrlAttribute) + ":" + ContractAspect.Layer1Build,
    "Metalama.Patterns.Contracts." + nameof(CreditCardAttribute) + ":" + ContractAspect.Layer1Build )]