#if !NETCOREAPP1_1 && SYSTEM_TRACE // TraceSource is in .NET Core 1.1 but not in .NET Standard 1.3
#define TRACE_SOURCE_LOGGER
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Custom;

namespace PostSharp.Patterns.Common.Tests.Diagnostics
{
    public class TestDefaultLogger : Xunit.IClassFixture<TestDefaultLogger.TraceSourceFixture>
    {
        public class TraceSourceFixture
        {
#if TRACE_SOURCE_LOGGER
            public readonly TraceSource TraceSource = TraceSourceLogger.GetTraceSource();
            public readonly MyListener Listener = new MyListener();
#endif
#pragma warning disable 618
            public Logger Logger = Logger.GetLogger();
#pragma warning restore 618
            public LogSource LogSource = LogSource.Get();

            public TraceSourceFixture()
            {
#if TRACE_SOURCE_LOGGER
                this.TraceSource.Listeners.Add( this.Listener );
                this.TraceSource.Switch.Level = SourceLevels.All;
#endif
            }
        }

        private readonly TraceSourceFixture fixture;

#if TRACE_SOURCE_LOGGER
        public TestDefaultLogger( TraceSourceFixture fixture )
        {
            this.fixture = fixture;
            this.fixture.Listener.Messages.Clear();
        }
#endif

        [Fact]
        public void Test_Logger_Write()
        {
            this.fixture.Logger.Write(LogLevel.Error, "Error" );
            this.fixture.Logger.Write(LogLevel.Warning, "Warning");
            this.fixture.Logger.Write(LogLevel.Critical, "Critical");
            this.fixture.Logger.Write(LogLevel.Debug, "Debug");
            this.fixture.Logger.Write(LogLevel.Trace, "Trace");
#if TRACE_SOURCE_LOGGER
            Assert.Equal(5, this.fixture.Listener.Messages.Count );
            Assert.Equal("Error", this.fixture.Listener.Messages[0 ] );
#endif
        }

        [Fact]
        public void Test_Logger_ActivitySuccess()
        {
            using ( var activity = this.fixture.Logger.OpenActivity("Activity"))
            {
                activity.SetSuccess();
            }

            #if TRACE_SOURCE_LOGGER
            Assert.Equal(2, this.fixture.Listener.Messages.Count);
            Assert.Equal("Activity: Starting", this.fixture.Listener.Messages[0]);
            Assert.Equal("Activity: Succeeded", this.fixture.Listener.Messages[1]);
            #endif
        }

        [Fact]
        public void Test_Logger_ActivityFailure()
        {
            using (var activity = this.fixture.Logger.OpenActivity("Activity"))
            {
                activity.SetFailure("Oops: {Hello}", "Hello");
            }

            #if TRACE_SOURCE_LOGGER
            Assert.Equal(2, this.fixture.Listener.Messages.Count);
            Assert.Equal("Activity: Starting", this.fixture.Listener.Messages[0]);
            Assert.Equal("Activity: Failed, Oops: Hello", this.fixture.Listener.Messages[1]);
            #endif
        }

#if CS_IN_PARAMETER
        [Fact]
        public void Test_TextLogger_Write()
        {
            this.fixture.LogSource.WithLevel( LogLevel.Error ).Write( FormattedMessageBuilder.Formatted( "Error" ) );
            this.fixture.LogSource.WithLevel( LogLevel.Warning ).Write(FormattedMessageBuilder.Formatted("Warning"));
            this.fixture.LogSource.WithLevel( LogLevel.Critical ).Write(FormattedMessageBuilder.Formatted("Critical"));
            this.fixture.LogSource.WithLevel( LogLevel.Debug ).Write(FormattedMessageBuilder.Formatted("Debug"));
            this.fixture.LogSource.WithLevel( LogLevel.Trace ).Write(FormattedMessageBuilder.Formatted("Trace"));
#if TRACE_SOURCE_LOGGER
            AssertEx.Equal( 5, this.fixture.Listener.Messages.Count );
            AssertEx.Equal( "Error", this.fixture.Listener.Messages[0] );
#endif
        }

        [Fact]
        public void Test_TextLogger_ActivitySuccess()
        {
            using (var activity = this.fixture.LogSource.Default.OpenActivity(FormattedMessageBuilder.Formatted("Activity")))
            {
                activity.SetSuccess();
            }

#if TRACE_SOURCE_LOGGER
            AssertEx.Equal( 2, this.fixture.Listener.Messages.Count );
            AssertEx.Equal( "Activity: Starting", this.fixture.Listener.Messages[0] );
            AssertEx.Equal( "Activity: Succeeded", this.fixture.Listener.Messages[1] );
#endif
        }

        [Fact]
        public void Test_TextLogger_ActivityFailure()
        {
            using (var activity = this.fixture.LogSource.Default.OpenActivity(FormattedMessageBuilder.Formatted("Activity")))
            {
                activity.SetOutcome( LogLevel.Error, FormattedMessageBuilder.Formatted("Oops: {Hello}", "Hello"));
            }

#if TRACE_SOURCE_LOGGER
            AssertEx.Equal( 2, this.fixture.Listener.Messages.Count );
            AssertEx.Equal( "Activity: Starting", this.fixture.Listener.Messages[0] );
            AssertEx.Equal( "Activity: Oops: Hello", this.fixture.Listener.Messages[1] );
#endif
        }
#endif

#if VALUE_TUPLE && CS_VALUE_TUPLE && POSTSHARP_DIAGNOSTICS_SEMANTIC_MESSAGE

        [Fact]
        public void Test_SemanticLogger_Write()
        {
            this.fixture.LogSource.WithLevel( LogLevel.Error ).Write( SemanticMessageBuilder.Semantic("Error", ("a", "b"), ("b", "c") ));
            this.fixture.LogSource.WithLevel( LogLevel.Warning ).Write( SemanticMessageBuilder.Semantic("Warning", ("a", "b") ));
            this.fixture.LogSource.WithLevel( LogLevel.Critical ).Write( SemanticMessageBuilder.Semantic("Critical", ("a", "b") ));
            this.fixture.LogSource.WithLevel( LogLevel.Debug ).Write( SemanticMessageBuilder.Semantic("Debug", ("a", "b") ));
            this.fixture.LogSource.WithLevel( LogLevel.Trace ).Write( SemanticMessageBuilder.Semantic("Trace", ("a", null) ));
#if TRACE_SOURCE_LOGGER
            AssertEx.Equal( 5, this.fixture.Listener.Messages.Count );
            AssertEx.Equal( "Error, a = b, b = c", this.fixture.Listener.Messages[0] );
#endif
        }

        [Fact]
        public void Test_SemanticLogger_ActivitySuccess()
        {
            using (var activity = this.fixture.LogSource.Default.OpenActivity(SemanticMessageBuilder.Semantic("Activity")))
            {
                activity.SetSuccess();
            }

#if TRACE_SOURCE_LOGGER
            AssertEx.Equal( 2, this.fixture.Listener.Messages.Count );
            AssertEx.Equal( "Activity: Starting", this.fixture.Listener.Messages[0] );
            AssertEx.Equal( "Activity: Succeeded", this.fixture.Listener.Messages[1] );
#endif
        }

        [Fact]
        public void Test_SemanticLogger_ActivityFailure()
        {
            using ( var activity = this.fixture.LogSource.Default.OpenActivity( SemanticMessageBuilder.Semantic( "Activity", ("a", "b") ) ) )
            {
                activity.SetOutcome( LogLevel.Error, SemanticMessageBuilder.Semantic( "Failed", ("c", "d") ));
            }

#if TRACE_SOURCE_LOGGER
            AssertEx.Equal( 2, this.fixture.Listener.Messages.Count );
            AssertEx.Equal( "Activity, a = b: Starting", this.fixture.Listener.Messages[0] );
            AssertEx.Equal( "Activity, a = b: Failed, c = d", this.fixture.Listener.Messages[1] );
#endif
        }

#endif


#if TRACE_SOURCE_LOGGER
        public class MyListener : TraceListener
        {
            string buffer = "";
            public readonly List<string> Messages = new List<string>();
        
            public override void Write(string message)
            {
                buffer += message;
            }

            public override void WriteLine(string message)
            {
                this.Messages.Add(buffer + message );
                buffer = "";
            }
            public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
            {
                this.Messages.Add(message );
            }

            public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
            {
                base.TraceEvent(eventCache, source, eventType, id);
            }

            public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
            {
                this.Messages.Add(string.Format(format, args));
            }

        }
#endif
    }
}
