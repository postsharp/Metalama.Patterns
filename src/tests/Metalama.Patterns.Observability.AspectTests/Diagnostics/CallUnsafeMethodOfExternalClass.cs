// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Observability.AspectTests.Diagnostics.CallUnsafeMethodOfExternalClass;

// @RemoveOutputCode

[Observable]
public class CallUnsafeMethodOfExternalClass
{
    public int Count => ExternalClass.Foo( this );
}