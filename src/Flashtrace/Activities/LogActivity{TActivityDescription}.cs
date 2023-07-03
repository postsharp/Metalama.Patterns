// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Messages;
using Flashtrace.Options;
using Flashtrace.Records;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Flashtrace.Activities;

/// <summary>
/// Represents a logged activity, i.e. something that has a beginning and an end with a specific outcome.
/// </summary>
[PublicAPI]
public readonly struct LogActivity<TActivityDescription> : ILogActivity
    where TActivityDescription : IMessage
{
    private readonly TActivityDescription _description;

    internal IContextLocalLogger Logger { get; }

    private readonly ActivityLogLevels _levels;

    [MethodImpl( MethodImplOptions.AggressiveInlining )] // To avoid copying the struct.
    internal LogActivity( IContextLocalLogger logger, in ActivityLogLevels levels, ILoggingContext? context, in TActivityDescription description )
    {
        this._description = description;
        this.Logger = logger;
        this.Context = context;
        this._levels = levels;
    }

    /// <inheritdoc />
    public ILoggingContext? Context { get; }

    [MethodImpl( MethodImplOptions.NoInlining )]
    private void SetOutcomeImpl<TMessage>(
        LogLevel level,
        in TMessage message,
        Exception? exception,
        in CloseActivityOptions options,
        ref CallerInfo callerInfo,
        int skipFrames = 1 )
        where TMessage : IMessage
    {
        if ( this.Logger == null )
        {
            return;
        }

        level = this._levels.DefaultLevel.CopyForce( level );

        var contextLocalLogger = this.Logger;

        try
        {
            if ( this.Context == null )
            {
                if ( contextLocalLogger.IsEnabled( level ) )
                {
                    if ( callerInfo.IsNull )
                    {
                        callerInfo = CallerInfo.GetDynamic( skipFrames + 1 );
                    }

                    using ( var builder = contextLocalLogger.GetRecordBuilder(
                               new LogRecordOptions(
                                   level,
                                   LogRecordKind.Message,
                                   LogRecordAttributes.WriteActivityDescriptionAndOutcome,
                                   options.Data ),
                               ref callerInfo ) )
                    {
                        // The context was not opened because default verbosity was lower than the minimal one.

                        MessageHelper.Write( this._description, builder, LogRecordItem.ActivityDescription );
                        MessageHelper.Write( message, builder, LogRecordItem.ActivityOutcome );

                        if ( exception != null )
                        {
                            builder.SetException( exception );
                        }

                        builder.Complete();
                    }
                }
            }
            else
            {
                try
                {
                    if ( contextLocalLogger.IsEnabled( level ) )
                    {
                        if ( callerInfo.IsNull )
                        {
                            callerInfo = CallerInfo.GetDynamic( skipFrames + 1 );
                        }

                        using ( var builder = contextLocalLogger.GetRecordBuilder(
                                   new LogRecordOptions(
                                       level,
                                       LogRecordKind.ActivityExit,
                                       LogRecordAttributes.WriteActivityOutcome,
                                       options.Data ),
                                   ref callerInfo,
                                   this.Context ) )
                        {
                            message.Write( builder, LogRecordItem.ActivityOutcome );

                            if ( exception != null )
                            {
                                builder.SetException( exception );
                            }

                            builder.Complete();
                        }
                    }
                }
                finally
                {
                    // We need to try to dispose the context even if there is an exception while writing the final message, so we fix the context stack.
                    this.Context.Dispose();
                }
            }
        }
        catch ( Exception e )
        {
            // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
            // Allow cautious approach, we never want to cause an exception here.
            contextLocalLogger?.OnInternalException( e );
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            if ( this.Context is { IsDisposed: false } )
            {
                this.SetOutcome( LogLevel.Warning, SemanticMessageBuilder.Semantic( "Indeterminate" ) );
            }
        }
        catch ( Exception e )
        {
            this.Logger.OnInternalException( e );
        }
    }

    /// <inheritdoc />
    [MethodImpl( MethodImplOptions.NoInlining )]
    public void SetSuccess( in CloseActivityOptions options = default )
    {
        CallerInfo callerInfo = default;
        this.SetOutcomeImpl( this._levels.DefaultLevel, SemanticMessageBuilder.Semantic( "Succeeded" ), null, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void SetSuccess( in CloseActivityOptions options, ref CallerInfo callerInfo )
    {
        this.SetOutcomeImpl( this._levels.DefaultLevel, SemanticMessageBuilder.Semantic( "Succeeded" ), null, options, ref callerInfo );
    }

    /// <inheritdoc />
    [MethodImpl( MethodImplOptions.NoInlining )]
    public void SetResult<TResult>( TResult result, in CloseActivityOptions options = default )
    {
        CallerInfo callerInfo = default;
        this.SetOutcomeImpl( this._levels.DefaultLevel, SemanticMessageBuilder.Semantic( "Succeeded", "result", result ), null, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void SetResult<TResult>( TResult result, in CloseActivityOptions options, ref CallerInfo callerInfo )
    {
        this.SetOutcomeImpl( this._levels.DefaultLevel, SemanticMessageBuilder.Semantic( "Succeeded", "result", result ), null, options, ref callerInfo );
    }

    /// <inheritdoc />
    public void SetOutcome<TMessage>( LogLevel level, in TMessage message, Exception? exception = null, in CloseActivityOptions options = default )
        where TMessage : IMessage
    {
        CallerInfo callerInfo = default;
        this.SetOutcomeImpl( level, message, exception, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void SetOutcome<TMessage>( LogLevel level, in TMessage message, Exception exception, in CloseActivityOptions options, ref CallerInfo callerInfo )
        where TMessage : IMessage
    {
        this.SetOutcomeImpl( level, message, exception, options, ref callerInfo );
    }

    /// <inheritdoc />
    public void SetException( Exception exception, in CloseActivityOptions options = default )
    {
        CallerInfo callerInfo = default;
        this.SetOutcomeImpl( this._levels.FailureLevel, SemanticMessageBuilder.Semantic( "Failed" ), exception, options, ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void SetException( Exception exception, in CloseActivityOptions options, ref CallerInfo callerInfo )
    {
        this.SetOutcomeImpl( this._levels.FailureLevel, SemanticMessageBuilder.Semantic( "Failed" ), exception, options, ref callerInfo );
    }

    /// <inheritdoc />
    public void Resume()
    {
        this.Resume( ref CallerInfo.Null );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void Resume( ref CallerInfo callerInfo )
    {
        if ( this.Context == null )
        {
            return;
        }

        if ( !this.Context.IsAsync )
        {
            this.Logger.OnInvalidUserCode( ref callerInfo, "Cannot call Resume outside of an async activity." );

            return;
        }

        try
        {
            this.Logger.ResumeActivity( this.Context, ref callerInfo );
        }
        catch ( Exception e )
        {
            this.Logger.OnInternalException( e );
        }
    }

    /// <inheritdoc />
    public void Suspend()
    {
        this.Suspend( ref CallerInfo.Null );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void Suspend( ref CallerInfo callerInfo )
    {
        if ( this.Context == null )
        {
            return;
        }

        if ( !this.Context.IsAsync )
        {
            this.Logger.OnInvalidUserCode( ref callerInfo, "Cannot call Suspend outside of an async activity." );

            return;
        }

        try
        {
            this.Logger.SuspendActivity( this.Context, ref callerInfo );
        }
        catch ( Exception e )
        {
            this.Logger.OnInternalException( e );
        }
    }
}