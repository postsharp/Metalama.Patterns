// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using System.Text.RegularExpressions;

namespace Metalama.Patterns.Contracts;

/// <summary>
/// Custom attribute that, when added to a field, property or parameter, throws
/// an <see cref="ArgumentException"/> if the target is assigned a value that
/// is not a valid phone number. Null strings are accepted and do not
/// throw an exception.
/// </summary>
/// <remarks>
/// <para>Error message is identified by <see cref="ContractLocalizedTextProvider.PhoneErrorMessage"/>.</para>
/// </remarks>
[PublicAPI]
public sealed class PhoneAttribute : RegularExpressionAttribute
{
    private const string _pattern =
        "^(\\+\\s?)?((?<!\\+.*)\\(\\+?\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+)([\\s\\-\\.]?(\\(\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+))*(\\s?(x|ext\\.?)\\s?\\d+)?$";

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneAttribute"/> class.
    /// </summary>
    public PhoneAttribute()
        : base( _pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture ) { }

    /// <inheritdoc/>
    [CompileTime]
    protected override ExceptionInfo GetExceptionInfo()
        => new(
            typeof(ArgumentException),
            CompileTimeHelpers.GetContractLocalizedTextProviderField(
                nameof(ContractLocalizedTextProvider
                           .PhoneErrorMessage) ),
            false );
}