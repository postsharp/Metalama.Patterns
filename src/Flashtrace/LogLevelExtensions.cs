// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace Flashtrace;

[PublicAPI]
public static class LogLevelExtensions
{
    private const LogLevel _force = (LogLevel) 0x10;

    public static LogLevel WithForce( this LogLevel level ) => level | _force;

    public static LogLevel WithoutForce( this LogLevel level ) => level & ~_force;

    public static bool HasForce( this LogLevel level ) => (level & _force) != 0;

    public static LogLevel CopyForce( this LogLevel source, LogLevel other ) => source.HasForce() ? other.WithForce() : other.WithoutForce();
}