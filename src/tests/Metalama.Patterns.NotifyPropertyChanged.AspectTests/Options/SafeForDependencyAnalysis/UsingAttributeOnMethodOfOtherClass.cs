// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.Options;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Options.SafeForDependencyAnalysis.UsingAttributeOnMethodOfOtherClass;

public static class OtherClass
{
    [SafeForDependencyAnalysis]
    public static int Foo() => 42;
}

// <target>
[NotifyPropertyChanged]
public class UsingAttributeOnMethodOfOtherClass
{
    public int X => OtherClass.Foo();
}