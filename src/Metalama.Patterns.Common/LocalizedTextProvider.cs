// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Globalization;

namespace Metalama.Patterns.Utilities
{
    /// <summary>
    /// Base class for providers of error messages. Implements chain of responsibility between multiple providers.
    /// </summary>
    /// <remarks>
    /// Each chain of providers has its own base class derived from this one, which contains static field <c>Current</c> that points to current
    /// provider.
    /// </remarks>
    public abstract class LocalizedTextProvider
    {
        private readonly LocalizedTextProvider _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizedTextProvider"/> class.
        /// </summary>
        /// <param name="next">Next <see cref="LocalizedTextProvider"/> in the responsibility chain.</param>
        protected LocalizedTextProvider( LocalizedTextProvider next )
        {
            this._next = next;
        }

        /// <summary>
        /// Gets a message declared by the <see cref="LocalizedTextProvider"/> or the rest of responsibility chain if applicable.
        /// </summary>
        /// <param name="messageId">Identifier of the message.</param>
        /// <returns>Message represented by <paramref name="messageId"/>.  Returning <c>null</c> is not allowed.
        /// </returns>
        /// <remarks>
        /// <para>An implementation must call the base implementation if it does not provide the requested message.
        /// The base implementation is responsible to invoke the next provider in the chain of responsibility.
        /// </para>
        /// </remarks>
        public virtual string GetMessage( string messageId )
        {
            if ( this._next == null )
            {
                throw new ArgumentOutOfRangeException( nameof( messageId ), string.Format( CultureInfo.InvariantCulture, "No message defined for id {0}.", messageId ) );
            }
            else
            {
                return this._next.GetMessage( messageId );
            }
        }

        /// <summary>
        /// Formats a string. An implementation would typically invoke <see cref="string.Format(string,object[])"/>.
        /// </summary>
        /// <param name="format">Formatting string. Typically (but not necessarily) the string returned by <see cref="GetMessage"/>.</param>
        /// <param name="arguments">Arguments.</param>
        /// <returns>The formatted string.</returns>
        public virtual string? FormatString( string format, object[] arguments )
        {
            if ( format == null )
            {
                return null;
            }

            return string.Format( CultureInfo.CurrentCulture, format, arguments );
        }
    }
}