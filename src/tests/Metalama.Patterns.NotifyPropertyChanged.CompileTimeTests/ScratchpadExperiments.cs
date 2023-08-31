// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Testing.UnitTesting;
using Xunit;
using Xunit.Abstractions;

namespace Metalama.Patterns.NotifyPropertyChanged.CompileTimeUnitTests;

// Experimental tests used during development. Some may become formal tests later in development.
public class ScratchpadExperiments : UnitTestClass
{
    public ScratchpadExperiments( ITestOutputHelper testOutput ) : base( testOutput, false ) { }

    [Fact]
    public void Test1()
    {
        using var testContext = this.CreateTestContext();

        var code = @"
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

        var result = Implementation.DependencyHelper.GetDependencyGraph<int>( type );

        this.TestOutput.WriteLine( result.ToString() );

        this.TestOutput.WriteLine("");

        foreach ( var node in result.DecendantsDepthFirst() )
        {
            this.TestOutput.WriteLine( node.GetPath() );
        }
    }

    [Fact]
    public void Test2()
    {
        using var testContext = this.CreateTestContext();

        // A2 accesses A1 without `this.` using expression body.
        var code = @"
class A
{
    public int A1 { get; set; }

    public int A2 => A1;

    public int A3 => A1 + A1;
}";

        var compilation = testContext.CreateCompilation( code );

        var type = compilation.Types.OfName( "A" ).Single();

        var result = Implementation.DependencyHelper.GetDependencyGraph<int>( type );

        this.TestOutput.WriteLine( result.ToString() );
    }

    [Fact]
    public void Test3()
    {
        using var testContext = this.CreateTestContext();

        // A2 accesses A1 without `this.` using expression body.
        var code = @"
class A
{
    public int A1 { get; set; }

    public int A2
    {
        get
        {
            return A1 + A1;
        }
    }
}";

        var compilation = testContext.CreateCompilation( code );

        var type = compilation.Types.OfName( "A" ).Single();

        var result = Implementation.DependencyHelper.GetDependencyGraph<int>( type );

        this.TestOutput.WriteLine( result.ToString() );
    }

}