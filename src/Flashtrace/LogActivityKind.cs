// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Kinds of <see cref="LogActivity{TActivityDescription}"/>.
/// </summary>
[PublicAPI]
public enum LogActivityKind
{
    // TODO: Check actual uses of default, update "created by" below.
    
    // ReSharper disable InvalidXmlDocComment
    
    /// <summary>
    /// Default, created by <see cref="LogLevelSource.OpenActivity{T}(in T, in OpenActivityOptions)"/> or <see cref="LogLevelSource.LogActivityAsync{TDescription, TResult}(in TDescription, Func{Task{TResult}}, in OpenActivityOptions)"/>.
    /// </summary>
    Default,
    
    // ReSharper restore InvalidXmlDocComment
    
    /// <summary>
    /// Activity of creating a <see cref="Task"/>.
    /// </summary>
    TaskLauncher,

    /// <summary>
    /// Code running in a <see cref="Task"/>.
    /// </summary>
    Task,

    /// <summary>
    /// Activity of waiting for a <see cref="Task"/> or another dispatcher object.
    /// </summary>
    Wait,

    /// <summary>
    /// An outgoing request. The property "RequestId" is expected to be defined.
    /// </summary>
    OutgoingRequest,

    /// <summary>
    /// An incoming request of the service (typically a web request). A transaction boundary.
    /// </summary>
    IncomingRequest,

    /// <summary>
    /// A custom transaction.
    /// </summary>
    Transaction
}