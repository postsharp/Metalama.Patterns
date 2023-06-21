// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics.CodeAnalysis;
using PostSharp.Patterns.Diagnostics.Contexts;
using System;

namespace PostSharp.Patterns.Diagnostics.Custom
{
    /// <summary>
    /// Extends <see cref="ILogger"/>.
    /// </summary>
    public interface ILogger2 : ILogger, ILoggerExceptionHandler
    {
        /// <summary>
        /// Gets the <see cref="ILoggerFactory"/>, which allows to create new instances of the logger. This is used for instance by <see cref="LogSource.ForType(System.Type)"/>.
        /// </summary>
        [Obsolete("Use ILogger3.Factory")]
        ILoggerFactory2 Factory { get; }

        /// <summary>
        /// Gets the logger for the current context.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Expensive to evaluate.")]
        IContextLocalLogger GetContextLocalLogger();
        
        /// <summary>
        /// Gets the current <see cref="ILoggingContext"/>.
        /// </summary>
        ILoggingContext CurrentContext { get; }
        
    }

    /// <summary>
    /// Extends <see cref="ILogger2"/>.
    /// </summary>
    public interface ILogger3 : ILogger2
    {
        /// <summary>
        /// Gets the logger for the current context and returns a flag determining if the logger is enabled for a given level.
        /// </summary>
        /// <returns></returns>
        new ILoggerFactory3 Factory { get; }

        /// <summary>
        /// Gets the <see cref="IContextLocalLogger"/> plus a flag indicating whether the source is enabled for a given <see cref="LogLevel"/>.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <returns>The <see cref="IContextLocalLogger"/> for the current execution context and a flag indicating whether logging
        /// is enabled for the given <paramref name="level"/>.</returns>
        (IContextLocalLogger logger, bool isEnabled) GetContextLocalLogger( LogLevel level );
    }
     
}


