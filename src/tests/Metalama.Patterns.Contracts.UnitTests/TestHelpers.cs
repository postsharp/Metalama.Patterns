// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Contracts.UnitTests;

internal static class TestHelpers
{
    public static TException? RecordException<TException>( Action action )
        where TException : Exception
    {
        try
        {
            action();
        }
        catch ( TException exception )
        {
            return exception;
        }

        return null;
    }
}