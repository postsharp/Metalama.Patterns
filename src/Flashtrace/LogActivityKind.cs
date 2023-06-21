// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.


namespace PostSharp.Patterns.Diagnostics
{
    /// <summary>
    /// Kinds of <see cref="LogActivity"/>.
    /// </summary>
    public enum LogActivityKind
    {
        /// <summary>
        /// Default, created by <see cref="Logger.OpenActivity(PostSharp.Patterns.Diagnostics.LogActivityOptions,string)"/> or <see cref="Logger.OpenAsyncActivity(PostSharp.Patterns.Diagnostics.LogActivityOptions,string)"/>.
        /// </summary>
        Default,

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

}