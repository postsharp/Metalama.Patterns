// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if APP_DOMAIN

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Formatters;


namespace PostSharp.Patterns.Common.Tests.Formatters
{
    public class FormatterRegistrationTests : AppDomainTestsBaseClass<FormatterRegistrationTests>
    {
        [Fact]
        public void RegisterBeforeFirstGet()
        {
            ExecuteTest( tc => tc.RegisterBeforeFirstGetMethod() );
        }

        private void RegisterBeforeFirstGetMethod()
        {
            EnumerableFormatter<int> formatter = new EnumerableFormatter<int>();
            FormatterRepository<TestRole>.Register( formatter );

            IFormatter<IEnumerable<int>> afterFormatter = FormatterRepository<TestRole>.Get<IEnumerable<int>>();

            Assert.Same( formatter, afterFormatter );
        }

        [Fact]
        public void RegisterAfterFirstGet()
        {
            ExecuteTest( tc => tc.RegisterAfterFirstGetMethod() );
        }

        private void RegisterAfterFirstGetMethod()
        {
            IFormatter<IEnumerable<int>> beforeFormatter = FormatterRepository<TestRole>.Get<IEnumerable<int>>();
            Assert.Equal( "DynamicFormatter`2", beforeFormatter.GetType().Name );

            EnumerableFormatter<int> formatter = new EnumerableFormatter<int>();
            FormatterRepository<TestRole>.Register( formatter );

            IFormatter<IEnumerable<int>> afterFormatter = FormatterRepository<TestRole>.Get<IEnumerable<int>>();

            Assert.Same( formatter, afterFormatter );
        }

        private static string Format<T>(T value)
        {
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder(1024);
            FormatterRepository<TestRole>.Get<T>().Write(stringBuilder, value);
            return stringBuilder.ToString();
        }

        [Fact]
        public void RegisterBeforeFirstLog()
        {
            ExecuteTest( tc => tc.RegisterBeforeFirstLogMethod() );
        }

        private void RegisterBeforeFirstLogMethod()
        {
            EnumerableFormatter<int> formatter = new EnumerableFormatter<int>();
            FormatterRepository<TestRole>.Register( formatter );

            string result = Format<IEnumerable<int>>( new[] {1, 2, 3} );

            Assert.Equal( "[1,2,3]", result );
        }

        [Fact]
        public void RegisterAfterFirstLog()
        {
            ExecuteTest( tc => tc.RegisterAfterFirstLogMethod() );
        }

        private void RegisterAfterFirstLogMethod()
        {
            string result = Format<IEnumerable<int>>( new[] {1, 2, 3} );
            Assert.Equal( "{int[]}", result );

            FormatterRepository<TestRole>.Register( new EnumerableFormatter<int>() );

            result = Format<IEnumerable<int>>( new[] {1, 2, 3} );

            Assert.Equal( "[1,2,3]", result );
        }

        [Fact]
        public void CanChangeBack()
        {
            ExecuteTest( tc => tc.CanChangeBackMethod() );
        }

        private void CanChangeBackMethod()
        {
            int[] array = {1, 2, 3};

            string result = Format<IEnumerable<int>>( array );
            Assert.Equal("{int[]}", result );

            FormatterRepository<TestRole>.Register( new EnumerableFormatter<int>() );

            result = Format<IEnumerable<int>>( array );
            Assert.Equal( "[1,2,3]", result );

            FormatterRepository<TestRole>.Register( new DefaultFormatter<TestRole,IEnumerable<int>>() );

            result = Format<IEnumerable<int>>( array );
            Assert.Equal("{int[]}", result );
        }

        [Fact]
        public void EnumerableIntFormatter()
        {
            ExecuteTest( tc => tc.EnumerableIntFormatterMethod( false, new EnumerableFormatter<int>() ) );
            ExecuteTest( tc => tc.EnumerableIntFormatterMethod( true, new EnumerableFormatter<int>() ) );
            ExecuteTest( tc => tc.EnumerableIntFormatterMethod( false, new EnumerableIntFormatter() ) );
            ExecuteTest( tc => tc.EnumerableIntFormatterMethod( true, new EnumerableIntFormatter() ) );
        }

        private void EnumerableIntFormatterMethod( bool logFirst, Formatter<IEnumerable<int>> formatter )
        {
            int[] array = {1, 2, 3};

            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<int[]>();
                FormatterRepository<TestRole>.Get<IEnumerable<int>>();
                FormatterRepository<TestRole>.Get<IEnumerable>();
            }

            FormatterRepository<TestRole>.Register( formatter );

            Assert.Equal( "[1,2,3]", Format<int[]>( array ) );
            Assert.Equal( "[1,2,3]", Format<IEnumerable<int>>( array ) );
            Assert.Equal("[1,2,3]", Format<IEnumerable>( array ) );
        }

        [Fact]
        public void EnumerableTFormatter()
        {
            ExecuteTest( tc => tc.EnumerableTFormatterMethod( false ) );
            ExecuteTest( tc => tc.EnumerableTFormatterMethod( true ) );
        }

        private void EnumerableTFormatterMethod( bool logFirst )
        {
            int[] array = {1, 2, 3};

            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<int[]>();
                FormatterRepository<TestRole>.Get<IEnumerable<int>>();
                FormatterRepository<TestRole>.Get<IEnumerable>();
            }

            FormatterRepository<TestRole>.Register( typeof(IEnumerable<>), typeof(EnumerableFormatter<>) );

            Assert.Equal( "[1,2,3]", Format<int[]>( array ) );
            Assert.Equal( "[1,2,3]", Format<IEnumerable<int>>( array ) );
            Assert.Equal("[1,2,3]", Format<IEnumerable>( array ) );
        }

        [Fact]
        public void CollectionTFormatter()
        {
            ExecuteTest( tc => tc.CollectionTFormatterMethod( false ) );
            ExecuteTest( tc => tc.CollectionTFormatterMethod( true ) );
        }

        private void CollectionTFormatterMethod( bool logFirst )
        {
            ObservableCollection<int> collection = new ObservableCollection<int> {1, 2, 3};

            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<ObservableCollection<int>>();
                FormatterRepository<TestRole>.Get<Collection<int>>();
                FormatterRepository<TestRole>.Get<IEnumerable<int>>();
            }

            FormatterRepository<TestRole>.Register( typeof(Collection<>), typeof(EnumerableFormatter<>) );

            Assert.Equal( "[1,2,3]", Format<ObservableCollection<int>>( collection ) );
            Assert.Equal( "[1,2,3]", Format<Collection<int>>( collection ) );
            Assert.Equal("[1,2,3]", Format<IEnumerable<int>>( collection ) );
        }

        [Fact]
        public void ArrayFormatter()
        {
            ExecuteTest( tc => tc.ArrayFormatterMethod( false ) );
            ExecuteTest( tc => tc.ArrayFormatterMethod( true ) );
        }

        private void ArrayFormatterMethod( bool logFirst )
        {
            int[] array = {1, 2, 3};

            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<int[]>();
                FormatterRepository<TestRole>.Get<IEnumerable<int>>();
                FormatterRepository<TestRole>.Get<IEnumerable>();
            }

            FormatterRepository<TestRole>.Register( typeof(Array), typeof(EnumerableFormatter<>) );

            Assert.Equal( "[1,2,3]", Format<int[]>( array ) );
            Assert.Equal("[1,2,3]", Format<IEnumerable<int>>( array ) );
            Assert.Equal("[1,2,3]", Format<IEnumerable>( array ) );
        }

        [Fact]
        public void DictionaryFormatter()
        {
            ExecuteTest( tc => tc.DictionaryFormatterMethod( false ) );
            ExecuteTest( tc => tc.DictionaryFormatterMethod( true ) );
        }

        private void DictionaryFormatterMethod( bool logFirst )
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string> {{1, "uno"}, {2, "dos"}, {3, "tres"}};

            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<Dictionary<int, string>>();
                FormatterRepository<TestRole>.Get<IDictionary<int, string>>();
                FormatterRepository<TestRole>.Get<IDictionary>();
            }

            FormatterRepository<TestRole>.Register( typeof(IDictionary<,>), typeof(DictionaryFormatter<,>) );

            Assert.Equal( "{1:uno,2:dos,3:tres}", Format<Dictionary<int, string>>( dictionary ) );
            Assert.Equal( "{1:uno,2:dos,3:tres}", Format<IDictionary<int, string>>( dictionary ) );
            Assert.Equal("{1:uno,2:dos,3:tres}", Format<IDictionary>( dictionary ) );
        }

        [Fact]
        public void NullableFormatter()
        {
            ExecuteTest( tc => tc.NullableFormatterMethod( false ) );
            ExecuteTest( tc => tc.NullableFormatterMethod( true ) );
        }

        private void NullableFormatterMethod( bool logFirst )
        {
            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<int?>();
                FormatterRepository<TestRole>.Get<int>();
            }

            FormatterRepository<TestRole>.Register( typeof(int?), typeof(NullableFormatter<int>) );

            Assert.Equal( "<null>", Format<int?>( null ) );
            Assert.Equal( "2", Format<int>( 2 ) );
        }

        [Fact]
        public void GenericNullableFormatter()
        {
            ExecuteTest( tc => tc.GenericNullableFormatterMethod( false ) );
            ExecuteTest( tc => tc.GenericNullableFormatterMethod( true ) );
        }

        private void GenericNullableFormatterMethod( bool logFirst )
        {
            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<int?>();
                FormatterRepository<TestRole>.Get<int>();
            }

            FormatterRepository<TestRole>.Register( typeof(Nullable<>), typeof(NullableFormatter<>) );

            Assert.Equal( "<null>", Format<int?>( null ) );
            Assert.Equal( "2", Format<int>( 2 ) );
        }

        [Fact]
        public void NonNullableFormatter()
        {
            ExecuteTest( tc => tc.NonNullableFormatterMethod( false ) );
            ExecuteTest( tc => tc.NonNullableFormatterMethod( true ) );
        }

        private void NonNullableFormatterMethod( bool logFirst )
        {
            if ( logFirst )
            {
                FormatterRepository<TestRole>.Get<int?>();
                FormatterRepository<TestRole>.Get<int>();
            }

            FormatterRepository<TestRole>.Register( typeof(int), typeof(NonNullableFormatter<int>) );

            Assert.Equal( "null", Format<int?>( null ) );
            Assert.Equal( "[2]", Format<int>( 2 ) );
        }

        [Fact]
        public void FormatterExceptions()
        {
            ExecuteTest( tc => tc.FormatterExceptionsMethod() );
        }

        private void FormatterExceptionsMethod()
        {
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(int[]), typeof(EnumerableFormatter<>) ) );
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(int), typeof(EnumerableFormatter<int>) ) );
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(IEnumerable<>), typeof(EnumerableFormatter<int>) ) );
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(IEnumerable<int>), typeof(EnumerableFormatter<>) ) );
            AssertEx.Throws<InvalidCastException>( () => FormatterRepository<TestRole>.Register( typeof(int), typeof(int) ) );
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(IEnumerable<>), typeof(IEnumerable<>) ) );
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(Dictionary<,>), typeof(EnumerableFormatter<>) ) );

            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(IEnumerable<>), new EnumerableFormatter<int>() ) );
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(int), new EnumerableFormatter<int>() ) );

            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(int), typeof(NullableFormatter<int>) ) );
            AssertEx.Throws<ArgumentException>( () => FormatterRepository<TestRole>.Register( typeof(int?), typeof(NonNullableFormatter<int>) ) );
        }

        [Fact]
        public void NoVariance()
        {
            ExecuteTest( tc => tc.NoVarianceMethod() );
        }

        private void NoVarianceMethod()
        {
            FormatterRepository<TestRole>.Register( new EnumerableFormatter<object>() );

            string[] array = {"foo", "bar", "baz"};

            Assert.Equal( "{string[]}", Format<IEnumerable<string>>( array ) );
            Assert.Equal( "[foo,bar,baz]", Format<IEnumerable<object>>( array ) );
        }
    }

    internal class EnumerableFormatter<T> : Formatter<IEnumerable<T>>
    {
        public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
        {
            stringBuilder.Append( '[' );
            stringBuilder.Append( string.Join( ",", value ) );
            stringBuilder.Append( ']' );
        }
    }

    internal class EnumerableIntFormatter : EnumerableFormatter<int>
    {
    }

    internal class DictionaryFormatter<TKey, TValue> : Formatter<IDictionary<TKey, TValue>>
    {
        public override void Write( UnsafeStringBuilder stringBuilder, IDictionary<TKey, TValue> value )
        {
            stringBuilder.Append(
                "{" + string.Join( ",", value.Select( kvp => string.Format( "{0}:{1}", kvp.Key, kvp.Value ) ) ) + "}" );
        }
    }

    internal class NonNullableFormatter<T> : Formatter<T>
        where T : struct
    {
        public override void Write( UnsafeStringBuilder stringBuilder, T value )
        {
            stringBuilder.Append( '[' );
            stringBuilder.Append( value.ToString() );
            stringBuilder.Append( ']' );
        }
    }

    internal class NullableFormatter<T> : Formatter<T?>
        where T : struct
    {
        public override void Write( UnsafeStringBuilder stringBuilder, T? value )
        {
            stringBuilder.Append( '<' );
            stringBuilder.Append( value == null ? "null" : value.ToString() );
            stringBuilder.Append( '>' );
        }
    }
}

#endif