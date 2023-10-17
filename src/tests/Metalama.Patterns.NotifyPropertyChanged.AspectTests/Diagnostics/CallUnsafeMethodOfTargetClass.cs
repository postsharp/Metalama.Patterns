// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics.CallUnsafeMethodOfTargetClass;

// @RemoveOutputCode

[NotifyPropertyChanged]
public class CallUnsafeMethodOfTargetClass
{
    public int X => this.Foo();

    // ReSharper disable once MemberCanBeMadeStatic.Local
    private int Foo() => 42;
}