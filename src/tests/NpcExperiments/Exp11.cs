// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged;

namespace NpcExperiments.Exp11;

#pragma warning disable

[NotifyPropertyChanged]
class A
{
    public B A1 { get; set; }

    public C A2 => A1.B2;
}


class AA : A
{
    public int AA1 => A1.B2.C2.D1;
}

[NotifyPropertyChanged]
class B
{
    public int B1 { get; set; }

    public C B2 { get; set; }
}

[NotifyPropertyChanged]
class C
{
    public int C1 { get; set; }

    public D C2 { get; set; }
}

[NotifyPropertyChanged]
class D
{
    public int D1 { get; set; }
}