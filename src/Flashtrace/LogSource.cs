// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;
using Flashtrace.Custom;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Flashtrace
{
    /// <summary>
    /// Allow to write log messages and trace the execution of activities. This class is optimized for use with C# 7.2 or later.
    /// For previous compiler versions, consider using the legacy <see cref="Logger"/> class.
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
    public sealed class LogSource
    {
        private LogLevelSource debugLogLevelSource,
                               traceLogLevelSource,
                               infoLogLevelSource,
                               warningLogLevelSource,
                               errorLogLevelSource,
                               criticalLogLevelSource,
                               noneLogLevelSource;

        internal LogSource( [Required] ILogger3 logger, LogLevel defaultLevel = LogLevel.Debug, LogLevel failureLevel = LogLevel.Error )
        {
            this.Logger = logger;
            this.DefaultLevel = defaultLevel;
            this.FailureLevel = failureLevel;
        }

        /// <inheritdoc />
        public ILoggingContext CurrentContext => this.Logger.CurrentContext;

        internal ILogger3 Logger { get; }

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

            return new LogSource( ((ILoggerFactory3) this.Logger.Factory).GetLogger( sourceName ), this.DefaultLevel, this.FailureLevel );
        }

        /// <summary>
        /// Gets a new <see cref="LogSource"/> keeping all the configuration of the current instance, but for the calling type.
        /// </summary>
        /// <returns>A <see cref="LogSource"/> for the calling type.</returns>
        [MethodImpl( MethodImplOptions.NoInlining )]
        public LogSource ForCurrentType()
        {
            var callerInfo = CallerInfo.GetDynamic( 1 );

            return this.ForType( callerInfo.SourceType ?? typeof(object) );
        }

        /// <excludeOverload />
        [EditorBrowsable( EditorBrowsableState.Never )]
        public LogSource ForCurrentType( ref CallerInfo callerInfo )
        {
            return this.ForType( callerInfo.SourceType );
        }

        /// <summary>
        /// Gets a <see cref="LogSource"/> for the calling type.
        /// </summary>
        /// <returns>A <see cref="LogSource"/> for the calling type.</returns>
        [SuppressMessage( "Microsoft.Performance", "CA1801" )]
        [MethodImpl( MethodImplOptions.NoInlining )]
        public static LogSource Get()
        {
            var callerInfo = CallerInfo.GetDynamic( 1 );

            return Get( ref callerInfo );
        }

        /// <excludeOverload />
        [EditorBrowsable( EditorBrowsableState.Never )]
        public static LogSource Get( ref CallerInfo callerInfo ) => LogSourceFactory.Default3.GetLogSource( ref callerInfo );

        /// <summary>
        /// Gets a log source associated with a specific source name.
        /// </summary>
        /// <param name="sourceName">Name that the logging backend associates with a log source.</param>
        public static LogSource Get( string sourceName ) => LogSourceFactory.Default3.GetLogSource( sourceName );

        /// <summary>
        /// Gets a log source for a specified role and source name.
        /// </summary>
        /// <param name="role">A role name. See <see cref="LoggingRoles"/>.</param>
        /// <param name="sourceName">The source name. A dotted name.</param>
        /// <returns></returns>
        public static LogSource Get( string sourceName, string role = null ) => LogSourceFactory.ForRole3( role ?? "Custom" ).GetLogSource( sourceName );

        /// <summary>
        /// Gets a log source for a specified role and <see cref="Type"/>.
        /// </summary>
        /// <param name="role">See <see cref="LoggingRoles"/>.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static LogSource Get( Type type, string role = null ) => LogSourceFactory.ForRole3( role ?? "Custom" ).GetLogSource( type );

        /// <summary>
        /// Determines whether logging is enabled in the current <see cref="LogSource"/> for a given <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="level">A <see cref="LogLevel"/>.</param>
        /// <returns><c>true</c> if logging is enabled for <paramref name="level"/>, otherwise <c>false</c>.</returns>
        [SuppressMessage( "Microsoft.Performance", "CA1801" )]
        [SuppressMessage( "Microsoft.Design", "CA1031" )]
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

        private LogLevelSource GetWriteMessage( ref LogLevelSource field, LogLevel level ) => field ?? (field = new LogLevelSource( this, level ));

        /// <summary>
        /// Exposes methods that allow to open activities with the <see cref="LogLevel.None"/> severity. Such activities are never displayed,
        /// but they can define properties that can be inherited to children activities and messages.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource None => (this.noneLogLevelSource ?? (this.noneLogLevelSource = new LogLevelSource( this, LogLevel.None )));

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the default severity of the current <see cref="LogSource"/>. The default severity is <see cref="LogLevel.Debug"/>,
        /// unless it has been overwritten by the <see cref="WithLevels(LogLevel, LogLevel)"/> method.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Default => this.WithLevel( this.DefaultLevel );

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the default failure severity of the current <see cref="LogSource"/>. The default severity is <see cref="LogLevel.Error"/>,
        /// unless it has been overwritten by the <see cref="WithLevels(LogLevel, LogLevel)"/> method.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Failure => this.WithLevel( this.FailureLevel );

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the <see cref="LogLevel.Critical"/> severity.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Critical => this.GetWriteMessage( ref this.criticalLogLevelSource, LogLevel.Critical );

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the <see cref="LogLevel.Error"/> severity.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Error => this.GetWriteMessage( ref this.errorLogLevelSource, LogLevel.Error );

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the <see cref="LogLevel.Warning"/> severity.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Warning => this.GetWriteMessage( ref this.warningLogLevelSource, LogLevel.Warning );

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the <see cref="LogLevel.Info"/> severity.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Info => this.GetWriteMessage( ref this.infoLogLevelSource, LogLevel.Info );

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the <see cref="LogLevel.Trace"/> severity.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Trace => this.GetWriteMessage( ref this.traceLogLevelSource, LogLevel.Trace );

        /// <summary>
        /// Exposes methods that allow to write messages and open activities with the <see cref="LogLevel.Debug"/> severity.
        /// </summary>
        /// <remarks>
        ///   <para>This property is properly optimized. Subsequent calls will return the same instance.</para>
        /// </remarks>
        public LogLevelSource Debug => this.GetWriteMessage( ref this.debugLogLevelSource, LogLevel.Debug );

        /// <summary>
        /// Emits a log record with the source file and line of the caller.
        /// </summary>
        [SuppressMessage( "Microsoft.Performance", "CA1822:MarkMembersAsStatic" )]
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
        [SuppressMessage( "Microsoft.Design", "CA1031" )]
        public void WriteExecutionPoint( ref CallerInfo callerInfo )
        {
            if ( this.IsEnabled( LogLevel.Debug ) )
            {
                this.Logger.Write( null, LogLevel.Debug, LogRecordKind.ExecutionPoint, "Executing", null, ref callerInfo );
            }
        }

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

            if ( this.Logger.GetContextLocalLogger() is ITransactionAwareContextLocalLogger logger )
            {
                logger.ApplyTransactionRequirements( ref openActivityOptions );
            }
        }
    }
}