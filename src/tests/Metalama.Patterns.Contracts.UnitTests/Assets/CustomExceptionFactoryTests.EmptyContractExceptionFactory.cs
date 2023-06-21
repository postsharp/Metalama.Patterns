// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.UnitTests;

public partial class CustomExceptionFactoryTests
{
    private sealed class EmptyContractExceptionFactory : ContractExceptionFactory
    {
        public EmptyContractExceptionFactory( ContractExceptionFactory next ) : base( next ) { }
    }
}