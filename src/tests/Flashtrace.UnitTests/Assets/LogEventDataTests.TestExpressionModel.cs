// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.UnitTests.Records;

public sealed partial class LogEventDataTests
{
    private sealed class TestExpressionModel
    {
        public readonly object? Data;

        public TestExpressionModel( object? data )
        {
            this.Data = data;
        }
    }
}