// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
namespace PostSharp.Patterns.Diagnostics
{
    internal readonly struct LogLevels
    {
        private readonly sbyte defaultLevel;
        private readonly sbyte failureLevel;

        public LogLevels(LogLevel defaultLevel, LogLevel failureLevel) 
        {
            this.defaultLevel = (sbyte) defaultLevel;
            this.failureLevel = (sbyte) failureLevel;
        }

        public LogLevel DefaultLevel => (LogLevel)this.defaultLevel;

        public LogLevel FailureLevel => (LogLevel)this.failureLevel;


    }
}
