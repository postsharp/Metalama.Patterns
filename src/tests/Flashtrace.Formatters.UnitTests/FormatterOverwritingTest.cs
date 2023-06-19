// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters.UnitTests.Formatters;
using System.Collections.ObjectModel;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Flashtrace.Formatters.UnitTests;

public class FormatterOverwritingTest : FormattersTestsBase
{
    public FormatterOverwritingTest( ITestOutputHelper logger ) : base( logger )
    {
    }

    public record TestCase( Type TValue, Type OldFormatterTargetType, Type OldFormatterType, Type NewFormatterTargetType, Type NewFormatterType, string Description )
    {
        public override string ToString()
            => $"{this.Description} <{this.TValue.Name}>( {this.OldFormatterTargetType.Name}, {this.OldFormatterType.Name}, {this.NewFormatterTargetType.Name}, {this.NewFormatterType.Name} )";
    }

    private static TestCase Case<TValue>( string description, Type oldFormatterTargetType, Type oldFormatterType, Type newFormatterTargetType, Type newFormatterType ) =>
        new TestCase( typeof( TValue ), oldFormatterTargetType, oldFormatterType, newFormatterTargetType, newFormatterType, description );

    private static IEnumerable<object[]> MakeBeforeBetweenPermutations( params TestCase[] testCases )
    {
        foreach ( var testCase in testCases )
        {
            yield return new object[] { testCase, false, false };
            yield return new object[] { testCase, false, true };
            yield return new object[] { testCase, true, false };
            yield return new object[] { testCase, true, true };
        }
    }

    public static IEnumerable<object[]> EnsureOverwritesTestCases() => MakeBeforeBetweenPermutations(
        // object -> object
        Case<int[]>( 
            "object -> object",
            typeof( object ), typeof( ZeroFormatter<object> ),
            typeof( object ), typeof( OneFormatter<object> ) ),

        // object -> generic interface
        Case<int[]>( 
            "object -> generic interface",
            typeof( object ), typeof( ZeroFormatter<object> ),
            typeof( IEnumerable<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic interface -> generic interface (same)
        Case<int[]>( 
            "generic interface -> generic interface (same)",
            typeof( IEnumerable<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( IEnumerable<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic interface -> generic interface (better)
        Case<int[]>( 
            "generic interface -> generic interface (better)",
            typeof( IEnumerable<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( ICollection<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic interface -> generic base type (better)
        Case<ObservableCollection<int>>( 
            "generic interface -> generic base type (better)",
            typeof( IEnumerable<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( Collection<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic base type -> generic base type (same)
        Case<ObservableCollection<int>>( 
            "generic base type -> generic base type (same)",
            typeof( Collection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( Collection<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic base type -> generic base type (better)
        Case<MyObservableCollection<int>>( 
            "generic base type -> generic base type (better)",
            typeof( Collection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( ObservableCollection<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic base type -> generic exact type
        Case<ObservableCollection<int>>( 
            "generic base type -> generic exact type",
            typeof( Collection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( ObservableCollection<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic exact type -> generic exact type
        Case<ObservableCollection<int>>( 
            "generic exact type -> generic exact type",
            typeof( ObservableCollection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( ObservableCollection<> ), typeof( OneEnumerableFormatter<> ) ),

        // interface -> generic exact type
        Case<ObservableCollection<int>>( 
            "interface -> generic exact type",
            typeof( IEnumerable<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( ObservableCollection<> ), typeof( OneEnumerableFormatter<> ) ),

        // interface -> interface (same)
        Case<ObservableCollection<int>>( 
            "interface -> interface (same)",
            typeof( IEnumerable<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( IEnumerable<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // interface -> interface (better)
        Case<ObservableCollection<int>>( 
            "interface -> interface (better)",
            typeof( IEnumerable<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( ICollection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // interface -> base type (better)
        Case<ObservableCollection<int>>( 
            "interface -> base type (better)",
            typeof( IEnumerable<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( Collection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // base type -> base type (same)
        Case<ObservableCollection<int>>( 
            "base type -> base type (same)",
            typeof( Collection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( Collection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // base type -> base type (better)
        Case<MyObservableCollection<int>>( 
            "base type -> base type (better)",
            typeof( Collection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( ObservableCollection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // base type -> exact type
        Case<ObservableCollection<int>>( 
            "base type -> exact type",
            typeof( Collection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( ObservableCollection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // exact type -> exact type
        Case<ObservableCollection<int>>( 
            "exact type -> exact type",
            typeof( ObservableCollection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( ObservableCollection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // Array (base type) -> Array (generic base type)
        Case<int[]>( 
            "Array (base type) -> Array (generic base type)",
            typeof( Array ), typeof( ZeroFormatter<Array> ),
            typeof( Array ), typeof( OneEnumerableFormatter<> ) ) );

    [MemberData( nameof( EnsureOverwritesTestCases ) )]
    [Theory]
    public void EnsureOverwrites( TestCase testCase, bool logBefore, bool logBetween )
    {
        typeof( FormatterOverwritingTest ).GetMethod( nameof(this.EnsureOverwritesCore ), BindingFlags.Instance | BindingFlags.NonPublic ).MakeGenericMethod( testCase.TValue )
            .Invoke(
                this,
                new object[]
                {
                    testCase.OldFormatterTargetType,
                    testCase.OldFormatterType,
                    testCase.NewFormatterTargetType,
                    testCase.NewFormatterType,
                    logBefore,
                    logBetween
                } );
    }

    private void EnsureOverwritesCore<TValue>(
        Type oldFormatterTargetType,
        Type oldFormatterType,
        Type newFormatterTargetType,
        Type newFormatterType,
        bool logBefore,
        bool logBetween )
    {
        var oldExpectedOutput = "0";
        var newExpectedOutput = "1";

        string result;

        if ( logBefore )
        {
            this.DefaultRepository.Get<TValue>();
        }

        this.DefaultRepository.Register( oldFormatterTargetType, oldFormatterType );

        if ( logBetween )
        {
            result = this.FormatDefault( default( TValue ) );

            Assert.Equal( oldExpectedOutput, result );
        }

        this.DefaultRepository.Register( newFormatterTargetType, newFormatterType );

        result = this.FormatDefault( default( TValue ) );

        Assert.Equal( newExpectedOutput, result );
    }

    public static IEnumerable<object[]> EnsureDoesntOverwriteTestCases() => MakeBeforeBetweenPermutations(
        // generic interface -> object
        Case<int[]>( "generic interface -> object",
            typeof( IEnumerable<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( object ), typeof( OneFormatter<object> ) ),

        // generic interface -> generic interface (worse)
        Case<int[]>( "generic interface -> generic interface (worse)",
            typeof( ICollection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( IEnumerable<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic base type -> generic interface (worse)
        Case<ObservableCollection<int>>( "generic base type -> generic interface (worse)",
            typeof( Collection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( IEnumerable<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic base type -> generic base type (worse)
        Case<MyObservableCollection<int>>( "generic base type -> generic base type (worse)",
            typeof( ObservableCollection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( Collection<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic exact type -> generic base type
        Case<ObservableCollection<int>>( "generic exact type -> generic base type",
            typeof( ObservableCollection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( Collection<> ), typeof( OneEnumerableFormatter<> ) ),

        // generic exact type -> interface
        Case<ObservableCollection<int>>( "generic exact type -> interface",
            typeof( ObservableCollection<> ), typeof( ZeroEnumerableFormatter<> ),
            typeof( IEnumerable<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // interface -> interface (worse)
        Case<ObservableCollection<int>>( "interface -> interface (worse)",
            typeof( ICollection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( IEnumerable<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // base type -> interface (worse)
        Case<ObservableCollection<int>>( "base type -> interface (worse)",
            typeof( Collection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( IEnumerable<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // base type -> base type (worse)
        Case<MyObservableCollection<int>>( "base type -> base type (worse)",
            typeof( ObservableCollection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( Collection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // exact type -> base type
        Case<ObservableCollection<int>>( "exact type -> base type",
            typeof( ObservableCollection<int> ), typeof( ZeroEnumerableFormatter<int> ),
            typeof( Collection<int> ), typeof( OneEnumerableFormatter<int> ) ),

        // Array (generic base type) -> Array (base type)
        Case<int[]>( "Array (generic base type) -> Array (base type)",
            typeof( Array ), typeof( ZeroEnumerableFormatter<> ),
            typeof( Array ), typeof( OneFormatter<Array> ) ) );

    [MemberData( nameof( EnsureDoesntOverwriteTestCases ) )]
    [Theory]
    public void EnsureDoesntOverwrite( TestCase testCase, bool logBefore, bool logBetween )
    {
        typeof( FormatterOverwritingTest ).GetMethod( nameof( this.EnsureDoesntOverwriteCore ), BindingFlags.Instance | BindingFlags.NonPublic ).MakeGenericMethod( testCase.TValue )
            .Invoke(
                this,
                new object[]
                {
                    testCase.OldFormatterTargetType,
                    testCase.OldFormatterType,
                    testCase.NewFormatterTargetType,
                    testCase.NewFormatterType,
                    logBefore,
                    logBetween
                } );
    }

    private void EnsureDoesntOverwriteCore<TValue>(
        Type oldFormatterTargetType,
        Type oldFormatterType,
        Type newFormatterTargetType,
        Type newFormatterType,
        bool logBefore,
        bool logBetween )
    {
        var oldExpectedOutput = "0";

        string result;

        if ( logBefore )
        {
            this.DefaultRepository.Get<TValue>();
        }

        this.DefaultRepository.Register( oldFormatterTargetType, oldFormatterType );

        if ( logBetween )
        {
            result = this.FormatDefault( default( TValue ) );

            Assert.Equal( oldExpectedOutput, result );
        }

        this.DefaultRepository.Register( newFormatterTargetType, newFormatterType );

        result = this.FormatDefault( default( TValue ) );

        Assert.Equal( oldExpectedOutput, result );
    }
}

internal class ZeroFormatter<T> : Formatter<T>
{
    public ZeroFormatter( IFormatterRepository repository ) : base( repository )
    {
    }

    public override void Write( UnsafeStringBuilder stringBuilder, T value )
    {
        stringBuilder.Append( 0 );
    }
}

internal class ZeroEnumerableFormatter<T> : ZeroFormatter<IEnumerable<T>>
{
    public ZeroEnumerableFormatter( IFormatterRepository repository ) : base( repository )
    {
    }
}

internal class OneFormatter<T> : Formatter<T>
{
    public OneFormatter( IFormatterRepository repository ) : base( repository )
    {
    }

    public override void Write( UnsafeStringBuilder stringBuilder, T value )
    {
        stringBuilder.Append( 1 );
    }
}

internal class OneEnumerableFormatter<T> : OneFormatter<IEnumerable<T>>
{
    public OneEnumerableFormatter( IFormatterRepository repository ) : base( repository )
    {
    }
}

internal class MyObservableCollection<T> : ObservableCollection<T>
{
}