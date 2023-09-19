// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.ChildPropertyAssets;

[NotifyPropertyChanged]
public partial class A
{
    public A()
    {
        this.A2 = new();
    }

    public int A1 { get; set; }

    public B A2 { get; set; }

    /// <summary>
    /// Leaf ref (A2.B2.C2.D1).
    /// </summary>
    public int A3 => this.A2.B2.C2.D1;
}

[NotifyPropertyChanged]
public partial class B
{
    public B() 
    { 
        this.B2 = new();
    }

    public int B1 { get; set; }

    public C B2 { get; set; }
}

[NotifyPropertyChanged]
public partial class C
{
    public C()
    {
        this.C2 = new();
    }
    public int C1 { get; set; }

    public D C2 { get; set; }
}

[NotifyPropertyChanged]
public partial class D
{
    public int D1 { get; set; }

    public int D2 { get; set; }
}

[NotifyPropertyChanged]
public partial class E
{
    public E()
    {
        this.E2 = new();
    }

    public int E1 { get; set; }

    public B E2 { get; set; }

    /// <summary>
    /// Leaf ref (E2.B2.C2.D1).
    /// </summary>
    public int LR => this.E2.B2.C2.D1;

    /// <summary>
    /// Leaf-parent ref (E2.B2.C1).
    /// </summary>
    public int LP1R => this.E2.B2.C1;

    /// <summary>
    /// Leaf-parent-parent ref (E2.B1).
    /// </summary>
    public int LP2R => this.E2.B1;
}