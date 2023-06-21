// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.ComponentModel;
using PostSharp.Patterns.Diagnostics.Contexts;
using PostSharp.Patterns.Diagnostics.Custom.Messages;

namespace PostSharp.Patterns.Diagnostics
{
    
    /// <summary>
    /// Exposes the public API of the generic value type <see cref="LogActivity{TActivityDescription}"/>.
    /// </summary>
    public interface ILogActivity : IDisposable
    {
        /// <summary>
        /// Gets the <see cref="ILoggingContext"/> corresponding to the current activity.
        /// </summary>
        ILoggingContext Context { get; }

        /// <summary>
        /// Closes the activity with success and sets no other outcome message than <c>Succeeded</c>.
        /// </summary>
        /// <param name="options">Options.</param>
        void SetSuccess(in CloseActivityOptions options = default);

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        void SetSuccess(in CloseActivityOptions options, ref CallerInfo callerInfo);

        /// <summary>
        /// Closes the activity with success and sets includes a result in the outcome message.
        /// </summary>
        /// <param name="result">The result of the activity.</param>
        /// <param name="options">Options.</param>
        void SetResult<TResult>(TResult result, in CloseActivityOptions options = default);

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        void SetResult<TResult>(TResult result, in CloseActivityOptions options, ref CallerInfo callerInfo);

        /// <summary>
        /// Closes the activity and sets its outcome.
        /// </summary>
        /// <typeparam name="TMessage">Type of the <paramref name="message"/> parameter.</typeparam>
        /// <param name="level">Level of the outcome message.</param>
        /// <param name="message">The description of the activity outcome, typically created using the <see cref="PostSharp.Patterns.Diagnostics.SemanticMessageBuilder"/> or <see cref="PostSharp.Patterns.Diagnostics.FormattedMessageBuilder"/> class.</param>
        /// <param name="exception">An optional <see cref="System.Exception"/>.</param>
        /// <param name="options">Options.</param>
        void SetOutcome<TMessage>( LogLevel level, in TMessage message, Exception exception = null, in CloseActivityOptions options = default )
            where TMessage : IMessage;

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        void SetOutcome<TMessage>( LogLevel level, in TMessage message, Exception exception, in CloseActivityOptions options, ref CallerInfo callerInfo)
            where TMessage : IMessage;

        /// <summary>
        /// Closes the activity with an <see cref="System.Exception"/>.
        /// </summary>
        /// <param name="exception">An <see cref="System.Exception"/>.</param>
        /// <param name="options">Options.</param>
        void SetException(Exception exception, in CloseActivityOptions options = default);

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        void SetException( Exception exception, in CloseActivityOptions options, ref CallerInfo callerInfo );

#pragma warning disable CA1716 // Identifiers should not match keywords
        /// <summary>
        /// Resumes the current async activity after it has been suspended by a call to <see cref="PostSharp.Patterns.Diagnostics.LogActivity{TActivityDescription}.Suspend()"/>. There is typically no need
        /// to invoke this method in user code because all async methods that use the <see cref="PostSharp.Patterns.Diagnostics.Logger"/> class are automatically instrumented.
        /// </summary>
        void Resume();

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Resume(ref CallerInfo callerInfo);
#pragma warning restore CA1716 // Identifiers should not match keywords

        /// <summary>
        /// Suspends the current async activity.
        /// The activity must than be resumed by a call of the <see cref="PostSharp.Patterns.Diagnostics.LogActivity{TActivityDescription}.Resume()"/> method.
        /// There is typically no need to invoke this method in user code because all async methods that use the <see cref="PostSharp.Patterns.Diagnostics.Logger"/> class are automatically instrumented.
        /// </summary>
        void Suspend();

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Suspend(ref CallerInfo callerInfo);
    }
}