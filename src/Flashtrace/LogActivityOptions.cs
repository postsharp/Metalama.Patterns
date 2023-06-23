// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

// TODO: [Pre-FT] LogActivityOptions should be passed by reference everywhere.

/// <summary>
/// Options of a new <see cref="LogActivity{TActivityDescription}"/>.
/// </summary>
[PublicAPI]
public struct LogActivityOptions
{
    private byte _kind;
    private Flags _flags;

    /// <summary>
    /// Gets the default value of the <see cref="LogActivityOptions"/> type.
    /// </summary>
    public static LogActivityOptions Default => default;

    /// <summary>
    /// Gets or sets the kind of <see cref="LogActivity{TActivityDescription}"/>.
    /// </summary>
    public LogActivityKind Kind
    {
        get => (LogActivityKind) this._kind;
        set => this._kind = (byte) value;
    }

    // TODO: [FT] LogActivityOptions.IsAsync is never set true. It was previously set by deleted legacy class LogActivity. Action is pending wider use-case assessment (eg, after further porting).
    
    /// <summary>
    /// Gets a value indicating whether the <see cref="LogActivityOptions"/> is async.
    /// </summary>
    public bool IsAsync
    {
        get => (this._flags & Flags.IsAsync) != 0;
        internal set
        {
            var otherFlags = this._flags & ~Flags.IsAsync;
            var thisFlag = value ? Flags.IsAsync : Flags.Default;
            this._flags = otherFlags | thisFlag;
        }
    }

    [Flags]
    private enum Flags : byte
    {
        Default,
        IsAsync = 1
    }
}