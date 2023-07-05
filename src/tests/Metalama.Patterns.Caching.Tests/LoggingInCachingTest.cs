// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Xunit;
using System;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Diagnostics;
using Metalama.Patterns.Diagnostics.Backends;
using Metalama.Patterns.Diagnostics.RecordBuilders;
using Metalama.Patterns.Formatters;

namespace Metalama.Patterns.Caching.Tests
{
    public class LoggingInCachingTest : IDisposable
    {
        [Fact]
        public void TestLoggingOutput()
        {
            var selfLoggingBackend = new SelfLoggingBackend();
            LoggingServices.DefaultBackend = selfLoggingBackend;
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( "LoggingInCachingTest" );

            // By default, there is no logging.
            this.Return4();
            this.Return4();

            // But then we enable it:
            Assert.Equal( 0, selfLoggingBackend.RecordCount );
            LoggingServices.DefaultBackend.CurrentContextLocalConfiguration.Verbosity.SetMinimalLevel( LogLevel.Debug, LoggingRoles.Caching );
            this.Return4();
            this.Return4();
            Assert.NotEqual( 0, selfLoggingBackend.RecordCount );
        }

        public void Dispose()
        {
            TestProfileConfigurationFactory.DisposeTest();
            LoggingServices.DefaultBackend = null;
        }

        [Cache]
        private int Return4()
        {
            return 4;
        }

        public class SelfLoggingBackend : TextLoggingBackend
        {
            public int RecordCount { get; set; } = 0;

            protected override LoggingTypeSource CreateTypeSource( LoggingNamespaceSource parent, Type type )
            {
                return new SelfTypeSource( parent, type.FullName, type );
            }

            public override LogRecordBuilder CreateRecordBuilder()
            {
                return new SelfLogRecordBuilder( this );
            }

            protected override TextLoggingBackendOptions GetTextBackendOptions()
            {
                return new TextLoggingBackendOptions();
            }
        }

        public class SelfTypeSource : LoggingTypeSource
        {
#pragma warning disable 618
            public SelfTypeSource( LoggingNamespaceSource parent, string shortName, Type type ) : base( parent, type.FullName, type )
#pragma warning restore 618
            { }

            protected override bool IsBackendEnabled( LogLevel level )
            {
                return true;
            }
        }

        public class SelfLogRecordBuilder : TextLogRecordBuilder
        {
            private readonly SelfLoggingBackend backend;

            public SelfLogRecordBuilder( SelfLoggingBackend backend ) : base( backend )
            {
                this.backend = backend;
            }

            protected override void Write( UnsafeString message )
            {
                this.backend.RecordCount++;
            }
        }
    }
}