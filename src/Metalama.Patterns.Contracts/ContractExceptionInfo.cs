// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Contracts;

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
    /// Gets the value that applies the target declaration.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Gets the name of the target declaration, or <see langword="null"/> for return values.
    /// </summary>
    public string? TargetName { get; }

    /// <summary>
    /// Gets the kind of declaration to which the exception applies.
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
        object? value,
        string? targetName,
        ContractTargetKind targetKind,
        ContractDirection direction,
        string messageId,
        object[] messageArguments )
    {
        this.ExceptionType = exceptionType;
        this.AspectType = aspectType;
        this.Value = value;
        this.TargetName = targetName;
        this.TargetKind = targetKind;
        this.Direction = direction;
        this.MessageId = messageId;
        this.MessageArguments = messageArguments;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContractExceptionInfo"/> class.
    /// </summary>
    /// <param name="exceptionType">Requested <see cref="Type"/> of the exception that should be created. <see cref="PostconditionFailedException"/> will be used instead when <see cref="direction"/> is <see cref="ContractDirection.Output"/>.</param>
    /// <param name="aspectType">The type of the contract aspect.</param>
    /// <param name="value">The value that applies to the target.</param>
    /// <param name="targetName">Name of the target declaration.</param>
    /// <param name="targetKind">The target kind.</param>
    /// <param name="direction">The direction of data flow.</param>
    /// <param name="messageId">The id of the error message template to be used in the exception.</param>
    /// <param name="messageArguments">Any additional parameters to be used in the exception message formatting.</param>
    public static ContractExceptionInfo Create( 
        Type exceptionType,
        Type aspectType,
        object value,
        string? targetName,
        ContractTargetKind targetKind,
        ContractDirection direction,
        string messageId,
        params object[] messageArguments )
    {
        if ( direction == ContractDirection.Output )
        {
            exceptionType = typeof(PostconditionFailedException);
        }

        if ( string.IsNullOrEmpty( targetName ) && targetKind != ContractTargetKind.ReturnValue )
        {
            throw new ArgumentNullException( nameof(targetName) );
        }

        return new ContractExceptionInfo(
            exceptionType ?? throw new ArgumentNullException( nameof(exceptionType) ),
            aspectType ?? throw new ArgumentNullException( nameof(aspectType) ),
            value,
            targetName,
            targetKind,
            direction,
            messageId ?? throw new ArgumentNullException( nameof(messageId) ),
            messageArguments );
    }
}