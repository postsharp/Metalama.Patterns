// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Flashtrace.Contexts
{
    /// <summary>
    /// Possible values of the <see cref="CallerInfo.Attributes"/> property.
    /// </summary>
    [Flags]
    public enum CallerAttributes
    {
        /// <summary>
        /// Default.
        /// </summary>
        None,

        /// <summary>
        /// Determines whether the caller is an <c>async</c> method.
        /// </summary>
        IsAsync = 1
    }
}
