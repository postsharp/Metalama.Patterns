﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Include(Include/SimpleInpcByHand.cs)

using Metalama.Patterns.Observability.AspectTests.Include;

namespace Metalama.Patterns.Observability.AspectTests.InpcAutoPropertyWithInitializerWithRef;

[Observable]
public class InpcAutoPropertyWithInitializerWithRef
{
    public SimpleInpcByHand X { get; set; } = new( 42 );

    public int Y => this.X.A;
}