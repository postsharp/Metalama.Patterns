// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @Include(Include/SimpleInpcByHand.cs)

using Metalama.Patterns.NotifyPropertyChanged.AspectTests.Include;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.InpcAutoPropertyWithInitializerNoRefs;

[NotifyPropertyChanged]
public class InpcAutoPropertyWithInitializerNoRefs
{
    public SimpleInpcByHand X { get; set; } = new( 42 );
}