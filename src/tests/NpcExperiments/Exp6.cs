// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable

using Metalama.Patterns.NotifyPropertyChanged;

namespace NpcExperiments.Exp6;

[NotifyPropertyChanged]
class A
{
    public int A1 { get; set; }
}

class B : A
{
    public int B1 { get; set; }

    public int B2 => A1;
}