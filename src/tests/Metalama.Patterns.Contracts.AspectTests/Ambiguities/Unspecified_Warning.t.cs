// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;

namespace Metalama.Patterns.Contracts.AspectTests.Ambiguities.Unspecified_Warning;

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
            if ( value is < 0 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'A' property must be greater than or equal to 0." );
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
            if ( value is < 0 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'B' property must be greater than or equal to 0." );
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
            if ( value is < 5 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'D' property must be greater than or equal to 5." );
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
            if ( value is > 5 )
            {
                throw new ArgumentOutOfRangeException( "value", "The 'E' property must be less than or equal to 5." );
            }

            this._e = value;
        }
    }
}