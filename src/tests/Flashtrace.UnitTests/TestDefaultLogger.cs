// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Diagnostics;
using Xunit;

namespace Flashtrace.UnitTests;

public class TestDefaultLogger : IClassFixture<TestDefaultLogger.TraceSourceFixture>
{
    public class TraceSourceFixture
    {
        public readonly TraceSource TraceSource = TraceSourceLogger.GetTraceSource();
        public readonly MyListener Listener = new();

        public LogSource LogSource = LogSource.Get();

        public TraceSourceFixture()
        {
            this.TraceSource.Listeners.Add( this.Listener );
            this.TraceSource.Switch.Level = SourceLevels.All;
        }
    }

    private readonly TraceSourceFixture fixture;

    public TestDefaultLogger( TraceSourceFixture fixture )
    {
        this.fixture = fixture;
        this.fixture.Listener.Messages.Clear();
    }

    [Fact]
    public void Test_TextLogger_Write()
    {
        this.fixture.LogSource.WithLevel( LogLevel.Error ).Write( FormattedMessageBuilder.Formatted( "Error" ) );
        this.fixture.LogSource.WithLevel( LogLevel.Warning ).Write( FormattedMessageBuilder.Formatted( "Warning" ) );
        this.fixture.LogSource.WithLevel( LogLevel.Critical ).Write( FormattedMessageBuilder.Formatted( "Critical" ) );
        this.fixture.LogSource.WithLevel( LogLevel.Debug ).Write( FormattedMessageBuilder.Formatted( "Debug" ) );
        this.fixture.LogSource.WithLevel( LogLevel.Trace ).Write( FormattedMessageBuilder.Formatted( "Trace" ) );
        Assert.Equal( 5, this.fixture.Listener.Messages.Count );
        Assert.Equal( "Error", this.fixture.Listener.Messages[0] );
    }

    [Fact]
    public void Test_TextLogger_ActivitySuccess()
    {
        using ( var activity = this.fixture.LogSource.Default.OpenActivity( FormattedMessageBuilder.Formatted( "Activity" ) ) )
        {
            activity.SetSuccess();
        }

        Assert.Equal( 2, this.fixture.Listener.Messages.Count );
        Assert.Equal( "Activity: Starting", this.fixture.Listener.Messages[0] );
        Assert.Equal( "Activity: Succeeded", this.fixture.Listener.Messages[1] );
    }

    [Fact]
    public void Test_TextLogger_ActivityFailure()
    {
        using ( var activity = this.fixture.LogSource.Default.OpenActivity( FormattedMessageBuilder.Formatted( "Activity" ) ) )
        {
            activity.SetOutcome( LogLevel.Error, FormattedMessageBuilder.Formatted( "Oops: {Hello}", "Hello" ) );
        }

        Assert.Equal( 2, this.fixture.Listener.Messages.Count );
        Assert.Equal( "Activity: Starting", this.fixture.Listener.Messages[0] );
        Assert.Equal( "Activity: Oops: Hello", this.fixture.Listener.Messages[1] );
    }

    [Fact]
    public void Test_SemanticLogger_Write()
    {
        this.fixture.LogSource.WithLevel( LogLevel.Error ).Write( SemanticMessageBuilder.Semantic( "Error", ("a", "b"), ("b", "c") ) );
        this.fixture.LogSource.WithLevel( LogLevel.Warning ).Write( SemanticMessageBuilder.Semantic( "Warning", ("a", "b") ) );
        this.fixture.LogSource.WithLevel( LogLevel.Critical ).Write( SemanticMessageBuilder.Semantic( "Critical", ("a", "b") ) );
        this.fixture.LogSource.WithLevel( LogLevel.Debug ).Write( SemanticMessageBuilder.Semantic( "Debug", ("a", "b") ) );

        // TODO: Review - tell gael about this likely C# compiler bug.

        // NB: Had to add the Semantic( string name, in (string Name, object Value) parameter1 ) overload to get this to
        //     resolve correctly, otherwise the compiler was trying (incorrectly) to use the
        //     Semantic(string messageName, (string Name, object Value)[] parameters) overload (note there's no 'params').
        this.fixture.LogSource.WithLevel( LogLevel.Trace ).Write( SemanticMessageBuilder.Semantic( "Trace", ("a", null) ) );
        Assert.Equal( 5, this.fixture.Listener.Messages.Count );
        Assert.Equal( "Error, a = b, b = c", this.fixture.Listener.Messages[0] );
    }

    [Fact]
    public void Test_SemanticLogger_ActivitySuccess()
    {
        using ( var activity = this.fixture.LogSource.Default.OpenActivity( SemanticMessageBuilder.Semantic( "Activity" ) ) )
        {
            activity.SetSuccess();
        }

        Assert.Equal( 2, this.fixture.Listener.Messages.Count );
        Assert.Equal( "Activity: Starting", this.fixture.Listener.Messages[0] );
        Assert.Equal( "Activity: Succeeded", this.fixture.Listener.Messages[1] );
    }

    [Fact]
    public void Test_SemanticLogger_ActivityFailure()
    {
        using ( var activity = this.fixture.LogSource.Default.OpenActivity( SemanticMessageBuilder.Semantic( "Activity", ("a", "b") ) ) )
        {
            activity.SetOutcome( LogLevel.Error, SemanticMessageBuilder.Semantic( "Failed", ("c", "d") ) );
        }

        Assert.Equal( 2, this.fixture.Listener.Messages.Count );
        Assert.Equal( "Activity, a = b: Starting", this.fixture.Listener.Messages[0] );
        Assert.Equal( "Activity, a = b: Failed, c = d", this.fixture.Listener.Messages[1] );
    }

    public class MyListener : TraceListener
    {
        private string buffer = "";
        public readonly List<string> Messages = new();

        public override void Write( string message )
        {
            this.buffer += message;
        }

        public override void WriteLine( string message )
        {
            this.Messages.Add( this.buffer + message );
            this.buffer = "";
        }

        public override void TraceEvent( TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message )
        {
            this.Messages.Add( message );
        }

        public override void TraceEvent( TraceEventCache eventCache, string source, TraceEventType eventType, int id )
        {
            base.TraceEvent( eventCache, source, eventType, id );
        }

        public override void TraceEvent( TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args )
        {
            this.Messages.Add( string.Format( format, args ) );
        }
    }
}