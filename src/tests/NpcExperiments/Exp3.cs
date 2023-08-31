// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged;

namespace NpcExperiments.Exp3;

#pragma warning disable

[NotifyPropertyChanged]
class A
{
    // Property of a ref type which is not INPC. We can only check for changes to the ref.
    public B A1 { get; set; }

    public int A2 => A1.B1;

    // Value type auto property (with setter)
    public int A3 { get; set; }

    public int A4 => A3;
}

class B
{
    public int B1 { get; set; }
}