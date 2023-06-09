// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Globalization;

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// Base class for contract exception factories. Implements chain of responsibility between concrete exception factories.
    /// </summary>
    public abstract class ContractExceptionFactory
    {
        private readonly ContractExceptionFactory? _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractExceptionFactory"/> class.
        /// </summary>
        /// <param name="next">Next factory in chain.</param>
        protected ContractExceptionFactory( ContractExceptionFactory? next )
        {
            this._next = next;
        }

        /// <summary>
        /// Creates the exception based on the contents of <see cref="ContractExceptionInfo"/>.
        /// </summary>
        /// <param name="exceptionInfo">Information to be used for the creation of the requested exception.</param>
        /// <returns>The requested exception.</returns>
        public virtual Exception CreateException( ContractExceptionInfo exceptionInfo )
        {
            if ( this._next != null )
            {
                return this._next.CreateException( exceptionInfo );
            }
            else
            {
                const string template = "The [{0}] contract failed with {1}, but the current ContractExceptionFactory is not configured to instantiate this exception type";
                var aspectName = exceptionInfo.AspectType.Name;
                const string attribute = "Attribute";

                if ( aspectName.EndsWith( attribute, StringComparison.Ordinal ) )
                {
                    aspectName = aspectName.Substring( 0, aspectName.Length - attribute.Length );
                }

                throw new InvalidOperationException( string.Format( CultureInfo.InvariantCulture, template, aspectName, exceptionInfo.ExceptionType.Name ) );
            }
        }
    }
}