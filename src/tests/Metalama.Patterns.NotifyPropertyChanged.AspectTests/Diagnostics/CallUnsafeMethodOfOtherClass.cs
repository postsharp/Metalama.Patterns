// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics.CallUnsafeMethodOfOtherClass;

// @RemoveOutputCode

public static class OtherClass
{
    public static int Foo() => 42;
}

[NotifyPropertyChanged]
public class CallUnsafeMethodOfOtherClass
{
    public int X => OtherClass.Foo();
}