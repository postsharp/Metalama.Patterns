// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Testing.UnitTesting;
using Microsoft.CodeAnalysis;
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

    public int A7 => this.A3 + this.A4 + this.A5 + this.A6;

    public int A8 => this.A1.B1.C2;
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

        var result = Implementation.DependencyHelper.GetDependencyGraph( type );

        this.TestOutput.WriteLine( result.ToString() );

        this.TestOutput.WriteLine( "End of test" );
    }

    private void Dump( IEnumerable<KeyValuePair<(IPropertySymbol? Parent, IPropertySymbol Child), HashSet<IPropertySymbol>>> collection )
    {
        foreach ( var kvp in collection )
        {
            this.Dump( kvp );
        }
    }

    private void Dump( KeyValuePair<(IPropertySymbol? Parent, IPropertySymbol Child), HashSet<IPropertySymbol>> kvp )
    {
        this.TestOutput.WriteLine( $"{kvp.Key.Parent?.Name}.{kvp.Key.Child.Name} [ {string.Join( ", ", kvp.Value.Select( p => p.Name).OrderBy( s => s ) )} ]" );
    }
}