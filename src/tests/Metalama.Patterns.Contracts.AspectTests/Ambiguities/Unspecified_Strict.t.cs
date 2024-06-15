// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using Metalama.Framework.Fabrics;

namespace Metalama.Patterns.Contracts.AspectTests.Ambiguities.Unspecified_Strict;

public class C
{
    private int _a;

    [Positive]
    public int A
    {
        get
        {
            return this._a;
        }
        set
        {
            if ( value is <= 0 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'A' property must be strictly greater than 0." );
            }

            this._a = value;
        }
    }

    private int _b;

    [Positive]
    public int B
    {
        get
        {
            return this._b;
        }
        set
        {
            if ( value is <= 0 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'B' property must be strictly greater than 0." );
            }

            this._b = value;
        }
    }

    private int _d;

    [GreaterThan( 5 )]
    public int D
    {
        get
        {
            return this._d;
        }
        set
        {
            if ( value is <= 5 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'D' property must be strictly greater than 5." );
            }

            this._d = value;
        }
    }

    private int _e;

    [LessThan( 5 )]
    public int E
    {
        get
        {
            return this._e;
        }
        set
        {
            if ( value is >= 5 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'E' property must be strictly less than 5." );
            }

            this._e = value;
        }
    }
}
#pragma warning disable CS0067, CS8618, CS0162, CS0169, CS0414, CA1822, CA1823, IDE0051, IDE0052
internal class Fabric : ProjectFabric
{
    public override void AmendProject( IProjectAmender amender ) => throw new NotSupportedException( "Compile-time-only code cannot be called at run-time." );
}