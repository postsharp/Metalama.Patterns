// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts;

[assembly: AspectOrder(
    typeof(SuspendInvariantsAttribute),
    typeof(CheckInvariantsAspect),
    typeof(InvariantAttribute),
    typeof(NotNullAttribute),
    typeof(RequiredAttribute),
    typeof(StringLengthAttribute),
    typeof(NotEmptyAttribute),
    typeof(RegularExpressionAttribute),
    typeof(PhoneAttribute),
    typeof(EmailAttribute),
    typeof(UrlAttribute),
    typeof(CreditCardAttribute) )]