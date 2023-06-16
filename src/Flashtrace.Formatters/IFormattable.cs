// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// Exposes a <see cref="Format"/> method, which allows an object to format itself into an <see cref="UnsafeStringBuilder"/>.
    /// Logging and caching components rely on the <see cref="IFormattable"/> interface.
    /// </summary>
    public interface IFormattable
    {
        /// <summary>
        /// Appends a description of the current object to a given <see cref="UnsafeStringBuilder"/>.
        /// </summary>
        /// <param name="stringBuilder">The <see cref="UnsafeStringBuilder"/> to which the object description should be written.</param>
        /// <param name="role">An object describing in which context the object is being formatted (e.g. caching or logging).</param>
        void Format( UnsafeStringBuilder stringBuilder, FormattingRole role );
    }
}
