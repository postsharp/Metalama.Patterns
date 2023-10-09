// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.Diagnostics.CallUnsafeMethodOfExternalClass;

// @RemoveOutputCode

[NotifyPropertyChanged]
public class CallUnsafeMethodOfExternalClass
{
    public int Count => ExternalClass.Foo();
}