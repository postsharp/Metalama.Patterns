// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Contexts
{
    /// <summary>
    /// Defines the minimal semantics of a logging context required by the <see cref="Logger"/> class.
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

        /// <summary>
        /// Invokes a delegate for each property defined in the current logging context and optionally in its ancestor contexts.
        /// </summary>
        /// <param name="visitor">The delegate to invoke. The <c>state</c> parameter of the delegate will be assigned to a dummy variable.</param>
        /// <param name="includeAncestors"><c>true</c> if ancestor contexts should be visited, <c>false</c> if only the current context should be visited.</param>
        /// <remarks>
        /// <para>This method will visit all properties regardless of their name. If several properties have the same name, they will be visited several times.</para>
        /// </remarks>
        [Obsolete( "Use LoggingContext.VisitProperties." )]
        void ForEachProperty( LoggingPropertyVisitor<object> visitor, bool includeAncestors = true );

        /// <summary>
        /// Invokes a delegate for each property defined in the current logging context and optionally in its ancestor contexts, and specifies passes a state object
        /// to the delegate.
        /// </summary>
        /// <param name="visitor">The delegate to invoke. The <c>state</c> parameter of the delegate will be assigned to a dummy variable.</param>
        /// <param name="includeAncestors"><c>true</c> if ancestor contexts should be visited, <c>false</c> if only the current context should be visited.</param>
        /// <param name="state">Some state that will be passed to the <paramref name="visitor"/> delegate.</param>
        /// <typeparam name="T">Type of the state passed to the <paramref name="visitor"/> delegate.</typeparam>
        /// <remarks>
        /// <para>This method will visit all properties regardless of their name. If several properties have the same name, they will be visited several times.</para>
        /// </remarks>
        [Obsolete( "Use LoggingContext.VisitProperties." )]
        void ForEachProperty<T>( LoggingPropertyVisitor<T> visitor, ref T state, bool includeAncestors = true );
    }

    /// <summary>
    /// Delegate invoked by the <see cref="ILoggingContext.ForEachProperty{T}(LoggingPropertyVisitor{T}, ref T, bool)"/> method.
    /// </summary>
    /// <param name="property">The visited <see cref="LoggingProperty"/>.</param>
    /// <param name="value">The evaluated value of the property. Will not be <c>null</c> because null properties are ignored. </param>
    /// <param name="state">The value passed to the <see cref="ILoggingContext.ForEachProperty{T}(LoggingPropertyVisitor{T}, ref T, bool)"/> method.</param>
    public delegate void LoggingPropertyVisitor<T>( LoggingProperty property, object value, ref T state );
}