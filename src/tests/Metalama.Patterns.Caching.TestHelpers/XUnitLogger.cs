// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace;
using Flashtrace.Loggers;
using Flashtrace.Records;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.TestHelpers;

internal class XUnitLogger : IRoleLoggerFactory
{
    private readonly string _role;
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLogger( string role, ITestOutputHelper testOutputHelper )
    {
        this._role = role;
        this._testOutputHelper = testOutputHelper;
    }

    public ILogger GetLogger( Type type ) => new Logger( this._role, type.FullName!, this._testOutputHelper, this );

    public ILogger GetLogger( string sourceName ) => new Logger( this._role, sourceName, this._testOutputHelper, this );

    private sealed class Logger : SimpleSourceLogger
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Logger( string role, string name, ITestOutputHelper testOutputHelper, IRoleLoggerFactory factory ) : base( role, name )
        {
            this._testOutputHelper = testOutputHelper;
            this.Factory = factory;
        }

        public override bool IsEnabled( LogLevel level ) => true;

        public override IRoleLoggerFactory Factory { get; }

        protected override void Write( LogLevel level, LogRecordKind recordKind, string text, Exception exception )
        {
            this._testOutputHelper.WriteLine( $"{level.ToString().ToUpperInvariant()}: {text}" );
        }
    }
}