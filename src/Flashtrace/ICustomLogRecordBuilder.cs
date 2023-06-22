// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;

namespace Flashtrace
{
    /// <summary>
    /// Allows to build a custom log record (typically, but not necessarily, a string). A log record can be composed of one of several
    /// items.
    /// </summary>
    public interface ICustomLogRecordBuilder : IDisposable
    {
        /// <summary>
        /// Begins to build a specified item.
        /// </summary>
        /// <param name="item">The item being built.</param>
        /// <param name="options">Options.</param>
        void BeginWriteItem( CustomLogRecordItem item, in CustomLogRecordTextOptions options );

        /// <summary>
        /// Ends building a specified item.
        /// </summary>
        /// <param name="item"></param>
        void EndWriteItem( CustomLogRecordItem item );

        /// <summary>
        /// Writes a custom parameter.
        /// </summary>
        /// <typeparam name="T">Type of the parameter value.</typeparam>
        /// <param name="index">Index of the parameter (zero-based).</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <param name="options">Options.</param>
        void WriteCustomParameter<T>( int index, in CharSpan parameterName, T value, in CustomLogParameterOptions options );

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="str">A string.</param>
        void WriteCustomString( in CharSpan str );

        /// <summary>
        /// Assigns an <see cref="Exception"/> to the record.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/>.</param>
        void SetException( Exception exception );

        /// <summary>
        /// Sets an execution time to the record.
        /// </summary>
        /// <param name="executionTime">Execution time.</param>
        /// <param name="isOvertime"><c>true</c> whether the activity is overtime, otherwise <c>false</c>.</param>
        void SetExecutionTime( double executionTime, bool isOvertime );

        /// <summary>
        /// Completes the creation of the record. It must be invoked before <see cref="IDisposable.Dispose"/>.
        /// </summary>
        void Complete();
    }
}