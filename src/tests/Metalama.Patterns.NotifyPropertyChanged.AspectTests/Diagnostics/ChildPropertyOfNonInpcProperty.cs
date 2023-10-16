// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics.PropertyOfNonInpcProperty;

public class A
{
    public int A1 { get; set; }
}

public class B
{
    public A B1 { get; set; }
}

// <target>
[NotifyPropertyChanged]
public class C
{
    public B C1 { get; set; }

    public int C2 => this.C1.B1.A1;
}