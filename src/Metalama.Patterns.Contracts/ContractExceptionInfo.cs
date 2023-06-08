// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts
{
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractExceptionInfo"/> class.
        /// </summary>
        /// <param name="exceptionType">Requested Type of the exception that should be created.</param>
        /// <param name="value">Value being assigned to the location.</param>
        /// <param name="locationName">Name of the location.</param>
        /// <param name="targetKind">The target kind.</param>
        /// <param name="direction">The direction of data flow.</param>
        /// <param name="messageId">The id of the error message template to be used in the exception.</param>
        /// <param name="messageArguments">Any additional parameters to be used in the exception message formatting.</param>
        public ContractExceptionInfo(
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
    }
}