// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Contexts;


namespace Flashtrace
{
    /// <summary>
    /// Defines methods called in case of exception in logging infrastructure classes. This interface
    /// can be implemented by any class implementing the <see cref="ILogger"/> interface.
    /// When an <see cref="ILogger"/> does not implement this interface, logging exceptions are silently ignored.
    /// </summary>
    public interface ILoggerExceptionHandler
    {
        /// <summary>
        /// Method invoked when the user code calling a logging infrastucture method is invalid, e.g. when the formatting string
        /// is incorrect or does not match the arguments.
        /// </summary>
        /// <param name="callerInfo">Information about the line of code causing the error.</param>
        /// <param name="format">Formatting string of the error message.</param>
        /// <param name="args">Arguments.</param>
        void OnInvalidUserCode( ref CallerInfo callerInfo, string format, params object[] args );

        /// <summary>
        /// Method invoked when an exception is thrown in logging code.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/>.</param>
        void OnInternalException( Exception exception );
    }
}