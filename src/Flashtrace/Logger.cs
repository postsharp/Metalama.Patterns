// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Custom;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Flashtrace
{
    /// <summary>
    /// Allows to emit custom log records and define custom activities. This class is obsolete. Use <see cref="LogSource" /> for new developments.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    [SuppressMessage("Microsoft.Design", "CA1001")] // Loggers are meant to be forever - not disposable.
    [SuppressMessage("Microsoft.Design", "CA1031")]
    [RequirePostSharp(typeof(AwaitInstrumentationAspectProvider), AnyTypeReference = true)]
    [RequirePostSharp("PostSharp.Patterns.Common.Weaver", "AddCallerInfoTask", AnyTypeReference = true)]
    public partial class Logger
    {
        internal ILogger logger;
        internal static CallerInfo nullRecordInfo = default;

        private LogActivity nullActivity;

        private protected Logger(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            this.logger = logger;
        }



        private LogActivity CreateActivity(ILoggingContext context)
        {
            return new LogActivity(this.logger, context);
        }

        private LogActivity GetNullActivity()
        {
            if (this.nullActivity == null)
            {
                this.nullActivity = new LogActivity(this.logger, null);
            }

            return this.nullActivity;
        }




        private protected LogLevel DefaultLevel => this.logger.ActivityOptions.ActivityLevel;

        [SuppressMessage("Microsoft.Design", "CA1031")]
        private void DisposeSafe(IDisposable disposable)
        {
            if (disposable != null)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    this.ExceptionHandler?.OnInternalException(e);
                }
            }
        }


        ///  <exclude/>
        public bool IsOpenActivityEnabled => this.logger?.IsEnabled(this.logger.ActivityOptions.ActivityLevel) ?? false;

        /// <summary>
        /// Gets a <see cref="Logger"/> for a given role and <see cref="Type"/>.
        /// </summary>
        /// <param name="role">The role name (see <see cref="LoggingRoles"/> for a list of standard logging roles).</param>
        /// <param name="type">The type that will emit the records.</param>
        /// <returns>A <see cref="Logger"/> for <paramref name="role"/> and <paramref name="type"/>.</returns>
        [Obsolete("Use the LogSource class with new developments. Disable the warning with #pragma for existing code.")]
        public static Logger GetLogger(string role, Type type)
        {
            return new Logger(ServiceLocator.GetService<ILoggerFactory>()?.GetLogger(role, type));
        }

        /// <summary>
        /// Gets a <see cref="Logger"/> for a given role and for the calling type.
        /// </summary>
        /// <param name="role">The role name (see <see cref="LoggingRoles"/> for a list of standard logging roles). The default value is <c>Custom</c>.</param>
        /// <returns>A <see cref="Logger"/> for <paramref name="role"/> and the calling type.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1801")]
        [Obsolete("Use the LogSource class with new developments. Disable the warning with #pragma for existing code.")]
        public static Logger GetLogger(string role = "Custom")
        {
            throw new InvalidOperationException("The current assembly has not been enhanced by PostSharp Diagnostics. Make sure the package PostSharp.Patterns.Diagnostics is referenced.");
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use the LogSource class.")]
        public static Logger GetLogger(string role, ref CallerInfo callerInfo)
        {
            if (callerInfo.SourceType == null)
                throw new ArgumentNullException(nameof(callerInfo));

            return GetLogger(role, callerInfo.SourceType);
        }

        internal ILoggerExceptionHandler ExceptionHandler => this.logger as ILoggerExceptionHandler;

        private bool ValidateLogger(LogLevel level)
        {
            return this.logger != null && this.logger.IsEnabled(level);
        }



        /// <summary>
        /// Writes a custom log record without parameters.
        /// </summary>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record.</param>
        public void Write(LogLevel level, string text)
        {
            this.Write(level, text, ref nullRecordInfo);
        }


        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void Write(LogLevel level, string text, ref CallerInfo callerInfo)
        {
            if (!this.ValidateLogger(level))
                return;

            try
            {
                this.logger.Write(null, level, LogRecordKind.CustomRecord, text, null, ref callerInfo);
            }
            catch (Exception e)
            {
                this.ExceptionHandler?.OnInternalException(e);
            }
        }


        /// <summary>
        /// Writes a custom log record with an array of parameters.
        /// </summary>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Info"/> or <see cref="LogLevel.Warning"/>).</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode.</c>)</param>
        /// <param name="args">An array of parameters.</param>
        public void Write(LogLevel level, string text, params object[] args)
        {
            this.Write(level, text, args, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void Write(LogLevel level, string text, object[] args, ref CallerInfo callerInfo)
        {
            if (!this.ValidateLogger(level))
                return;

            try
            {
                this.logger.Write(null, level, LogRecordKind.CustomRecord, text, args, null, ref callerInfo);
            }
            catch (Exception e)
            {
                this.ExceptionHandler?.OnInternalException(e);
            }
        }

        /// <summary>
        /// Writes a custom record without parameters and associates it with an <see cref="Exception"/>.
        /// </summary>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Warning"/> or <see cref="LogLevel.Error"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record.</param>
        /// <param name="text">The text of the log record.</param>
        public void WriteException(LogLevel level, Exception exception, string text)
        {
            this.WriteException(level, exception, text, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void WriteException(LogLevel level, Exception exception, string text, ref CallerInfo callerInfo)
        {
            if (!this.ValidateLogger(level))
                return;

            try
            {
                this.logger.Write(null, level, LogRecordKind.CustomRecord, text, exception, ref callerInfo);
            }
            catch (Exception e)
            {
                this.ExceptionHandler?.OnInternalException(e);
            }
        }


        /// <summary>
        /// Writes a custom record with an array of parameters and associates it with an <see cref="Exception"/>.
        /// </summary>
        /// <param name="level">The severity of the record (e.g. <see cref="LogLevel.Warning"/> or <see cref="LogLevel.Error"/>).</param>
        /// <param name="exception">The <see cref="Exception"/> associated with the record.</param>
        /// <param name="text">The text of the log record, including parameters (e.g. <c>Opening {Path} file {ShareMode} sharing mode.</c>)</param>
        /// <param name="args">An array of parameters.</param>
        public void WriteException(LogLevel level, Exception exception, string text, object[] args)
        {
            this.WriteException(level, exception, text, args, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void WriteException(LogLevel level, Exception exception, string text, object[] args, ref CallerInfo callerInfo)
        {
            if (exception == null)
            {
                this.ExceptionHandler?.OnInvalidUserCode(ref callerInfo, "The exception parameter cannot be null.");
                return;
            }

            if (!this.ValidateLogger(level))
                return;

            try
            {
                this.logger.Write(null, level, LogRecordKind.CustomRecord, text, args, exception, ref callerInfo);
            }
            catch (Exception e)
            {
                this.ExceptionHandler?.OnInternalException(e);
            }
        }


        #region OpenActivity

        private static void SetOptions(ref LogActivityOptions options, ref CallerInfo callerInfo)
        {
            if ((callerInfo.Attributes & CallerAttributes.IsAsync) != 0)
            {
                options.IsAsync = true;
            }
        }
        /// <excludeOverload />
        [MethodImpl(MethodImplOptions.NoInlining)]
        public LogActivity OpenActivity(LogActivityOptions options, string text = null)
        {
            CallerInfo callerInfo = CallerInfo.GetDynamic(1);
            return this.OpenActivity( options, text, ref callerInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public LogActivity OpenActivity(LogActivityOptions options, string text, ref CallerInfo callerInfo)
        {
            ILoggingContext activity = null;
            try
            {
                LogLevel level = this.DefaultLevel;

                if (!this.IsEnabled(level))
                {
                    return this.GetNullActivity();
                }

                SetOptions(ref options, ref callerInfo);

                activity = this.logger.OpenActivity(options, ref callerInfo);

                this.logger.Write(activity, level, LogRecordKind.CustomActivityEntry,
                                   text ?? callerInfo.MethodName ?? "Unnamed activity",
                                   null, ref callerInfo);

                return this.CreateActivity(activity);
            }
            catch (Exception e)
            {
                this.DisposeSafe(activity);

                this.ExceptionHandler?.OnInternalException(e);
                return this.GetNullActivity();
            }
        }



        /// <excludeOverload />
        [MethodImpl(MethodImplOptions.NoInlining)]
        public LogActivity OpenActivity(LogActivityOptions options, string text, params object[] args)
        {
            CallerInfo callerInfo = CallerInfo.GetDynamic(1);
            return this.OpenActivity(options, text, args, ref callerInfo);
        }


        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public LogActivity OpenActivity(LogActivityOptions options, string text, object[] args, ref CallerInfo callerInfo)
        {
            LogLevel level = this.DefaultLevel;

            if (!this.IsEnabled(level))
            {
                return this.GetNullActivity();
            }
            ILoggingContext activity = null;


            try
            {
                SetOptions(ref options, ref callerInfo);

                activity = this.logger.OpenActivity(options, ref callerInfo);
                this.logger.Write(activity, level, LogRecordKind.CustomActivityEntry, text, args, null, ref callerInfo);
                return this.CreateActivity(activity);
            }
            catch (Exception e)
            {
                this.DisposeSafe(activity);

                this.ExceptionHandler?.OnInternalException(e);
                return this.GetNullActivity();
            }
        }


        /// <exclude />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public LogActivity OpenAsyncActivity(LogActivityOptions options, string text, ref CallerInfo callerInfo)
        {
            LogLevel level = this.DefaultLevel;

            if (!this.IsEnabled(level))
            {
                return this.GetNullActivity();
            }

            ILoggingContext activity = null;

            try
            {
                options.IsAsync = true;
                activity = this.logger.OpenActivity(options, ref callerInfo);
                this.logger.Write(activity, level, LogRecordKind.CustomActivityEntry, text, null, ref callerInfo);
                return this.CreateActivity(activity);
            }
            catch (Exception e)
            {
                this.DisposeSafe(activity);
                this.ExceptionHandler?.OnInternalException(e);

                return this.GetNullActivity();
            }
        }
        #endregion

        #region OpenActivity without options
        /// <summary>
        /// Opens a custom activity with an optional description, but without parameters. 
        /// </summary>
        /// <param name="text">A description of the activity, or <c>null</c> to use the caller method name as the activity description.</param>
        /// <returns>A <see cref="Logger"/> representing the new activity.</returns>
        /// <remarks>The activity must be closed using <see cref="LogActivity.SetSuccess(string)"/>, <see cref="LogActivity.SetFailure(String)"/> or <see cref="LogActivity.SetException(Exception)"/>. </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public LogActivity OpenActivity(string text = null)
        {
            CallerInfo callerInfo = CallerInfo.GetDynamic(1);
            return this.OpenActivity(text, ref callerInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LogActivity OpenActivity(string text, ref CallerInfo callerInfo)
        {
            return this.OpenActivity(LogActivityOptions.Default, text, ref callerInfo);
        }



        /// <summary>
        /// Opens a custom activity with an array of parameters. 
        /// </summary>
        /// <param name="text">The description of the activity, including parameters (e.g. <c>Writing {LineCount} line(s) in file {Path}.</c>)</param>
        /// <param name="args">An array of parameters.</param>
        /// <returns>A <see cref="Logger"/> representing the new activity.</returns>
        /// <remarks>The activity must be closed using <see cref="LogActivity.SetSuccess(string)"/>, <see cref="LogActivity.SetFailure(String)"/> or <see cref="LogActivity.SetException(Exception)"/>. </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public LogActivity OpenActivity(string text, params object[] args)
        {
            CallerInfo callerInfo = CallerInfo.GetDynamic(1);
            return this.OpenActivity(text, args, ref callerInfo);
        }


        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public LogActivity OpenActivity(string text, object[] args, ref CallerInfo callerInfo)
        {
            return this.OpenActivity(LogActivityOptions.Default, text, args, ref callerInfo);
        }
        #endregion

        /// <summary>
        /// Determines whether logging is enabled in the current <see cref="Logger"/> for a given <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="level">A <see cref="LogLevel"/>.</param>
        /// <returns><c>true</c> if logging is enabled for <paramref name="level"/>, otherwise <c>false</c>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public bool IsEnabled(LogLevel level)
        {
            if (this.logger == null)
                return false;

            try
            {
                return this.logger.IsEnabled(level);
            }
            catch (Exception e)
            {
                this.ExceptionHandler?.OnInternalException(e);
                return false;
            }
        }

        /// <summary>
        /// Emits a log record with the source file and line of the caller.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void WriteExecutionPoint()
        {
            // TODO: better error message.
            throw new InvalidOperationException("The method call should have been transformed.");
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void WriteExecutionPoint(ref CallerInfo callerInfo)
        {
            if (!this.ValidateLogger(LogLevel.Debug))
                return;

            try
            {
                this.logger.Write(null, LogLevel.Debug, LogRecordKind.ExecutionPoint, "Executing", null, ref callerInfo);
            }
            catch (Exception e)
            {
                this.ExceptionHandler?.OnInternalException(e);
            }
        }

        /// <summary>
        /// Creates a <see cref="LogSource"/> from the current legacy <see cref="Logger"/>.
        /// </summary>
        /// <returns></returns>
        public LogSource ToLogSource()
        {
            return new LogSource( (ILogger3) this.logger, this.DefaultLevel, this.logger.ActivityOptions.ExceptionLevel);
        }
    }


}


