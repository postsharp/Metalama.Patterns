// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace
{
    internal readonly struct LogLevels
    {
        private readonly sbyte defaultLevel;
        private readonly sbyte failureLevel;

        public LogLevels( LogLevel defaultLevel, LogLevel failureLevel )
        {
            this.defaultLevel = (sbyte) defaultLevel;
            this.failureLevel = (sbyte) failureLevel;
        }

        public LogLevel DefaultLevel => (LogLevel) this.defaultLevel;

        public LogLevel FailureLevel => (LogLevel) this.failureLevel;
    }
}