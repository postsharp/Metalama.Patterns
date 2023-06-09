// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Patterns.Utilities;

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// Base class for contract error messages providers.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///    An implementation would typically override the <see cref="GetMessage"/> method. This method returns a formatting
    /// string where first 4 parameters have a well-known signification:
    ///   </para>
    /// <list type="table">
    ///     <listheader>  
    ///         <term>Parameter</term>  
    ///         <description>Description</description>  
    ///      </listheader>  
    ///    <item>  
    ///     <term>{0}</term>  
    ///     <description>Name of the declaration being validated (empty in case of return value).</description>  
    ///     </item>  
    ///    <item>  
    ///     <term>{1}</term>  
    ///     <description>Kind of the declaration being validated.</description>  
    ///     </item>
    ///    <item>  
    ///     <term>{2}</term>  
    ///     <description>Name and kind of the declaration being validated.</description>  
    ///     </item>
    ///    <item>  
    ///     <term>{3}</term>  
    ///     <description>The invalid value.</description>  
    ///     </item>
    /// </list>
    /// </remarks>
    public class ContractLocalizedTextProvider : LocalizedTextProvider
    {
        private readonly Dictionary<string, string> _messages = new()
        {
            { CreditCardErrorMessage, "The {2} must be a valid credit card number." },
            { EmailAddressErrorMessage, "The {2} must be a valid email address." },
            { EnumDataTypeErrorMessage, "The {2} must be a valid {4}." },
            { GreaterThanErrorMessage, "The {2} must be greater than {4}." },
            { LessThanErrorMessage, "The {2} must be less than {4}." },
            { LocationContractErrorMessage, "The {2} has an invalid value." },
            { NotEmptyErrorMessage, "The {2} cannot be null or empty." },
            { NotNullErrorMessage, "The {2} must not be null." },
            { PhoneErrorMessage, "The {2} should be a valid phone number." },
            { RangeErrorMessage, "The {2} must be between {4} and {5}." },
            { RegularExpressionErrorMessage, "The {2} must match the regular expression '{4}'." },
            { RequiredErrorMessage, "The {2} is required." },
            { StrictlyGreaterThanErrorMessage, "The {2} must be strictly greater than {4}." },
            { StringLengthMinErrorMessage, "The {2} must be a string with a minimum length of {4}." },
            { StringLengthMaxErrorMessage, "The {2} must be a string with a maximum length of {4}." },
            { StringLengthRangeErrorMessage, "The {2} must be a string with length between {4} and {5}." },
            { UrlErrorMessage, "The {2} must be a valid HTTP, HTTPS, or FTP URL." },
            { StrictRangeErrorMessage, "The {2} must be strictly between {4} and {5}." }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractLocalizedTextProvider"/> class.
        /// </summary>
        /// <param name="next">The next node in the chain of responsibility.</param>
        public ContractLocalizedTextProvider( ContractLocalizedTextProvider next )
            : base( next )
        {
        }

        /// <summary>
        /// Formats an error message with concrete values.
        /// </summary>
        /// <param name="errorMessage">Message to be formatted and passed into the created exception</param>
        /// <param name="value">Value assigned to the location.</param>
        /// <param name="targetName">The name of the declaration being validated (or <c>null</c> if a return value is being validated).</param>
        /// <param name="targetKind">The kind of declaration being validated.</param>
        /// <param name="additionalArguments">Optional arguments to be used in the message formatting</param>
        /// <returns>A string derived from <c>errorMessage</c>, where placeholders have been
        /// replaced by their concrete value.</returns>
        internal string FormatMessage( string errorMessage, object value, string? targetName, ContractTargetKind targetKind, object[] additionalArguments )
        {
            if ( errorMessage == null )
            {
                throw new ArgumentNullException( nameof( errorMessage ) );
            }

            var arguments = GetFormattingStringArguments( value, targetName, targetKind, additionalArguments );
            return this.FormatString( errorMessage, arguments );
        }

        /// <summary>
        /// Returns an array of arguments that can be passed to the <see cref="string.Format(string,object[])"/> method
        /// </summary>
        /// <param name="value">The incorrect value (passed, assigned or returned).</param>
        /// <param name="targetName">The name of the declaration being validated (or <c>null</c> if a return value is being validated).</param>
        /// <param name="targetKind">The kind of declaration being validated.</param>
        /// <param name="additionalArguments">Additional arguments appended to the array of arguments.</param>
        /// <returns>An array of arguments that can be passed to the <see cref="string.Format(string,object[])"/> method,
        /// where the formatting strings can have parameters as described in the remarks of
        /// the documentation of the <see cref="ContractLocalizedTextProvider"/> class.</returns>
        public static object[] GetFormattingStringArguments(object value, string? targetName, ContractTargetKind targetKind, object[] additionalArguments)
        {
            if ( additionalArguments == null )
            {
                additionalArguments = Array.Empty<object>();
            }

            object[] arguments = new object[additionalArguments.Length + 4];

            arguments[0] = targetName;
            arguments[1] = targetKind.GetDisplayName();
            arguments[2] = targetKind.GetDisplayName( targetName );
            arguments[3] = value;

            Array.Copy( additionalArguments, 0, arguments, 4, additionalArguments.Length );

            return arguments;
        }

        /// <summary>
        /// Identifier of the error message shown when <see cref="CreditCardAttribute"/> constraint is violated.
        /// </summary>
        public const string CreditCardErrorMessage = "CreditCardErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="EmailAddressAttribute"/> constraint is violated.
        /// </summary>
        public const string EmailAddressErrorMessage = "EmailAddressErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="EnumDataTypeAttribute"/> constraint is violated.
        /// </summary>
        public const string EnumDataTypeErrorMessage = "EnumDataTypeErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="GreaterThanAttribute"/> constraint is violated.
        /// </summary>
        public const string GreaterThanErrorMessage = "GreaterThanErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="LessThanAttribute"/> constraint is violated.
        /// </summary>
        public const string LessThanErrorMessage = "LessThanErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="LocationContractAttribute"/> constraint is violated.
        /// </summary>
        public const string LocationContractErrorMessage = "LocationContractErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="NotEmptyAttribute"/> constraint is violated.
        /// </summary>
        public const string NotEmptyErrorMessage = "NotEmptyErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="NotNullAttribute"/> constraint is violated.
        /// </summary>
        public const string NotNullErrorMessage = "NotNullErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="PhoneAttribute"/> constraint is violated.
        /// </summary>
        public const string PhoneErrorMessage = "PhoneErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="RangeAttribute"/> constraint is violated.
        /// </summary>
        public const string RangeErrorMessage = "RangeErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="RangeAttribute"/> constraint is violated.
        /// </summary>
        public const string StrictRangeErrorMessage = "StrictRangeErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="RegularExpressionAttribute"/> constraint is violated.
        /// </summary>
        public const string RegularExpressionErrorMessage = "RegularExpressionErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="RequiredAttribute"/> constraint is violated.
        /// </summary>
        public const string RequiredErrorMessage = "RequiredErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="StrictlyGreaterThanAttribute"/> constraint is violated.
        /// </summary>
        public const string StrictlyGreaterThanErrorMessage = "StrictlyGreaterThanErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="StrictlyLessThanAttribute"/> constraint is violated.
        /// </summary>
        public const string StrictlyLessThanErrorMessage = "StrictlyLessThanErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="StringLengthAttribute"/> constraint is violated (only minimum length was specified).
        /// </summary>
        public const string StringLengthMinErrorMessage = "StringLengthMinErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="StringLengthAttribute"/> constraint is violated (only maximum length was specified).
        /// </summary>
        public const string StringLengthMaxErrorMessage = "StringLengthMaxErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="StringLengthAttribute"/> constraint is violated (both minimum and maximum was specified).
        /// </summary>
        public const string StringLengthRangeErrorMessage = "StringLengthRangeErrorMessage";

        /// <summary>
        /// Identifier of the error message shown when <see cref="UrlAttribute"/> constraint is violated.
        /// </summary>
        public const string UrlErrorMessage = "UrlErrorMessage";

        /// <summary>
        /// Gets a message template using <see cref="GetMessage"/> and formats it using the values in exceptionInfo.
        /// </summary>
        /// <param name="exceptionInfo">Information about the exception to be created.</param>
        /// <returns>A formatted message that uses the formatting parameters described in the remarks of
        /// the documentation of the <see cref="ContractLocalizedTextProvider"/> class.
        /// </returns>
        public string GetFormattedMessage( ContractExceptionInfo exceptionInfo )
        {
            var errorMessageTemplate = this.GetMessage( exceptionInfo.MessageId );
            var errorMessage = this.FormatMessage( errorMessageTemplate, exceptionInfo.Value, exceptionInfo.TargetName, exceptionInfo.TargetKind, exceptionInfo.MessageArguments );

            return errorMessage;
        }

        /// <inheritdoc />
        public override string GetMessage( string messageId )
        {
            if ( string.IsNullOrEmpty( messageId ) )
            {
                throw new ArgumentNullException( nameof(messageId) );
            }

            if ( this._messages.TryGetValue( messageId, out string message ) )
            {
                return message;
            }
            else
            {
                return base.GetMessage( messageId );
            }
        }
    }
}
