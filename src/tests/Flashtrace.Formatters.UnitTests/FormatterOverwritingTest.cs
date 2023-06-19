// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if APP_DOMAIN

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Text;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Formatters;


namespace PostSharp.Patterns.Common.Tests.Formatters
{
    public class FormatterOverwritingTest
    {
        private static bool currentLogBefore;
        private static bool currentLogBetween;

        private static void ExecuteOverwritingTest( Action<FormatterOverwritingTest> testMethod )
        {
            ExecuteOverwritingTestCore( testMethod, false, false );
            ExecuteOverwritingTestCore( testMethod, false, true );
            ExecuteOverwritingTestCore( testMethod, true, false );
            ExecuteOverwritingTestCore( testMethod, true, true );
        }

        private static void ExecuteOverwritingTestCore( Action<FormatterOverwritingTest> testMethod, bool logBefore, bool logBetween )
        {
            AppDomain domain = AppDomain.CreateDomain(
                "Test",
                new Evidence( AppDomain.CurrentDomain.Evidence ),
                new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                } );

            domain.DoCallBack( new OverwritingClosure( testMethod, logBefore, logBetween ).Execute );
        }

        [Serializable]
        private class OverwritingClosure
        {
            private readonly Action<FormatterOverwritingTest> callback;
            private readonly bool logBefore;
            private readonly bool logBetween;

            public OverwritingClosure( Action<FormatterOverwritingTest> callback, bool logBefore, bool logBetween )
            {
                this.callback = callback;
                this.logBefore = logBefore;
                this.logBetween = logBetween;
            }

            public void Execute()
            {
                FormatterRepository<TestRole>.Reset();

                currentLogBefore = this.logBefore;
                currentLogBetween = this.logBetween;

                FormatterOverwritingTest testClass = new FormatterOverwritingTest();
                this.callback( testClass );
            }
        }

        [Fact]
        public void EnsureOverwritesTest()
        {
            // object -> object
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<int[]>(
                    typeof(object), typeof(ZeroFormatter<object>),
                    typeof(object), typeof(OneFormatter<object>) ) );

            // object -> generic interface
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<int[]>(
                    typeof(object), typeof(ZeroFormatter<object>),
                    typeof(IEnumerable<>), typeof(OneEnumerableFormatter<>) ) );

            // generic interface -> generic interface (same)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<int[]>(
                    typeof(IEnumerable<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(IEnumerable<>), typeof(OneEnumerableFormatter<>) ) );

            // generic interface -> generic interface (better)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<int[]>(
                    typeof(IEnumerable<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(ICollection<>), typeof(OneEnumerableFormatter<>) ) );

            // generic interface -> generic base type (better)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(IEnumerable<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(Collection<>), typeof(OneEnumerableFormatter<>) ) );

            // generic base type -> generic base type (same)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(Collection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(Collection<>), typeof(OneEnumerableFormatter<>) ) );

            // generic base type -> generic base type (better)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<MyObservableCollection<int>>(
                    typeof(Collection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(ObservableCollection<>), typeof(OneEnumerableFormatter<>) ) );

            // generic base type -> generic exact type
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(Collection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(ObservableCollection<>), typeof(OneEnumerableFormatter<>) ) );

            // generic exact type -> generic exact type
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(ObservableCollection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(ObservableCollection<>), typeof(OneEnumerableFormatter<>) ) );

            // interface -> generic exact type
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(IEnumerable<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(ObservableCollection<>), typeof(OneEnumerableFormatter<>) ) );

            // interface -> interface (same)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(IEnumerable<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(IEnumerable<int>), typeof(OneEnumerableFormatter<int>) ) );

            // interface -> interface (better)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(IEnumerable<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(ICollection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // interface -> base type (better)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(IEnumerable<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(Collection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // base type -> base type (same)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(Collection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(Collection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // base type -> base type (better)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<MyObservableCollection<int>>(
                    typeof(Collection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(ObservableCollection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // base type -> exact type
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(Collection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(ObservableCollection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // exact type -> exact type
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<ObservableCollection<int>>(
                    typeof(ObservableCollection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(ObservableCollection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // Array (base type) -> Array (generic base type)
            ExecuteOverwritingTest(
                fot => fot.EnsureOverwrites<int[]>(
                    typeof(Array), typeof(ZeroFormatter<Array>),
                    typeof(Array), typeof(OneEnumerableFormatter<>) ) );
        }

        private static string Format<T>(T value)
        {
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder(1024);
            FormatterRepository<TestRole>.Get<T>().Write(stringBuilder, value);
            return stringBuilder.ToString();
        }

        private void EnsureOverwrites<TValue>(
            Type oldFormatterTargetType, Type oldFormatterType,
            Type newFormatterTargetType, Type newFormatterType )
        {
            string oldExpectedOutput = "0";
            string newExpectedOutput = "1";

            string result;

            if ( currentLogBefore )
            {
                FormatterRepository<TestRole>.Get<TValue>();
            }

            FormatterRepository<TestRole>.Register( oldFormatterTargetType, oldFormatterType );

            if ( currentLogBetween )
            {
                result = Format( default(TValue) );

                Assert.Equal( oldExpectedOutput, result );
            }

            FormatterRepository<TestRole>.Register( newFormatterTargetType, newFormatterType );

            result = Format( default(TValue) );

            Assert.Equal( newExpectedOutput, result );
        }

        [Fact]
        public void EnsureDoesntOverwriteTest()
        {
            // generic interface -> object
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<int[]>(
                    typeof(IEnumerable<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(object), typeof(OneFormatter<object>) ) );

            // generic interface -> generic interface (worse)
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<int[]>(
                    typeof(ICollection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(IEnumerable<>), typeof(OneEnumerableFormatter<>) ) );

            // generic base type -> generic interface (worse)
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<ObservableCollection<int>>(
                    typeof(Collection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(IEnumerable<>), typeof(OneEnumerableFormatter<>) ) );

            // generic base type -> generic base type (worse)
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<MyObservableCollection<int>>(
                    typeof(ObservableCollection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(Collection<>), typeof(OneEnumerableFormatter<>) ) );

            // generic exact type -> generic base type
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<ObservableCollection<int>>(
                    typeof(ObservableCollection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(Collection<>), typeof(OneEnumerableFormatter<>) ) );

            // generic exact type -> interface
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<ObservableCollection<int>>(
                    typeof(ObservableCollection<>), typeof(ZeroEnumerableFormatter<>),
                    typeof(IEnumerable<int>), typeof(OneEnumerableFormatter<int>) ) );

            // interface -> interface (worse)
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<ObservableCollection<int>>(
                    typeof(ICollection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(IEnumerable<int>), typeof(OneEnumerableFormatter<int>) ) );

            // base type -> interface (worse)
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<ObservableCollection<int>>(
                    typeof(Collection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(IEnumerable<int>), typeof(OneEnumerableFormatter<int>) ) );

            // base type -> base type (worse)
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<MyObservableCollection<int>>(
                    typeof(ObservableCollection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(Collection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // exact type -> base type
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<ObservableCollection<int>>(
                    typeof(ObservableCollection<int>), typeof(ZeroEnumerableFormatter<int>),
                    typeof(Collection<int>), typeof(OneEnumerableFormatter<int>) ) );

            // Array (generic base type) -> Array (base type)
            ExecuteOverwritingTest(
                fot => fot.EnsureDoesntOverwrite<int[]>(
                    typeof(Array), typeof(ZeroEnumerableFormatter<>),
                    typeof(Array), typeof(OneFormatter<Array>) ) );
        }

        private void EnsureDoesntOverwrite<TValue>(
            Type oldFormatterTargetType, Type oldFormatterType,
            Type newFormatterTargetType, Type newFormatterType )
        {
            string oldExpectedOutput = "0";

            string result;

            if ( currentLogBefore )
            {
                FormatterRepository<TestRole>.Get<TValue>();
            }

            FormatterRepository<TestRole>.Register( oldFormatterTargetType, oldFormatterType );

            if ( currentLogBetween )
            {
                result = Format( default(TValue) );

                Assert.Equal( oldExpectedOutput, result );
            }

            FormatterRepository<TestRole>.Register( newFormatterTargetType, newFormatterType );

            result = Format( default(TValue) );

            Assert.Equal( oldExpectedOutput, result );
        }
    }

    internal class ZeroFormatter<T> : Formatter<T>
    {
        public override void Write( UnsafeStringBuilder stringBuilder, T value )
        {
            stringBuilder.Append( 0 );
        }
    }

    internal class ZeroEnumerableFormatter<T> : ZeroFormatter<IEnumerable<T>>
    {
    }

    internal class OneFormatter<T> : Formatter<T>
    {
        public override void Write( UnsafeStringBuilder stringBuilder, T value )
        {
            stringBuilder.Append( 1 );
        }
    }

    internal class OneEnumerableFormatter<T> : OneFormatter<IEnumerable<T>>
    {
    }

    internal class MyObservableCollection<T> : ObservableCollection<T>
    {
    }
}

#endif