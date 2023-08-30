// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.TestHelpers;

public class XUnitLoggerFactory : ILoggerFactory
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLoggerFactory( ITestOutputHelper testOutputHelper )
    {
        this._testOutputHelper = testOutputHelper;
    }

    public IRoleLoggerFactory ForRole( string role ) => new XUnitLogger( role, this._testOutputHelper );
}