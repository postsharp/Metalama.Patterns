// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections;

namespace Flashtrace
{
    /// <summary>
    /// Kinds of log entry.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
    [Flags]
    public enum LogRecordKind
    {

        /// <summary>
        /// The value was not set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Before method execution.
        /// </summary>
        MethodEntry = 1 << 1,

        /// <summary>
        /// After successful method execution.
        /// </summary>
        MethodSuccess = 1 << 2,

        /// <summary>
        /// After failed method execution (the method threw an exception).
        /// </summary>
        MethodException = 1 << 3,

        /// <summary>
        /// Before async method suspends execution.
        /// </summary>
        AsyncMethodAwait = 1 << 4,

        /// <summary>
        /// After async method resumes execution.
        /// </summary>
        AsyncMethodResume = 1 << 5,

        /// <summary>
        /// When value of a field or property changes.
        /// </summary>
        ValueChanged = 1 << 6,

        /// <summary>
        /// Custom record (emitted by <see cref="Logger.Write(Flashtrace.LogLevel,string)"/>.
        /// </summary>
        CustomRecord = 1 << 7,

        /// <summary>
        /// Before a custom activity (emitted by <see cref="Logger.OpenActivity(string)"/>).
        /// </summary>
        CustomActivityEntry = 1 << 8,

        /// <summary>
        /// When a custom activity fails with an exception (emitted by <see cref="LogActivity.SetException(System.Exception)"/>).
        /// </summary>
        CustomActivityException = 1 << 9,

        /// <summary>
        /// When a custom activity succeeds (emitted by <see cref="LogActivity.SetSuccess(string)"/>).
        /// </summary>
        CustomActivitySuccess = 1 << 10,

        /// <summary>
        /// When a custom activity fails with a custom message (emitted by <see cref="LogActivity.SetFailure(string)"/>.
        /// </summary>
        CustomActivityFailure = 1 << 11,

        /// <summary>
        /// When an iterator yields a result.
        /// </summary>
        IteratorYield = 1 << 12,

        /// <summary>
        /// Before the <see cref="IEnumerator.MoveNext"/> method of an iterator executes.
        /// </summary>
        IteratorMoveNext = 1 << 13,

        /// <summary>
        /// Emitted by <see cref="Logger.WriteExecutionPoint()"/>.
        /// </summary>
        ExecutionPoint = 1 << 14,

        /// <summary>
        /// After a method execution is successful but lasted more time than the threshold.
        /// </summary>
        MethodOvertime = 1 << 15,

 
        /// <summary>
        /// Any exit of a custom activity, where it is not known whether the execution succeeded or failed.
        /// </summary>
        CustomActivityExit = 1 << 16

    }
}