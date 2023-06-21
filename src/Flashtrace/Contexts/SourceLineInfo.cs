// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Diagnostics;

namespace Flashtrace.Contexts
{
    /// <summary>
    /// Represents a position in a file of source code.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public readonly struct SourceLineInfo
    {
        /// <summary>
        /// Initializes a new <see cref="SourceLineInfo"/>.
        /// </summary>
        /// <param name="file">Path to the source code file.</param>
        /// <param name="line">Line in <paramref name="file"/>.</param>
        /// <param name="column">Column in <paramref name="file"/>.</param>
        [DebuggerStepThrough]
        public SourceLineInfo( string file, int line, int column )
        {
            this.File = file;
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Gets the path to the source code file.
        /// </summary>
        public string File { get; }

        /// <summary>
        /// Gets the line in <see cref="File"/>.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column in <see cref="File"/>.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Determines whether the current <see cref="SourceLineInfo"/> is null.
        /// </summary>
        public bool IsNull => this.File == null;

    }
}
