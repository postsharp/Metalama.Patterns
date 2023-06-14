// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.Tests;

public partial class CustomExceptionFactoryTests
{
    private sealed class CustomContractExceptionFactory : ContractExceptionFactory
    {
        private readonly Action<Type> _callback;

        public CustomContractExceptionFactory( ContractExceptionFactory next, Action<Type> callback = null!) : base( next )
        {
            this._callback = callback;
        }

        public override Exception CreateException( ContractExceptionInfo exceptionInfo )
        {
            this._callback?.Invoke( exceptionInfo.ExceptionType );
            if ( exceptionInfo.ExceptionType == typeof(ArgumentNullException) )
            {
                return new TestException( "test" );
            }

            return base.CreateException( exceptionInfo );
        }
    }
}