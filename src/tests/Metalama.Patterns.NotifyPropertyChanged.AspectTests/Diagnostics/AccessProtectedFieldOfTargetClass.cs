// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics.AccessProtectedFieldOfTargetClass;

// @RemoveOutputCode

[NotifyPropertyChanged]
public class AccessProtectedFieldOfTargetClass
{
    protected int _x;

    public int X => this._x;
}