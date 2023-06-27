// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.


namespace PostSharp.Patterns.Caching.Implementation
{
    /// <summary>
    /// Statuses of a <see cref="CachingBackend"/>.
    /// </summary>
    public enum CachingBackendStatus
    {
        /// <summary>
        /// Default.
        /// </summary>
        Default,

        /// <summary>
        /// Being currently disposed.
        /// </summary>
        Disposing,

        /// <summary>
        /// Already disposed.
        /// </summary>
        Disposed,


        /// <summary>
        /// A previous call of Dispose failed.
        /// </summary>
        DisposeFailed
    }
}