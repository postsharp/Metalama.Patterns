// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged;

namespace NpcExperiments.Exp2;

[NotifyPropertyChanged]
class A
{
    public B A1 { get; set; }

    public int A2 { get; set; }

    public int A3 => this.A1.B1.C1.D1;

    public int A4 => this.A2;

    public int A5 => this.A1.B2;

    public int A6 => this.A2 + this.A1.B2;

    public int A7 => this.A3 + this.A4 + A5 + this.A6;

    public int A8 => this.A1.B1.C2;

    public int? A9 => A1?.B2;

    public int? A10 => this.A1?.B2;

    public int? A11 => (this.A1)?.B2;

    public int A12 => ++A1.B2;

    public int A13
    {
        get
        {
            A1.B2 = 99; // Write-only to A1.B2, should not be treated as a reference.
            return A1.B1.C2;
        }
    }

    // Demonstrate non-leaf access:
    public C A14 => A1.B1;
}

[NotifyPropertyChanged]
class B
{
    public C B1 { get; set; }

    public int B2 { get; set; }
}

[NotifyPropertyChanged]
class C
{
    public D C1 { get; set; }

    public int C2 { get; set; }
}

[NotifyPropertyChanged]
class D
{
    public int D1 { get; set; }
}