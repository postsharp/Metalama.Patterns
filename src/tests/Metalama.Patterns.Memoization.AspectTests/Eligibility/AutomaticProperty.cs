// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Memoize.AspectTests.Eligibility.AutomaticProperty;

public class C
{
    [Memoize]
    private int P { get; }
}