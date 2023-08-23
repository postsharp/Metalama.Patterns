// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Caching.Backends.Azure;

public sealed class AzureCacheInvalidatorExceptionEventArgs : EventArgs
{
    public Exception Exception { get; }

    public AzureCacheInvalidatorExceptionEventArgs( Exception exception )
    {
        this.Exception = exception;
    }
}