// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.Tests;

public partial class CustomExceptionFactoryTests
{
    private class ContractTesting
    {
        public bool Method( [Required] string parameter )
        {
            return true;
        }

        public void Method2( [StringLength(5)] string parameter)
        {
        }
    }
}