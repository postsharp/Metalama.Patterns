// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Flashtrace
{
    /// <summary>
    /// Specifies the severity of a logged message.
    /// </summary>
#pragma warning disable CA1028 // Enum Storage should be Int32
    public enum LogLevel 
#pragma warning restore CA1028 // Enum Storage should be Int32
    {
       
        /// <summary>
        /// No message should be logged.
        /// </summary>
        /// <remarks>The value is <c>0</c>.</remarks>
        None = 0,

        /// <summary>
        /// The message should be logged at Trace level (when applicable).
        /// </summary>
        Trace = 1,

        /// <summary>
        /// The message should be logged at Debug level (when applicable).
        /// </summary>
        Debug = 2,

        /// <summary>
        /// The message should be logged at Info level (when applicable).
        /// </summary>
        Info = 3,

        /// <summary>
        /// The message should be logged at Warning level (when applicable).
        /// </summary>
        Warning = 4,

        /// <summary>
        /// The message should be logged at Error level (when applicable).
        /// </summary>
        Error = 5,

        /// <summary>
        /// The message should be logged at Critical level (when applicable).
        /// </summary>
        Critical = 6
    }

    [ExplicitCrossPackageInternal]
    internal static class LogLevelExtensions
    {
        public const LogLevel Force = (LogLevel) 0x10;
        
        [ExplicitCrossPackageInternal]
        internal static LogLevel WithForce(this LogLevel level) => level | Force;

        [ExplicitCrossPackageInternal]
        internal static LogLevel WithoutForce( this LogLevel level ) => level & ~Force;

        [ExplicitCrossPackageInternal]
        internal static bool HasForce( this LogLevel level ) => (level & Force) != 0;

        [ExplicitCrossPackageInternal]
        internal static LogLevel CopyForce( this LogLevel source, LogLevel other ) => source.HasForce() ? other.WithForce() : other.WithoutForce();
    }


}