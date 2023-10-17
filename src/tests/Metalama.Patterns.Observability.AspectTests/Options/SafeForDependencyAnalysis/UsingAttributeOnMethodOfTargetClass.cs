// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Observability.Options;

namespace Metalama.Patterns.Observability.AspectTests.Options.SafeForDependencyAnalysis.UsingAttributeOnMethodOfTargetClass;

[Observable]
public class UsingAttributeOnMethodOfTargetClass
{
    public int X => this.Foo();

    [SafeForDependencyAnalysis]
    private int Foo() => 42;
}