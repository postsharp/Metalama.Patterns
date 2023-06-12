// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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
public sealed class PhoneAttribute : RegularExpressionAttribute
{
    private const string _pattern =
        "^(\\+\\s?)?((?<!\\+.*)\\(\\+?\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+)([\\s\\-\\.]?(\\(\\d+([\\s\\-\\.]?\\d+)?\\)|\\d+))*(\\s?(x|ext\\.?)\\s?\\d+)?$";

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneAttribute"/> class.
    /// </summary>
    public PhoneAttribute()
        : base( _pattern, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture )
    {
    }

    // TODO: Review, aim to avoid wholesale override, see comment on base method.
    public override void Validate( dynamic? value )
    {
        CompileTimeHelpers.GetTargetKindAndName( meta.Target, out var targetKind, out var targetName );

        if ( value != null && !Regex.IsMatch( value, this.Pattern, this.Options ) )
        {
            throw ContractServices.ExceptionFactory.CreateException( ContractExceptionInfo.Create(
                typeof( ArgumentException ),
                this.GetType(),
                value,
                targetName,
                targetKind,
                meta.Target.ContractDirection,
                ContractLocalizedTextProvider.PhoneErrorMessage ) );
        }
    }
}