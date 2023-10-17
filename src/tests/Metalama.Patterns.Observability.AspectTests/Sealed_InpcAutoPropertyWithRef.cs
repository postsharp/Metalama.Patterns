// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Observability.AspectTests.Include;

// @Include(Include/SimpleInpcByHand.cs)

namespace Metalama.Patterns.Observability.AspectTests.SealedInpcAutoPropertyWithRef;

// <target>
[Observable]
public sealed class SealedInpcAutoPropertyWithRef
{
    public SimpleInpcByHand X { get; set; }

    public int Y => this.X.A;
}