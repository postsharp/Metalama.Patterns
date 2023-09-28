// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.NotifyPropertyChanged.Implementation.DependencyAnalysis;
using Metalama.Testing.UnitTesting;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.NotifyPropertyChanged.CompileTimeTests;

public sealed class DependencyGraphTests : UnitTestClass
{
    public DependencyGraphTests( ITestOutputHelper testOutput ) : base( testOutput, false ) { }

    [Fact]
    public void CommonPatterns()
    {
        // A general test of common class and expression patterns that should work.
        
        using var testContext = this.CreateTestContext();

        const string code = @"
class A
{
    public B A1 { get; set; }

    public int A2 { get; set; }

    public int A3 => this.A1.B1.C1.D1;

    public int A4 => this.A2;
    
    public int A5 => this.A1.B2;

    public int A6 => this.A2 + this.A1.B2;

    public int A7 => this.A3 + this.A4 + A5 + this.A6;

    public int A8 => this.A1.B1.C2;

    public int? A9 => A1?.B2;

    public int? A10 => this.A1?.B2;

    public int? A11 => (this.A1)?.B2;

    public int A12 => ++A1.B2;

    public int A13
    {
        get
        {
            A1.B2 = 99; // Write-only to A1.B2, should not be treated as a reference.
            return A1.B1.C2;
        }
    }
    
    // Demonstrate non-leaf access:
    public C A14 => A1.B1;

    public B A15 { get; set; }
}

class B
{
    public C B1 { get; set; }

    public int B2 { get; set; }
}

class C
{
    public D C1 { get; set; }

    public int C2 { get; set; }
}

class D
{
    public int D1 { get; set; }
}";

        var compilation = testContext.CreateCompilation( code );

        var type = compilation.Types.OfName( "A" ).Single();

        var result = DependencyGraph.GetDependencyGraph( type, ( _, _ ) => throw new InvalidOperationException( "Unexpected" ) );

        const string expected = @"<root>
  A1
    B1 [ A14 ]
      C1
        D1 [ A3, A7 ]
      C2 [ A13, A8 ]
    B2 [ A10, A11, A12, A5, A6, A7, A9 ]
  A10
  A11
  A12
  A13
  A14
  A2 [ A4, A6, A7 ]
  A3 [ A7 ]
  A4 [ A7 ]
  A5 [ A7 ]
  A6 [ A7 ]
  A7
  A8
  A9
";

        result.ToString().Should().Be( expected );

        // this.TestOutput.WriteLine( result.ToString() );
    }
}