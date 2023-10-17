// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.CallMethodOfNonExistentClass;

// @RemoveOutputCode

[NotifyPropertyChanged]
public class CallMethodOfNonExistentClass
{
#if METALAMA
    public int X => MissingClass.Foo();
#endif
}