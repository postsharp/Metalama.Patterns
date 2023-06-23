// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Messages;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Flashtrace;

/// <summary>
/// A facade to a <see cref="Flashtrace.LogSource"/> constrained to a specific <see cref="LogLevel"/>.
/// Instances of the <see cref="LogLevelSource"/> class are exposed as properties of the <see cref="Flashtrace.LogSource"/>
/// class, e.g. <see cref="Flashtrace.LogSource.Debug"/> or <see cref="Flashtrace.LogSource.Error"/>.
/// </summary>
[PublicAPI]
[SuppressMessage(
    "StyleCop.CSharp.ReadabilityRules",
    "SA1124:Do not use regions",
    Justification = "Class has many method overloads which suit grouping by name." )]
public sealed class LogLevelSource
{
    internal LogLevelSource( LogSource logSource, LogLevel level )
    {
        this.LogSource = logSource;
        this.Level = level;
    }

    #region Write

    /// <summary>
    /// Writes a message.
    /// </summary>
    /// <typeparam name="T">Type of the <paramref name="message"/>.</typeparam>
    /// <param name="message">The message, typically created using the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.</param>
    /// <param name="options">Options.</param>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public void Write<T>( in T message, in WriteMessageOptions options = default )
        where T : IMessage
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );
        this.Write( message, null, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void Write<T>( in T message, in WriteMessageOptions options, ref CallerInfo callerInfo )
        where T : IMessage
    {
        this.Write( message, null, options, ref callerInfo );
    }

    /// <summary>
    /// Writes a message and specifies an <see cref="Exception"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <paramref name="message"/>.</typeparam>
    /// <param name="message">The message, typically created using the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.</param>
    /// <param name="exception">An optional <see cref="Exception"/>.</param>
    /// <param name="options">Options.</param>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public void Write<T>( in T message, Exception? exception, in WriteMessageOptions options = default )
        where T : IMessage
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );
        this.Write( message, exception, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void Write<T>( in T message, Exception? exception, in WriteMessageOptions options, ref CallerInfo callerInfo )
        where T : IMessage
    {
        var (logger, isEnabled) = this.LogSource.Logger.GetContextLocalLogger( this.Level );

        if ( !isEnabled )
        {
            return;
        }

        try
        {
            using ( var recordBuilder = logger.GetRecordBuilder(
                       new LogRecordOptions( this.Level, LogRecordKind.Message, LogRecordAttributes.WriteMessage, options.Data ),
                       ref callerInfo ) )
            {
                MessageHelper.Write( message, recordBuilder, LogRecordItem.Message );

                if ( exception != null )
                {
                    recordBuilder.SetException( exception );
                }

                recordBuilder.Complete();
            }
        }
        catch ( Exception e )
        {
            logger.OnInternalException( e );
        }
    }

    #endregion

    #region OpenActivity

    // ReSharper disable InvalidXmlDocComment

    /// <summary>
    /// Opens an activity. 
    /// </summary>
    /// <param name="description">The activity description, typically created using the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.</param>
    /// <param name="options">Options.</param>
    /// <returns>A <see cref="Flashtrace.LogActivity{TActivityDescription}"/> representing the new activity.</returns>
    /// <remarks>The activity must be closed using
    /// <see cref="Flashtrace.LogActivity{TActivityDescription}.SetSuccess(in SetSuccess)"/>,
    /// <see cref="Flashtrace.LogActivity{TActivityDescription}.SetResult{TResult}(TResult,in CloseActivityOptions)"/>,
    /// <see cref="Flashtrace.LogActivity{TActivityDescription}.SetOutcome{TMessage}(LogLevel,in TMessage,Exception?,in CloseActivityOptions)"/>
    /// or <see cref="Flashtrace.LogActivity{TActivityDescription}.SetException(Exception,in CloseActivityOptions)"/>.
    /// </remarks>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public LogActivity<T> OpenActivity<T>( in T description, in OpenActivityOptions options = default )
        where T : IMessage
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );

        return this.OpenActivity( description, options, ref callerInfo );
    }

    // ReSharper restore InvalidXmlDocComment

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public LogActivity<T> OpenActivity<T>( in T description, in OpenActivityOptions options, ref CallerInfo callerInfo )
        where T : IMessage
    {
        try
        {
            var level = this.Level;
            IContextLocalLogger logger;
            bool isEnabled;

            ILoggingContext? context = null;

            if ( options.TransactionRequirement.RequiresTransaction )
            {
                // We need logger irrespective of whether it is enabled by verbosity. However, we need to respect other
                // ways to disable logging: disabled backend or disabled local configuration.
                level = level.WithForce();

                (logger, isEnabled) = this.LogSource.Logger.GetContextLocalLogger( level );

                if ( !isEnabled )
                {
                    // If even this is disabled, it means the source is hard disabled, then we won't open an activity.
                    return default;
                }

                // Do the actual opening:
                var mutableOptions = options;
                mutableOptions.Level = this.Level;
                context = logger.OpenActivity( mutableOptions, ref callerInfo );

                // The log source has changed during OpenActivity because we're now within a ".Use()" call so we must update ourselves
                // to use the new log source:
                logger = this.LogSource.Logger.GetContextLocalLogger();
                isEnabled = true;
            }
            else if ( options.Data.HasInheritedProperty )
            {
                // We need to open a context anyway, except when it is "hard" disabled.
                (logger, isEnabled) = this.LogSource.Logger.GetContextLocalLogger( this.Level );

                if ( isEnabled )
                {
                    context = logger.OpenActivity( options, ref callerInfo );
                }
                else
                {
                    var mutableOptions = options;
                    mutableOptions.IsHidden = true;

                    context = logger.OpenActivity( mutableOptions, ref callerInfo );
                }
            }
            else
            {
                (logger, isEnabled) = this.LogSource.Logger.GetContextLocalLogger( this.Level );

                if ( isEnabled )
                {
                    context = logger.OpenActivity( options, ref callerInfo );
                }
            }

            if ( isEnabled )
            {
                using ( var recordBuilder = logger.GetRecordBuilder(
                           new LogRecordOptions(
                               level,
                               LogRecordKind.ActivityEntry,
                               LogRecordAttributes.WriteActivityDescription,
                               default ),
                           ref callerInfo,
                           context ) )
                {
                    MessageHelper.Write( description, recordBuilder, LogRecordItem.ActivityDescription );
                    recordBuilder.Complete();
                }
            }

            return new LogActivity<T>( logger, new LogLevels( level, level.CopyForce( this.LogSource.FailureLevel ) ), context, description );
        }
        catch ( Exception e )
        {
            this.LogSource.Logger.OnInternalException( e );

            return default;
        }
    }

    #endregion

    #region LogActivity

    /// <summary>
    /// Executes an <see cref="Action"/> and logs its execution.
    /// </summary>
    /// <typeparam name="TDescription">The type of the description message.</typeparam>
    /// <param name="description">The activity description, typically created using the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.</param>
    /// <param name="action">The action be be executed.</param>
    /// <param name="options">Options.</param>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public void LogActivity<TDescription>( in TDescription description, Action action, in OpenActivityOptions options = default )
        where TDescription : IMessage
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );
        this.LogActivity( description, action, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void LogActivity<TDescription>(
        in TDescription description,
        Action action,
        in OpenActivityOptions options,
        ref CallerInfo callerInfo )
        where TDescription : IMessage
    {
        if ( action == null )
        {
            throw new ArgumentNullException( nameof(action) );
        }

        var activity = this.OpenActivity( description, options, ref callerInfo );

        try
        {
            action();
            activity.SetSuccess( default, ref callerInfo );
        }
        catch ( Exception e )
        {
            activity.SetException( e, default, ref callerInfo );

            throw;
        }
    }

    #endregion

    /// <summary>
    /// Executes a <see cref="Func{Result}"/> and logs its execution.
    /// </summary>
    /// <typeparam name="TDescription">The type of the description message.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the action to execute.</typeparam>
    /// <param name="description">The activity description, typically created using the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.</param>
    /// <param name="action">The action be be executed.</param>
    /// <param name="options">Options.</param>
    /// <returns>The return value of <paramref name="action"/>.</returns>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public TResult LogActivity<TDescription, TResult>( in TDescription description, Func<TResult> action, in OpenActivityOptions options = default )
        where TDescription : IMessage
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );

        return this.LogActivity( description, action, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public TResult LogActivity<TDescription, TResult>(
        in TDescription description,
        Func<TResult> action,
        in OpenActivityOptions options,
        ref CallerInfo callerInfo )
        where TDescription : IMessage
    {
        if ( action == null )
        {
            throw new ArgumentNullException( nameof(action) );
        }

        var activity = this.OpenActivity( description, options, ref callerInfo );

        try
        {
            var returnValue = action();
            activity.SetResult( returnValue, default, ref callerInfo );

            return returnValue;
        }
        catch ( Exception e )
        {
            activity.SetException( e, default, ref callerInfo );

            throw;
        }
    }

    /// <summary>
    /// Executes an asynchronous void delegate and logs its execution.
    /// </summary>
    /// <typeparam name="TDescription">The type of the description message.</typeparam>
    /// <param name="description">The activity description, typically created using the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.</param>
    /// <param name="action">The action be be executed.</param>
    /// <param name="options">Options.</param>
    /// <returns>The <see cref="Task"/>.</returns>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public Task LogActivityAsync<TDescription>( in TDescription description, Func<Task> action, in OpenActivityOptions options = default )
        where TDescription : IMessage
    {
        return this.LogActivityAsyncImpl( description, action, options, CallerInfo.GetDynamic( 1 ) );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public Task LogActivityAsync<TDescription>( in TDescription description, Func<Task> action, in OpenActivityOptions options, ref CallerInfo callerInfo )
        where TDescription : IMessage
    {
        return this.LogActivityAsyncImpl( description, action, options, callerInfo );
    }

    private async Task LogActivityAsyncImpl<TDescription>( TDescription description, Func<Task> action, OpenActivityOptions options, CallerInfo callerInfo )
        where TDescription : IMessage
    {
        if ( action == null )
        {
            throw new ArgumentNullException( nameof(action) );
        }

        var activity = this.OpenActivity( description, options, ref callerInfo );

        try
        {
            var task = action();

            if ( !task.IsCompleted )
            {
                activity.Suspend();

                try
                {
                    await task;
                }
                finally
                {
                    activity.Resume();
                }
            }

            activity.SetSuccess( default, ref callerInfo );
        }
        catch ( Exception e )
        {
            activity.SetException( e, default, ref callerInfo );

            throw;
        }
    }

    /// <summary>
    /// Executes an asynchronous non-void delegate and logs its execution.
    /// </summary>
    /// <typeparam name="TDescription">The type of the description message.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the action to execute.</typeparam>
    /// <param name="description">The activity description, typically created using the <see cref="SemanticMessageBuilder"/> or <see cref="FormattedMessageBuilder"/> class.</param>
    /// <param name="action">The action be be executed.</param>
    /// <param name="options">Options.</param>
    /// <returns>A <see cref="Task{TResult}"/> whose result will be set to the result of <paramref name="action"/>.</returns>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public Task<TResult> LogActivityAsync<TDescription, TResult>(
        in TDescription description,
        Func<Task<TResult>> action,
        in OpenActivityOptions options = default )
        where TDescription : IMessage
    {
        return this.LogActivityAsyncImpl( description, action, options, CallerInfo.GetDynamic( 1 ) );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public Task<TResult> LogActivityAsync<TDescription, TResult>(
        in TDescription description,
        Func<Task<TResult>> action,
        in OpenActivityOptions options,
        ref CallerInfo callerInfo )
        where TDescription : IMessage
    {
        return this.LogActivityAsyncImpl( description, action, options, callerInfo );
    }

    private async Task<TResult> LogActivityAsyncImpl<TDescription, TResult>(
        TDescription description,
        Func<Task<TResult>> action,
        OpenActivityOptions options,
        CallerInfo callerInfo )
        where TDescription : IMessage
    {
        if ( action == null )
        {
            throw new ArgumentNullException( nameof(action) );
        }

        var activity = this.OpenActivity( description, options, ref callerInfo );

        try
        {
            var task = action();

            if ( !task.IsCompleted )
            {
                activity.Suspend();

                try
                {
                    await task;
                }
                finally
                {
                    activity.Resume();
                }
            }

            activity.SetResult( task.Result, default, ref callerInfo );

            return task.Result;
        }
        catch ( Exception e )
        {
            activity.SetException( e, default, ref callerInfo );

            throw;
        }
    }

    internal LogSource LogSource { get; }

    /// <summary>
    /// Gets the <see cref="LogLevel"/> to which the current <see cref="LogLevelSource"/> is constrained.
    /// </summary>
    public LogLevel Level { get; }

    /// <summary>
    /// Gets a value indicating whether logging is enabled for the current <see cref="Level"/>.
    /// </summary>
    public bool IsEnabled => this.LogSource.Logger.IsEnabled( this.Level );

    /// <summary>
    /// Gets the current <see cref="LogLevelSource"/>, or <c>null</c> if logging is not enabled for the current instance. This
    /// property allows to write conditional logging using the <c>?.</c> operator.
    /// </summary>
    public LogLevelSource? EnabledOrNull => this.IsEnabled ? this : null;
}