// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.BasicReferenceAssets;

[NotifyPropertyChanged]
public partial class A
{
    public int A1 { get; set; }

    public B? A2 { get; set; }
}

[NotifyPropertyChanged]
public partial class B
{
    public int B1 { get; set; }

    public C? B2 { get; set; }
}

[NotifyPropertyChanged]
public partial class C
{
    public int C1 { get; set; }

    public D? C2 { get; set; }
}

[NotifyPropertyChanged]
public partial class D
{
    public int D1 { get; set; }

    public int D2 { get; set; }
}