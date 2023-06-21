// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;

namespace Flashtrace.Custom
{
    /// <summary>
    /// Abstraction of the Logging facility, through which other components emit their log records. This
    /// interfaces abstracts other PostSharp components from the PostSharp.Patterns.Diagnostics component.
    /// </summary>
    /// <remarks>
    /// <para>If you want to implement this interface, you should also implement the <see cref="ILoggerFactory"/> interface
    /// and register it to the <see cref="PostSharp.Patterns.Utilities.ServiceLocator"/>.</para>
    /// </remarks>
    public partial interface ILogger
    {
        /// <summary>
        /// Gets the source <see cref="Type"/> for which this <see cref="ILogger"/> was created.
        /// </summary>
        [Obsolete( "Don't use the Type property as some logger may have no type assigned depending on the initialization order." )]
        Type Type { get; }

        /// <summary>
        /// Gets the role of records created by this <see cref="ILogger"/>. A list of standard roles is available in the <see cref="LoggingRoles"/> class.
        /// </summary>
        string Role { get; }

        /// <summary>
        /// Determines whether logging is enabled for a given <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="level">A record level (or severity).</param>
        /// <returns><c>true</c> if logging is enabled for <paramref name="level"/>, otherwise <c>false</c>.</returns>
        bool IsEnabled( LogLevel level );

        /// <summary>
        /// Gets the default verbosity when opening and closing activities.
        /// </summary>
        ILogActivityOptions ActivityOptions { get; }

        /// <summary>
        /// Determines whether calls of <see cref="SuspendActivity"/> and <see cref="ResumeActivity"/> 
        /// is required for asynchronous custom activities in the current context.
        /// </summary>
        bool RequiresSuspendResume { get; }

        /// <summary>
        /// Writes a custom log record with a description without parameters.
        /// </summary>
        /// <param name="level"><see cref="LogLevel"/> of the record.</param>
        /// <param name="context"></param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="text">Text of the record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="callerInfo">Information about the caller source code.</param>
        void Write( ILoggingContext context, LogLevel level, LogRecordKind recordKind, string text, Exception exception, ref CallerInfo callerInfo );

        /// <summary>
        /// Writes a custom log record with a description with an array of parameters.
        /// </summary>
        /// <param name="level"><see cref="LogLevel"/> of the record.</param>
        /// <param name="context"></param>
        /// <param name="recordKind">Kind of record.</param>
        /// <param name="text">Text of the record.</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record, or <c>null</c>.</param>
        /// <param name="callerInfo">Information about the caller source code.</param>
        /// <param name="args">An array of parameters.</param>
        void Write(
            ILoggingContext context,
            LogLevel level,
            LogRecordKind recordKind,
            string text,
            object[] args,
            Exception exception,
            ref CallerInfo callerInfo );

        /// <summary>
        /// Opens an activity.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="callerInfo">Information about the caller source code.</param>
        /// <returns>An <see cref="ILoggingContext"/> representing the new activity.</returns>
        ILoggingContext OpenActivity( LogActivityOptions options, ref CallerInfo callerInfo );

        /// <summary>
        /// Resumes an asynchronous activity suspended by the <see cref="SuspendActivity"/> method.
        /// </summary>
        /// <param name="context">A context representing an asynchronous custom activity, created by <see cref="OpenActivity"/>
        /// and suspended by <see cref="SuspendActivity"/>.</param>
        /// <param name="callerInfo">Information about the caller source code.</param>
        void ResumeActivity( ILoggingContext context, ref CallerInfo callerInfo );

        /// <summary>
        /// Suspends an asynchronous activity, which can then be resumed by the <see cref="ResumeActivity"/> method.
        /// </summary>
        /// <param name="context">A context representing an asynchronous custom activity, created by <see cref="OpenActivity"/>.</param>
        /// <param name="callerInfo">Information about the caller source code.</param>
        void SuspendActivity( ILoggingContext context, ref CallerInfo callerInfo );

        /// <summary>
        /// Sets the wait dependency for a given context, i.e. give information about what the given context is waiting (or awaiting) for.
        /// </summary>
        /// <param name="context">The waiting context.</param>
        /// <param name="waited">The "thing" that is awaited for. Typically a <see cref="System.Threading.Tasks.Task"/>, or <c>TaskInfo</c>, or another context.</param>
        void SetWaitDependency( ILoggingContext context, object waited );
    }
}