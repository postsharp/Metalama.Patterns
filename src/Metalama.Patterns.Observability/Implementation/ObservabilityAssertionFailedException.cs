// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.Observability.Implementation;

[CompileTime]
public class ObservabilityAssertionFailedException : Exception
{
    public ObservabilityAssertionFailedException( string message ) : base( message ) { }

    public ObservabilityAssertionFailedException() { }
}