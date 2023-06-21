// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Contexts
{
    /// <summary>
    /// Defines the minimal semantics of a logging context required by the <see cref="LogSource"/> class.
    /// </summary>
    [InternalImplement]
    public interface ILoggingContext : IDisposable
    {
        /// <summary>
        /// Determines whether the context is currently disposed (contexts can be recycled, therefore the
        /// disposed state is not the final state).
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets an integer that is incremented every time the current instance is being recycled.
        /// </summary>
        int RecycleId { get; }

        /// <summary>
        /// Determines whether the context represents an <c>async</c> method or a custom activity in an <c>async</c> method.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// Gets a cross-process globally unique identifier for the current context.
        /// </summary>
        string SyntheticId { get; }
    }
}