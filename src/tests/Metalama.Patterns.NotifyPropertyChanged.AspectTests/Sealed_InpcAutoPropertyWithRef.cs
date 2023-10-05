// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.AspectTests.Include;

// @Include(Include/SimpleInpcByHand.cs)

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.SealedInpcAutoPropertyWithRef;

// <target>
[NotifyPropertyChanged]
public sealed class SealedInpcAutoPropertyWithRef
{
    public SimpleInpcByHand X { get; set; }

    public int Y => this.X.A;
}