// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Flashtrace;

/// <summary>
/// Allow to write log messages and trace the execution of activities.
/// </summary>
/// <remarks>
/// <para>
/// Instances of this class are immutable. 
/// </para>
/// <para>
/// You should typically have one instance of this class per type.
/// Use <see cref="LogSource.Get()"/> to get an instance of this class for the current type.
/// </para>
/// </remarks>
[PublicAPI]
public sealed class LogSource
{
    private LogLevelSource? _debugLogLevelSource;
    private LogLevelSource? _traceLogLevelSource;
    private LogLevelSource? _infoLogLevelSource;
    private LogLevelSource? _warningLogLevelSource;
    private LogLevelSource? _errorLogLevelSource;
    private LogLevelSource? _criticalLogLevelSource;
    private LogLevelSource? _noneLogLevelSource;

    internal LogSource( [Required] ILogger logger, LogLevel defaultLevel = LogLevel.Debug, LogLevel failureLevel = LogLevel.Error )
    {
        this.Logger = logger ?? throw new ArgumentNullException( nameof(logger) );
        this.DefaultLevel = defaultLevel;
        this.FailureLevel = failureLevel;
    }

    /// <inheritdoc cref="ILogger" />
    public ILoggingContext CurrentContext => this.Logger.CurrentContext;

    internal ILogger Logger { get; }

    /// <summary>
    /// Gets a new <see cref="LogSource"/> keeping all the configuration of the current instance, but for a different type.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which the <see cref="LogSource"/> should be created.</param>
    /// <returns>A <see cref="LogSource"/> for <paramref name="type"/>.</returns>
    public LogSource ForType( Type type )
    {
        if ( type == null )
        {
            throw new ArgumentNullException( nameof(type) );
        }

        return new LogSource( this.Logger.Factory.GetLogger( type ), this.DefaultLevel, this.FailureLevel );
    }

    /// <summary>
    /// Gets a new <see cref="LogSource"/> keeping all the configuration of the current instance, but for a source name.
    /// </summary>
    /// <param name="sourceName">The name of the source for which the <see cref="LogSource"/> should be created.</param>
    /// <returns>A <see cref="LogSource"/> for <paramref name="sourceName"/>.</returns>
    public LogSource ForSource( string sourceName )
    {
        if ( string.IsNullOrEmpty( sourceName ) )
        {
            throw new ArgumentNullException( nameof(sourceName) );
        }

        return new LogSource( this.Logger.Factory.GetLogger( sourceName ), this.DefaultLevel, this.FailureLevel );
    }

    /// <summary>
    /// Gets a new <see cref="LogSource"/> keeping all the configuration of the current instance, but for the calling type.
    /// </summary>
    /// <returns>A <see cref="LogSource"/> for the calling type.</returns>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public LogSource ForCurrentType()
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );

        return this.ForType( callerInfo.SourceType! );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public LogSource ForCurrentType( ref CallerInfo callerInfo )
    {
        return this.ForType( callerInfo.SourceType! );
    }

    /// <summary>
    /// Gets a <see cref="LogSource"/> for the calling type.
    /// </summary>
    /// <returns>A <see cref="LogSource"/> for the calling type.</returns>
    [MethodImpl( MethodImplOptions.NoInlining )]
    public static LogSource Get()
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );

        return Get( ref callerInfo );
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public static LogSource Get( ref CallerInfo callerInfo ) => LogSourceFactory.Default.GetLogSource( ref callerInfo );

    /// <summary>
    /// Gets a log source associated with a specific source name.
    /// </summary>
    /// <param name="sourceName">Name that the logging backend associates with a log source.</param>
    public static LogSource Get( string sourceName ) => LogSourceFactory.Default.GetLogSource( sourceName );

    /// <summary>
    /// Gets a log source for a specified role and source name.
    /// </summary>
    /// <param name="sourceName">The source name. A dotted name.</param>
    /// <param name="role">A role name. See <see cref="LoggingRoles"/>.</param>
    /// <returns></returns>
    public static LogSource Get( string sourceName, string? role )
        => role == null
            ? LogSourceFactory.Default.GetLogSource( sourceName )
            : LogSourceFactory.ForRole( role ).GetLogSource( sourceName );

    /// <summary>
    /// Gets a log source for a specified role and <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="role">See <see cref="LoggingRoles"/>.</param>
    /// <returns></returns>
    public static LogSource Get( Type type, string? role = null )
        => role == null
            ? LogSourceFactory.Default.GetLogSource( type )
            : LogSourceFactory.ForRole( role ).GetLogSource( type );

    /// <summary>
    /// Determines whether logging is enabled in the current <see cref="LogSource"/> for a given <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="level">A <see cref="LogLevel"/>.</param>
    /// <returns><c>true</c> if logging is enabled for <paramref name="level"/>, otherwise <c>false</c>.</returns>
    public bool IsEnabled( LogLevel level )
    {
        try
        {
            return this.Logger.IsEnabled( level );
        }
        catch ( Exception e )
        {
            this.Logger.OnInternalException( e );

            return false;
        }
    }

    /// <summary>
    /// Exposes methods that allow to write messages and open activities with a specified <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="level">The desired <see cref="LogLevel"/>.</param>
    /// <returns>A fluent API object that allows to write messages with the specified <paramref name="level"/>.</returns>
    /// <remarks>
    ///   <para>This method is properly optimized. Subsequent calls with the same parameter value will return the same instance.</para>
    /// </remarks>
    public LogLevelSource WithLevel( LogLevel level )
    {
        switch ( level )
        {
            case LogLevel.Trace:
                return this.Trace;

            case LogLevel.Debug:
                return this.Debug;

            case LogLevel.Info:
                return this.Info;

            case LogLevel.Warning:
                return this.Warning;

            case LogLevel.Error:
                return this.Error;

            case LogLevel.Critical:
                return this.Critical;

            default:
                return this.None;
        }
    }

    internal LogLevel DefaultLevel
    {
        get;
    }

    internal LogLevel FailureLevel
    {
        get;
    }

    /// <summary>
    /// Returns a new <see cref="LogSource"/> based on the current instance, but overrides the default logging levels.
    /// </summary>
    /// <param name="defaultLevel">The <see cref="LogLevel"/> for the <see cref="Default"/> severity (also used for the start and success messages of activities).</param>
    /// <param name="failureLevel">The <see cref="LogLevel"/> for the <see cref="Failure"/> severity (also used for the failure messages of activities).</param>
    /// <returns>A new <see cref="LogSource"/> with the specified log levels.</returns>
    public LogSource WithLevels( LogLevel defaultLevel, LogLevel failureLevel ) => new( this.Logger, defaultLevel, failureLevel );

    private LogLevelSource GetWriteMessage( ref LogLevelSource? field, LogLevel level ) => field ??= new LogLevelSource( this, level );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the <see cref="LogLevel.None"/> severity. Such messages and activities are never displayed,
    /// but the activities can define properties that can be inherited to children activities and messages.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource None => this._noneLogLevelSource ??= new LogLevelSource( this, LogLevel.None );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the default severity of the current <see cref="LogSource"/>. The default severity is <see cref="LogLevel.Debug"/>,
    /// unless it has been overwritten by the <see cref="WithLevels(LogLevel, LogLevel)"/> method.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Default => this.WithLevel( this.DefaultLevel );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the default failure severity of the current <see cref="LogSource"/>. The default severity is <see cref="LogLevel.Error"/>,
    /// unless it has been overwritten by the <see cref="WithLevels(LogLevel, LogLevel)"/> method.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Failure => this.WithLevel( this.FailureLevel );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the <see cref="LogLevel.Critical"/> severity.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Critical => this.GetWriteMessage( ref this._criticalLogLevelSource, LogLevel.Critical );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the <see cref="LogLevel.Error"/> severity.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Error => this.GetWriteMessage( ref this._errorLogLevelSource, LogLevel.Error );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the <see cref="LogLevel.Warning"/> severity.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Warning => this.GetWriteMessage( ref this._warningLogLevelSource, LogLevel.Warning );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the <see cref="LogLevel.Info"/> severity.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Info => this.GetWriteMessage( ref this._infoLogLevelSource, LogLevel.Info );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the <see cref="LogLevel.Trace"/> severity.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Trace => this.GetWriteMessage( ref this._traceLogLevelSource, LogLevel.Trace );

    /// <summary>
    /// Gets a <see cref="LogLevelSource"/> that can be used to write messages and open activities with the <see cref="LogLevel.Debug"/> severity.
    /// </summary>
    /// <remarks>
    ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
    /// </remarks>
    public LogLevelSource Debug => this.GetWriteMessage( ref this._debugLogLevelSource, LogLevel.Debug );

    /// <summary>
    /// Emits a log record with the source file and line of the caller.
    /// </summary>
    public void WriteExecutionPoint()
    {
        var callerInfo = CallerInfo.GetDynamic( 1 );

        if ( !callerInfo.SourceLineInfo.IsNull )
        {
            this.WriteExecutionPoint( ref callerInfo );
        }
    }

    /// <excludeOverload />
    [EditorBrowsable( EditorBrowsableState.Never )]
    public void WriteExecutionPoint( ref CallerInfo callerInfo )
    {
        if ( this.IsEnabled( LogLevel.Debug ) )
        {
            this.Logger.Write( null, LogLevel.Debug, LogRecordKind.ExecutionPoint, "Executing", null, ref callerInfo );
        }
    }

    // ReSharper disable InvalidXmlDocComment

    /// <summary>
    /// Evaluates whether a transaction needs to be open for a specified <see cref="OpenActivityOptions"/> and updates
    /// its <see cref="OpenActivityOptions.TransactionRequirement"/> property.
    /// This method must be invoked before calling <see cref="LogLevelSource.OpenActivity{T}(in T, in OpenActivityOptions)"/>.
    /// It is not automatically called.
    /// </summary>
    /// <param name="openActivityOptions">The options of the activity to be opened. This method updates the 
    /// <see cref="OpenActivityOptions.TransactionRequirement"/> property of this parameter.</param>
    public void ApplyTransactionRequirements( ref OpenActivityOptions openActivityOptions )
    {
        if ( openActivityOptions.TransactionRequirement.RequiresTransaction )
        {
            // It has already been previously decided to open a transaction.
            return;
        }

        // ReSharper disable once SuspiciousTypeConversion.Global
        if ( this.Logger.GetContextLocalLogger() is ITransactionAwareContextLocalLogger logger )
        {
            logger.ApplyTransactionRequirements( ref openActivityOptions );
        }
    }

    // ReSharper restore InvalidXmlDocComment
}