// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace;

internal readonly struct LogLevels
{
    private readonly sbyte _defaultLevel;
    private readonly sbyte _failureLevel;

    public LogLevels( LogLevel defaultLevel, LogLevel failureLevel )
    {
        this._defaultLevel = (sbyte) defaultLevel;
        this._failureLevel = (sbyte) failureLevel;
    }

    public LogLevel DefaultLevel => (LogLevel) this._defaultLevel;

    public LogLevel FailureLevel => (LogLevel) this._failureLevel;
}