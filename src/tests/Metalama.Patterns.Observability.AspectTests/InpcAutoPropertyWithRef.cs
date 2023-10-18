// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Observability.AspectTests.Include;

// @Include(Include/SimpleInpcByHand.cs)

namespace Metalama.Patterns.Observability.AspectTests.InpcAutoPropertyWithRef;

// <target>
[Observable]
public class InpcAutoPropertyWithRef
{
    public SimpleInpcByHand X { get; set; }

    public int Y => this.X.A;
}