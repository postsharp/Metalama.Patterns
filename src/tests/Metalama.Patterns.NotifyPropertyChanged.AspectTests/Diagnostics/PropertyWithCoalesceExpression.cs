// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics;

[NotifyPropertyChanged]
public class PropertyWithCoalesceExpression
{
    public int? P1 { get; set; }

    public int P2 => this.P1 ?? -1;
}