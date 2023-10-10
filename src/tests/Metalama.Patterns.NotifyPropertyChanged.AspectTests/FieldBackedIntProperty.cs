// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.FieldBackedIntProperty;

[NotifyPropertyChanged]
public class FieldBackedIntProperty
{
    private int _x;

    public int X => this._x;

    public int Y => this.X;
}