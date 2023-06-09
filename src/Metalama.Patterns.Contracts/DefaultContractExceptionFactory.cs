// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// Default implementation of <see cref="ContractExceptionFactory"/>.
    /// </summary>
    public class DefaultContractExceptionFactory : ContractExceptionFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContractExceptionFactory"/> class.
        /// </summary>
        public DefaultContractExceptionFactory() : this( ContractServices.DefaultExceptionFactory )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContractExceptionFactory"/> class.
        /// </summary>
        public DefaultContractExceptionFactory( ContractExceptionFactory? next ) : base( next )
        {
        }

        /// <inheritdoc cref="ContractExceptionFactory"/>
        public override Exception CreateException( ContractExceptionInfo exceptionInfo )
        {
            var errorMessage = ContractServices.LocalizedTextProvider.GetFormattedMessage( exceptionInfo );

            var parameterName = exceptionInfo.TargetKind.GetParameterName( exceptionInfo.TargetName );

            if ( exceptionInfo.ExceptionType == typeof( ArgumentException ) )
            {
                return new ArgumentException( errorMessage, parameterName );
            }
            else if ( exceptionInfo.ExceptionType == typeof( ArgumentNullException ) )
            {
                return new ArgumentNullException( parameterName, errorMessage );
            }
            else if ( exceptionInfo.ExceptionType == typeof( ArgumentOutOfRangeException ) )
            {
                return new ArgumentOutOfRangeException( parameterName, errorMessage );
            }
            else if ( exceptionInfo.ExceptionType == typeof( PostconditionFailedException ) )
            {
                return new PostconditionFailedException( errorMessage );
            }
            else
            {
                return base.CreateException( exceptionInfo );
            }
        }
    }
}