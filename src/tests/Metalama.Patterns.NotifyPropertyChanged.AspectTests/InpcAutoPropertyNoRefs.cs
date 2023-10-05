// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.AspectTests.Include;

// @Include(Include/SimpleInpcByHand.cs)

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.InpcAutoPropertyNoRefs;

// <target>
[NotifyPropertyChanged]
public class InpcAutoPropertyNoRefs
{
    public SimpleInpcByHand X { get; set; }
}