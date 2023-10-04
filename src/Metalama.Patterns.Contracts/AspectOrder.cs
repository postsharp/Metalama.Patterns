// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.Contracts;

[assembly: AspectOrder(
    typeof(NotNullAttribute),
    typeof(RequiredAttribute),
    typeof(StringLengthAttribute),
    typeof(NotEmptyAttribute),
    typeof(RegularExpressionAttribute),
    typeof(PhoneAttribute),
    typeof(EmailAddressAttribute),
    typeof(UrlAttribute),
    typeof(CreditCardAttribute)
    )]