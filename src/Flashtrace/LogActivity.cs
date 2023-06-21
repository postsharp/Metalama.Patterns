// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using PostSharp.Extensibility;
using PostSharp.Patterns.Diagnostics.Contexts;
using PostSharp.Patterns.Diagnostics.Custom;

namespace PostSharp.Patterns.Diagnostics
{
    /// <summary>
    /// Represents a logged custom activity, i.e. something that a beginning and an end with a specific outcome.
    /// This class is instantiated by the legacy API of the <see cref="Logger"/> class. For the modern API, see <see cref="LogActivity{TActivityDescription}"/>.
    /// </summary>
    [RequirePostSharp("PostSharp.Patterns.Common.Weaver", "AddCallerInfoTask", AnyTypeReference = true)]
    public partial class LogActivity : Logger, IDisposable
    {
        /// <summary>
        /// Initializes a new <see cref="LogActivity"/>.
        /// </summary>
        /// <param name="logger">The underlying <see cref="ILogger"/>.</param>
        /// <param name="context">The <see cref="ILoggingContext"/> implementing the new <see cref="LogActivity"/>.</param>
        public LogActivity(ILogger logger, ILoggingContext context) : base(logger)
        {
            this.Context = context;
        }

      

#pragma warning disable CS0618 // Type or member is obsolete
        private LogLevel ResolvedSuccessLevel => this.logger.ActivityOptions.ActivityLevel;
        private LogLevel ResolvedExceptionLevel => this.logger.ActivityOptions.ExceptionLevel;
        private LogLevel ResolvedFailureLevel => this.logger.ActivityOptions.FailureLevel;
#pragma warning restore CS0618 // Type or member is obsolete

        
        /// <summary>
        /// Determines whether the current <see cref="LogActivity"/> is valid, i.e. calls to methods 
        /// <see cref="LogActivity.SetFailure()"/> or <see cref="LogActivity.SetException(Exception)"/> will do something, or will be totally ignored.
        /// </summary>
        public bool IsValid => this.Context != null && !this.Context.IsDisposed;

        private bool RequiresCustomActivity(ref CallerInfo callerInfo, bool requiresAsync = false)
        {
            if (!this.IsValid)
                return false;


            if (this.Context == null )
            {
                this.ExceptionHandler?.OnInvalidUserCode( ref callerInfo, "This operation is only valid in the context of an activity.");
                return false;
            }

        
            if (requiresAsync && !this.Context.IsAsync)
            {
                this.ExceptionHandler?.OnInvalidUserCode( ref callerInfo, "This operation is only valid in the context of an async activity.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ends an activity (opened with <see cref="Logger.OpenActivity(string)"/>)
        /// with success with the default success message.
        /// </summary>
        public void SetSuccess()
        {
            this.SetSuccess(ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void SetSuccess(ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo))
                return;

            try
            {
                this.logger?.Write(this.Context, this.ResolvedSuccessLevel, LogRecordKind.CustomActivitySuccess, null, null, ref callerInfo );
                this.Close( ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }
        }

        /// <summary>
        /// Ends an activity (opened with <see cref="Logger.OpenActivity(string)"/>)
        /// with success and specifies a parameterless success message.
        /// </summary>
        /// <param name="text">The success message.</param>
        public void SetSuccess(string text)
        {
            this.SetSuccess(text, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void SetSuccess(string text, ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo))
                return;

            try
            {
                this.logger?.Write(null, this.ResolvedSuccessLevel, LogRecordKind.CustomActivitySuccess, text, null, ref callerInfo );
                this.Close( ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

        }

        /// <summary>
        /// Ends an activity (opened with <see cref="Logger.OpenActivity(string)"/>)
        /// with success and specifies a success message with an array of parameters.
        /// </summary>
        /// <param name="text">The success message with parameters, for instance <c>Written {Count} lines</c>.</param>
        /// <param name="args">An array of parameters.</param>
        public void SetSuccess(string text, params object[] args)
        {
            this.SetSuccess(text, args, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void SetSuccess(string text, object[] args, ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo))
                return;

            try
            {
                this.logger.Write(null, this.ResolvedSuccessLevel, LogRecordKind.CustomActivitySuccess, text, args, null, ref callerInfo );
                this.Close( ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

        }


        /// <summary>
        /// Ends an activity (opened with <see cref="Logger.OpenActivity(string)"/>)
        /// with failure with the default failure message.
        /// </summary>
        public void SetFailure()
        {
            this.SetFailure(ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void SetFailure(ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo))
                return;

            try
            {
                this.logger.Write(this.Context, this.ResolvedFailureLevel, LogRecordKind.CustomActivityFailure, null, null, ref callerInfo );
                this.Close( ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

        }

        /// <summary>
        /// Ends an activity (opened with <see cref="Logger.OpenActivity(string)"/>)
        /// with failure and specifies a parameterless failure message.
        /// </summary>
        /// <param name="text">The failure message.</param>
        public void SetFailure(string text)
        {
            this.SetFailure(text, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void SetFailure(string text, ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo))
                return;

            try
            {
                this.logger.Write(this.Context, this.ResolvedFailureLevel, LogRecordKind.CustomActivityFailure, text, null, ref callerInfo );
                this.Close( ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

        }

        /// <summary>
        /// Ends an activity (opened with <see cref="Logger.OpenActivity(string)"/>)
        /// with failure and specifies a failure message with an array of parameters.
        /// </summary>
        /// <param name="text">The failure message with parameters, for instance <c>Invalid file at line {Line}</c>.</param>
        /// <param name="args">An array of parameters.</param>
        public void SetFailure(string text, params object[] args)
        {
            this.SetFailure(text, args, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void SetFailure(string text, object[] args, ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo))
                return;

            try
            {
                this.logger.Write(this.Context, this.ResolvedFailureLevel, LogRecordKind.CustomActivityFailure, text, args, null, ref callerInfo );
                this.Close( ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

        }

        /// <summary>
        /// Ends an activity (opened with <see cref="Logger.OpenActivity(string)"/>)
        /// with a given <see cref="Exception"/>.
        /// </summary>
        /// <returns>Always <c>false</c>.</returns>
        /// <param name="exception">The exception.</param>
        public bool SetException(Exception exception)
        {
            return this.SetException(exception, ref nullRecordInfo);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public bool SetException(Exception exception, ref CallerInfo callerInfo)
        {
            if (exception == null)
            {
                this.ExceptionHandler?.OnInvalidUserCode( ref callerInfo, "The exception parameter cannot be null." );
                return false;
            }

            if (!this.RequiresCustomActivity(ref callerInfo))
                return false;

            try
            {
                this.logger.Write( this.Context, this.ResolvedExceptionLevel, LogRecordKind.CustomActivityException, null, exception, ref callerInfo );

                this.Close( ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

            return false;
        }

        private void Close(ref CallerInfo callerInfo)
        {
            if (this.RequiresCustomActivity(ref callerInfo))
            {
                this.Context.Dispose();
            }
            this.logger = null;
            this.Context = null;
        }

        /// <summary>
        /// Disposes the current <see cref="LogActivity"/>.
        /// </summary>
        /// <param name="disposing"><c>true</c> if the method is called because of a call to the <see cref="Dispose()"/> public method, <c>false</c> if it is called by the object finalizer.</param>
        [SuppressMessage("Microsoft.Design", "CA1031")]
        protected virtual void Dispose( bool disposing )
        {
            if (!this.RequiresCustomActivity(ref CallerInfo.Null))
                return;

            try
            {
                this.logger?.Write(null, LogLevel.Warning, LogRecordKind.CustomActivitySuccess, "Activity disposed without setting the outcome.", null, ref nullRecordInfo);
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }
            this.Close(ref CallerInfo.Null);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Resumes the current async activity after it has been suspended by a call to <see cref="Suspend()"/>. There is typically no need
        /// to invoke this method in user code because all async methods that use the <see cref="Logger"/> class are automatically instrumented.
        /// </summary>
        public void Resume(  )
        {
            this.Resume(ref CallerInfo.Null);
        }

       
        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void Resume(ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo, true))
                return;


            if ( !this.Context.IsAsync )
            {
                this.ExceptionHandler?.OnInvalidUserCode( ref callerInfo, "Cannot call Resume outside of an async activity.");
                return;
                
            }

            try
            {
                this.logger.ResumeActivity( this.Context, ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

        }


        /// <summary>
        /// Suspends the current async activity.
        /// The activity must than be resumed by a call of the <see cref="Resume()"/> method.
        /// There is typically no need to invoke this method in user code because all async methods that use the <see cref="Logger"/> class are automatically instrumented.
        /// </summary>
        public void Suspend()
        {
            this.Suspend(ref CallerInfo.Null);
        }

        /// <excludeOverload />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Design", "CA1031")]
        public void Suspend(ref CallerInfo callerInfo)
        {
            if (!this.RequiresCustomActivity(ref callerInfo, true))
                return;


            if ( !this.Context.IsAsync )
            {
                this.ExceptionHandler?.OnInvalidUserCode( ref callerInfo, "Cannot call Suspend outside of an async activity.");
                return;
            }

            try
            {
                this.logger.SuspendActivity( this.Context, ref callerInfo );
            }
            catch ( Exception e )
            {
                this.ExceptionHandler?.OnInternalException( e );
            }

        }

        /// <summary>
        /// Gets the <see cref="ILoggingContext"/> created from the current <see cref="LogActivity"/>.
        /// </summary>
        public ILoggingContext Context { get; private set; }




    }
}



