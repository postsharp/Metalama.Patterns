// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics.CodeAnalysis;

namespace Flashtrace
{
    // TODO: LogActivityOptions should be passed by reference everywhere.

    /// <summary>
    /// Options of a new <see cref="LogActivity"/>.
    /// </summary>
    [SuppressMessage( "Microsoft.Performance", "CA1815" )]
    public struct LogActivityOptions
    {
        private byte kind;
        private Flags flags;

        /// <summary>
        /// Gets the default value of the <see cref="LogActivityOptions"/> type.
        /// </summary>
        public static LogActivityOptions Default => default;

        /// <summary>
        /// Gets or sets the kind of <see cref="LogActivity"/>.
        /// </summary>
        public LogActivityKind Kind
        {
            get => (LogActivityKind) this.kind;
            set => this.kind = (byte) value;
        }

        /// <summary>
        /// Determines whether the new <see cref="LogActivityOptions"/> is async.
        /// </summary>
        public bool IsAsync
        {
            get => (this.flags & Flags.IsAsync) != 0;
            internal set
            {
                var otherFlags = this.flags & ~Flags.IsAsync;
                var thisFlag = value ? Flags.IsAsync : Flags.Default;
                this.flags = otherFlags | thisFlag;
            }
        }

        [Flags]
        private enum Flags : byte
        {
            Default,
            IsAsync = 1
        }
    }
}