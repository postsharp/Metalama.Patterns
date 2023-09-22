// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.HasInpcPropertyWithInitializer;

[NotifyPropertyChanged]
public partial class Simple
{
    public int A { get; set; }
}

[NotifyPropertyChanged]
public partial class HasInpcPropertyWithInitializer
{
    public Simple A { get; set; } = new();
}