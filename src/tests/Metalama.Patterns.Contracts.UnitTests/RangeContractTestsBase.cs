// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Tests.Helpers;
using System.Reflection;

namespace Metalama.Patterns.Contracts.Tests;

public abstract class RangeContractTestsBase
{
    protected const double DoubleTolerance = 2.220446049250313E-16;
    protected const decimal DecimalTolerance = 0.0000000000000000000000000001M;

    protected static void AssertFails( Action<long?> method, long? longValue )
    {
        try
        {
            method( longValue );
            throw new AssertionFailedException(
                string.Format( "{0}( long?:{1} ) did not fail.",
                    method.GetMethodInfo().Name,
                    NullableToString( longValue ) ) );
        }
        catch ( ArgumentOutOfRangeException )
        {
        }
    }

    protected static void AssertFails( Action<ulong?> method, ulong? ulongValue )
    {
        try
        {
            method( ulongValue );
            throw new AssertionFailedException(
                string.Format( "{0}( ulong?:{1} ) did not fail.",
                    method.GetMethodInfo().Name,
                    NullableToString( ulongValue ) ) );
        }
        catch ( ArgumentOutOfRangeException )
        {
        }
    }

    protected static void AssertFails( Action<double?> method, double? doubleValue )
    {
        try
        {
            method( doubleValue );
            throw new AssertionFailedException(
                string.Format( "{0}( double?:{1} ) did not fail.",
                    method.GetMethodInfo().Name,
                    NullableToString( doubleValue ) ) );
        }
        catch ( ArgumentOutOfRangeException )
        {
        }
    }

    protected static void AssertFails( Action<decimal?> method, decimal? decimalValue )
    {
        try
        {
            method( decimalValue );
            throw new AssertionFailedException(
                string.Format( "{0}( decimal:{1} ) did not fail.",
                    method.GetMethodInfo().Name,
                    NullableToString( decimalValue ) ) );
        }
        catch ( ArgumentOutOfRangeException )
        {
        }
    }

    protected static void AssertFails( Action<long?, ulong?, double?, decimal?> method,
        long? longValue,
        ulong? ulongValue,
        double? doubleValue,
        decimal? decimalValue )
    {
        try
        {
            method( longValue, ulongValue, doubleValue, decimalValue );
            throw new AssertionFailedException(
                string.Format( "{0}( long?:{1}, ulong?:{2}, double?:{3}, decimal?:{4} ) did not fail.",
                    method.GetMethodInfo().Name,
                    NullableToString( longValue ),
                    NullableToString( ulongValue ),
                    NullableToString( doubleValue ),
                    NullableToString( decimalValue ) ) );
        }
        catch ( ArgumentOutOfRangeException )
        {
        }
    }

    private static string NullableToString( object nullable ) => nullable == null ? "null" : nullable.ToString();
}