﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters.UnitTests.Formatters;
using System.Collections.ObjectModel;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Flashtrace.Formatters.UnitTests;

public class FormatterOverwritingTest : FormattersTestsBase
{
    public FormatterOverwritingTest( ITestOutputHelper logger ) : base( logger ) { }

    public record TestCase(
        string Description,
        Type TValue,
        Type OldFormatterTargetType,
        Type OldFormatterType,
        Type NewFormatterTargetType,
        Type NewFormatterType )
    {
        public override string ToString() => this.Description;
    }

    private static IEnumerable<object[]> MakeSerializableBeforeBetweenPermutations( IEnumerable<TestCase> testCases )
    {
        foreach ( var testCase in testCases )
        {
            yield return new object[] { testCase.ToString(), false, false };
            yield return new object[] { testCase.ToString(), false, true };
            yield return new object[] { testCase.ToString(), true, false };
            yield return new object[] { testCase.ToString(), true, true };
        }
    }

    public static IEnumerable<TestCase> EnsureOverwritesTestCases()
        => new TestCase[]
        {
            // object -> object
            new(
                "object -> object",
                typeof(int[]),
                typeof(object),
                typeof(ZeroFormatter<object>),
                typeof(object),
                typeof(OneFormatter<object>) ),

            // object -> generic interface
            new(
                "object -> generic interface",
                typeof(int[]),
                typeof(object),
                typeof(ZeroFormatter<object>),
                typeof(IEnumerable<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic interface -> generic interface (same)
            new(
                "generic interface -> generic interface (same)",
                typeof(int[]),
                typeof(IEnumerable<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(IEnumerable<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic interface -> generic interface (better)
            new(
                "generic interface -> generic interface (better)",
                typeof(int[]),
                typeof(IEnumerable<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(ICollection<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic interface -> generic base type (better)
            new(
                "generic interface -> generic base type (better)",
                typeof(ObservableCollection<int>),
                typeof(IEnumerable<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(Collection<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic base type -> generic base type (same)
            new(
                "generic base type -> generic base type (same)",
                typeof(ObservableCollection<int>),
                typeof(Collection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(Collection<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic base type -> generic base type (better)
            new(
                "generic base type -> generic base type (better)",
                typeof(MyObservableCollection<int>),
                typeof(Collection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(ObservableCollection<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic base type -> generic exact type
            new(
                "generic base type -> generic exact type",
                typeof(ObservableCollection<int>),
                typeof(Collection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(ObservableCollection<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic exact type -> generic exact type
            new(
                "generic exact type -> generic exact type",
                typeof(ObservableCollection<int>),
                typeof(ObservableCollection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(ObservableCollection<>),
                typeof(OneEnumerableFormatter<>) ),

            // interface -> generic exact type
            new(
                "interface -> generic exact type",
                typeof(ObservableCollection<int>),
                typeof(IEnumerable<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(ObservableCollection<>),
                typeof(OneEnumerableFormatter<>) ),

            // interface -> interface (same)
            new(
                "interface -> interface (same)",
                typeof(ObservableCollection<int>),
                typeof(IEnumerable<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(IEnumerable<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // interface -> interface (better)
            new(
                "interface -> interface (better)",
                typeof(ObservableCollection<int>),
                typeof(IEnumerable<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(ICollection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // interface -> base type (better)
            new(
                "interface -> base type (better)",
                typeof(ObservableCollection<int>),
                typeof(IEnumerable<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(Collection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // base type -> base type (same)
            new(
                "base type -> base type (same)",
                typeof(ObservableCollection<int>),
                typeof(Collection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(Collection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // base type -> base type (better)
            new(
                "base type -> base type (better)",
                typeof(MyObservableCollection<int>),
                typeof(Collection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(ObservableCollection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // base type -> exact type
            new(
                "base type -> exact type",
                typeof(ObservableCollection<int>),
                typeof(Collection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(ObservableCollection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // exact type -> exact type
            new(
                "exact type -> exact type",
                typeof(ObservableCollection<int>),
                typeof(ObservableCollection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(ObservableCollection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // Array (base type) -> Array (generic base type)
            new(
                "Array (base type) -> Array (generic base type)",
                typeof(int[]),
                typeof(Array),
                typeof(ZeroFormatter<Array>),
                typeof(Array),
                typeof(OneEnumerableFormatter<>) )
        };

    public static IEnumerable<object[]> EnsureOverwritesSerializablePermutations() => MakeSerializableBeforeBetweenPermutations( EnsureOverwritesTestCases() );

    [MemberData( nameof(EnsureOverwritesSerializablePermutations) )]
    [Theory]
    public void EnsureOverwrites( string testCase, bool logBefore, bool logBetween )
    {
        var testCaseRecord = EnsureOverwritesTestCases().Single( t => t.Description == testCase );

        typeof(FormatterOverwritingTest).GetMethod( nameof(this.EnsureOverwritesCore), BindingFlags.Instance | BindingFlags.NonPublic )
            .MakeGenericMethod( testCaseRecord.TValue )
            .Invoke(
                this,
                new object[]
                {
                    testCaseRecord.OldFormatterTargetType,
                    testCaseRecord.OldFormatterType,
                    testCaseRecord.NewFormatterTargetType,
                    testCaseRecord.NewFormatterType,
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
            result = this.FormatDefault( default(TValue) );

            Assert.Equal( oldExpectedOutput, result );
        }

        this.DefaultRepository.Register( newFormatterTargetType, newFormatterType );

        result = this.FormatDefault( default(TValue) );

        Assert.Equal( newExpectedOutput, result );
    }

    public static IEnumerable<TestCase> EnsureDoesntOverwriteTestCases()
        => new TestCase[]
        {
            // generic interface -> object
            new(
                "generic interface -> object",
                typeof(int[]),
                typeof(IEnumerable<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(object),
                typeof(OneFormatter<object>) ),

            // generic interface -> generic interface (worse)
            new(
                "generic interface -> generic interface (worse)",
                typeof(int[]),
                typeof(ICollection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(IEnumerable<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic base type -> generic interface (worse)
            new(
                "generic base type -> generic interface (worse)",
                typeof(ObservableCollection<int>),
                typeof(Collection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(IEnumerable<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic base type -> generic base type (worse)
            new(
                "generic base type -> generic base type (worse)",
                typeof(MyObservableCollection<int>),
                typeof(ObservableCollection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(Collection<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic exact type -> generic base type
            new(
                "generic exact type -> generic base type",
                typeof(ObservableCollection<int>),
                typeof(ObservableCollection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(Collection<>),
                typeof(OneEnumerableFormatter<>) ),

            // generic exact type -> interface
            new(
                "generic exact type -> interface",
                typeof(ObservableCollection<int>),
                typeof(ObservableCollection<>),
                typeof(ZeroEnumerableFormatter<>),
                typeof(IEnumerable<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // interface -> interface (worse)
            new(
                "interface -> interface (worse)",
                typeof(ObservableCollection<int>),
                typeof(ICollection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(IEnumerable<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // base type -> interface (worse)
            new(
                "base type -> interface (worse)",
                typeof(ObservableCollection<int>),
                typeof(Collection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(IEnumerable<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // base type -> base type (worse)
            new(
                "base type -> base type (worse)",
                typeof(MyObservableCollection<int>),
                typeof(ObservableCollection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(Collection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // exact type -> base type
            new(
                "exact type -> base type",
                typeof(ObservableCollection<int>),
                typeof(ObservableCollection<int>),
                typeof(ZeroEnumerableFormatter<int>),
                typeof(Collection<int>),
                typeof(OneEnumerableFormatter<int>) ),

            // Array (generic base type) -> Array (base type)
            new(
                "Array (generic base type) -> Array (base type)",
                typeof(int[]),
                typeof(Array),
                typeof(ZeroEnumerableFormatter<>),
                typeof(Array),
                typeof(OneFormatter<Array>) )
        };

    public static IEnumerable<object[]> EnsureDoesntOverwritesSerializablePermutations()
        => MakeSerializableBeforeBetweenPermutations( EnsureDoesntOverwriteTestCases() );

    [MemberData( nameof(EnsureDoesntOverwritesSerializablePermutations) )]
    [Theory]
    public void EnsureDoesntOverwrite( string testCase, bool logBefore, bool logBetween )
    {
        var testCaseRecord = EnsureDoesntOverwriteTestCases().Single( t => t.Description == testCase );

        typeof(FormatterOverwritingTest).GetMethod( nameof(this.EnsureDoesntOverwriteCore), BindingFlags.Instance | BindingFlags.NonPublic )
            .MakeGenericMethod( testCaseRecord.TValue )
            .Invoke(
                this,
                new object[]
                {
                    testCaseRecord.OldFormatterTargetType,
                    testCaseRecord.OldFormatterType,
                    testCaseRecord.NewFormatterTargetType,
                    testCaseRecord.NewFormatterType,
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
            result = this.FormatDefault( default(TValue) );

            Assert.Equal( oldExpectedOutput, result );
        }

        this.DefaultRepository.Register( newFormatterTargetType, newFormatterType );

        result = this.FormatDefault( default(TValue) );

        Assert.Equal( oldExpectedOutput, result );
    }
}

internal class ZeroFormatter<T> : Formatter<T>
{
    public ZeroFormatter( IFormatterRepository repository ) : base( repository ) { }

    public override void Write( UnsafeStringBuilder stringBuilder, T value )
    {
        stringBuilder.Append( 0 );
    }
}

internal class ZeroEnumerableFormatter<T> : ZeroFormatter<IEnumerable<T>>
{
    public ZeroEnumerableFormatter( IFormatterRepository repository ) : base( repository ) { }
}

internal class OneFormatter<T> : Formatter<T>
{
    public OneFormatter( IFormatterRepository repository ) : base( repository ) { }

    public override void Write( UnsafeStringBuilder stringBuilder, T value )
    {
        stringBuilder.Append( 1 );
    }
}

internal class OneEnumerableFormatter<T> : OneFormatter<IEnumerable<T>>
{
    public OneEnumerableFormatter( IFormatterRepository repository ) : base( repository ) { }
}

internal class MyObservableCollection<T> : ObservableCollection<T> { }