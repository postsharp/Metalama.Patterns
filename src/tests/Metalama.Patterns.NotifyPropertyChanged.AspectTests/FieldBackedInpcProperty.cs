// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.FieldBackedInpcProperty;

[NotifyPropertyChanged]
public class A
{
    public int A1 { get; set; }
}

// <target>
[NotifyPropertyChanged]
public class FieldBackedInpcProperty
{
    private A _x;

    public A P1 => this._x;

    public int P2 => this._x.A1;
}