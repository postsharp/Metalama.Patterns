// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts
{
    // TODO: Review terminology "application element" which is consistent with AttributeTargets.

    /// <summary>
    /// This class holds the information from which the <see cref="ContractExceptionFactory"/> should create the exception.
    /// </summary>
    public class ContractExceptionInfo
    {
        /// <summary>
        /// Gets the type of the exception that should be created.
        /// </summary>
        public Type ExceptionType { get; }

        /// <summary>
        /// Gets the type of the aspect that requested the exception.
        /// </summary>
        public Type AspectType { get; }

        /// <summary>
        /// Gets the value traversing the target application element.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the name of the target application element.
        /// </summary>
        public string TargetName { get; }

        /// <summary>
        /// Gets the kind of application element to which the exception applies.
        /// </summary>
        public ContractTargetKind TargetKind { get; }

        /// <summary>
        /// Gets the direction of data flow to which the exception applies.
        /// </summary>
        public ContractDirection Direction { get; }

        /// <summary>
        /// Gets the id of the error message to be used in the exception message formatting.
        /// </summary>
        public string MessageId { get; }

        /// <summary>
        /// Gets the additional parameters to be used in the exception message formatting.
        /// </summary>
        public object[] MessageArguments { get; }

        private ContractExceptionInfo(
            Type exceptionType,
            Type aspectType,
            object value,
            string locationName,
            ContractTargetKind targetKind,
            ContractDirection direction,
            string messageId,
            object[] messageArguments )
        {
            this.ExceptionType = exceptionType ?? throw new InvalidOperationException( "Exception type must be set" );
            this.AspectType = aspectType ?? throw new InvalidOperationException( "Calling aspect type must be specified" );
            this.Value = value;
            this.TargetName = locationName ?? throw new InvalidOperationException( "Location name must be specified" );
            this.TargetKind = targetKind;
            this.Direction = direction;
            this.MessageId = messageId ?? throw new InvalidOperationException( "Message ID must be specified" );
            this.MessageArguments = messageArguments;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractExceptionInfo"/> class.
        /// </summary>
        /// <param name="exceptionType">Requested <see cref="Type"/> of the exception that should be created. <see cref="PostconditionFailedException"/> will be used instead when <see cref="Direction"/> is <see cref="ContractDirection.Output"/>.</param>
        /// <param name="aspectType"></param>
        /// <param name="value">The value traversing the target.</param>
        /// <param name="targetName">Name of the target.</param>
        /// <param name="targetKind">The target kind.</param>
        /// <param name="direction">The direction of data flow.</param>
        /// <param name="messageId">The id of the error message template to be used in the exception.</param>
        /// <param name="messageArguments">Any additional parameters to be used in the exception message formatting.</param>
        public static ContractExceptionInfo Create(
            Type exceptionType,
            Type aspectType,
            object value,
            string targetName,
            ContractTargetKind targetKind,
            ContractDirection direction,
            string messageId,
            params object[] messageArguments )
        {
            if ( direction == ContractDirection.Output )
            {
                exceptionType = typeof( PostconditionFailedException );
            }

            return new ContractExceptionInfo(
                exceptionType,
                aspectType,
                value,
                targetName,
                targetKind,
                direction,
                messageId,
                messageArguments );
        }
    }
}