// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using JetBrains.Annotations;

namespace Flashtrace;

// TODO: [FT-Review] Is 'contact' intended in the comment below? Maybe 'context' or 'local context' instead? What does 'context-local' actually mean?
/// <summary>
/// Abstraction of custom logging methods where the contact has already been resolved.
/// </summary>
[PublicAPI]
public interface IContextLocalLogger : ILoggerExceptionHandler
{
    /// <summary>
    /// Determines whether logging is enabled for a given <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="level">A record level (or severity).</param>
    /// <returns><c>true</c> if logging is enabled for <paramref name="level"/>, otherwise <c>false</c>.</returns>
    bool IsEnabled( LogLevel level );

    /// <summary>
    /// Opens a new context for a custom activity.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="callerInfo">Information about the caller code.</param>
    /// <returns>A new context for the custom activity.</returns>
    ILoggingContext OpenActivity( in OpenActivityOptions options, ref CallerInfo callerInfo );

    /// <summary>
    /// Gets a record builder.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="callerInfo">Information about the caller code.</param>
    /// <param name="context">The context for which the record will be built, or <c>null</c> for standalone records.</param>
    /// <returns>A record builder.</returns>
    ICustomLogRecordBuilder GetRecordBuilder( in CustomLogRecordOptions options, ref CallerInfo callerInfo, ILoggingContext? context = null );

    /// <summary>
    /// Resumes an asynchronous activity suspended by the <see cref="SuspendActivity(ILoggingContext,ref CallerInfo)"/> method.
    /// </summary>
    /// <param name="context">A context representing an asynchronous custom activity, created by <see cref="OpenActivity"/>
    /// and suspended by <see cref="SuspendActivity(ILoggingContext,ref CallerInfo)"/>.</param>
    /// <param name="callerInfo">Information about the caller code.</param>
    void ResumeActivity( ILoggingContext context, ref CallerInfo callerInfo );

    /// <summary>
    /// Suspends an asynchronous activity, which can then be resumed by the <see cref="ResumeActivity(ILoggingContext,ref CallerInfo)"/> method.
    /// </summary>
    /// <param name="context">A context representing an asynchronous custom activity, created by <see cref="OpenActivity"/>.</param>
    /// <param name="callerInfo">Information about the caller code.</param>
    void SuspendActivity( ILoggingContext context, ref CallerInfo callerInfo );

    /// <summary>
    /// Sets the wait dependency for a given context, i.e. give information about what the given context is waiting (or awaiting) for.
    /// </summary>
    /// <param name="context">The waiting context.</param>
    /// <param name="waited">The "thing" that is awaited for. Typically a <see cref="System.Threading.Tasks.Task"/>, or <c>TaskInfo</c>, or another context.</param>
    void SetWaitDependency( ILoggingContext context, object waited );
}