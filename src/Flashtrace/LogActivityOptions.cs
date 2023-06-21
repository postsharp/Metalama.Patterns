// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PostSharp.Patterns.Diagnostics
{
    // TODO: LogActivityOptions should be passed by reference everywhere.

    /// <summary>
    /// Options of a new <see cref="LogActivity"/>.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815")]
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
                Flags otherFlags = this.flags & ~Flags.IsAsync;
                Flags thisFlag = value ? Flags.IsAsync : Flags.Default;
                this.flags = otherFlags | thisFlag;  
            }
        }


        [Flags]
        enum Flags : byte
        {
            Default,
            IsAsync = 1
        }
    }


}


