﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.UnitTests;

public class ContractTesting
{
    public bool Method( string parameter )
    {
        return true;
    }

    public void Method2( [StringLength( 5 )] string parameter ) { }
}