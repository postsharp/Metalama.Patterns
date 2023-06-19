// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters.UnitTests.Formatters;
using System.Collections;
using System.Collections.ObjectModel;
using Xunit;
using Xunit.Abstractions;

namespace Flashtrace.Formatters.UnitTests
{
    public class FormatterRegistrationTests : FormattersTestsBase
    {
        public FormatterRegistrationTests( ITestOutputHelper logger ) : base( logger )
        {
        }

        [Fact]
        public void RegisterBeforeFirstGet()
        {
            var formatter = new EnumerableFormatter<int>( this.DefaultRepository );
            this.DefaultRepository.Register( formatter );

            var afterFormatter = this.DefaultRepository.Get<IEnumerable<int>>();

            Assert.Same( formatter, afterFormatter );
        }

        [Fact]
        public void RegisterAfterFirstGet()
        {
            var beforeFormatter = this.DefaultRepository.Get<IEnumerable<int>>();
            Assert.Equal( "DynamicFormatter`1", beforeFormatter!.GetType().Name );

            var formatter = new EnumerableFormatter<int>( this.DefaultRepository );
            this.DefaultRepository.Register( formatter );

            var afterFormatter = this.DefaultRepository.Get<IEnumerable<int>>();

            Assert.Same( formatter, afterFormatter );
        }

        [Fact]
        public void RegisterBeforeFirstLog()
        {
            var formatter = new EnumerableFormatter<int>( this.DefaultRepository );
            this.DefaultRepository.Register( formatter );

            var result = this.FormatDefault<IEnumerable<int>>( new[] { 1, 2, 3 } );

            Assert.Equal( "[1,2,3]", result );
        }

        [Fact]
        public void RegisterAfterFirstLog()
        {
            var result = this.FormatDefault<IEnumerable<int>>( new[] { 1, 2, 3 } );
            Assert.Equal( "{int[]}", result );

            this.DefaultRepository.Register( new EnumerableFormatter<int>( this.DefaultRepository ) );

            result = this.FormatDefault<IEnumerable<int>>( new[] { 1, 2, 3 } );

            Assert.Equal( "[1,2,3]", result );
        }

        [Fact]
        public void CanChangeBack()
        {
            int[] array = { 1, 2, 3 };

            var result = this.FormatDefault<IEnumerable<int>>( array );
            Assert.Equal( "{int[]}", result );

            this.DefaultRepository.Register( new EnumerableFormatter<int>( this.DefaultRepository ) );

            result = this.FormatDefault<IEnumerable<int>>( array );
            Assert.Equal( "[1,2,3]", result );

            this.DefaultRepository.Register( new DefaultFormatter<IEnumerable<int>>( this.DefaultRepository ) );

            result = this.FormatDefault<IEnumerable<int>>( array );
            Assert.Equal( "{int[]}", result );
        }

        [Fact]
        public void EnumerableIntFormatter()
        {
            this.EnumerableIntFormatterMethod( false, new EnumerableFormatter<int>( this.DefaultRepository ) );
            this.EnumerableIntFormatterMethod( true, new EnumerableFormatter<int>( this.DefaultRepository ) );
            this.EnumerableIntFormatterMethod( false, new EnumerableIntFormatter( this.DefaultRepository ) );
            this.EnumerableIntFormatterMethod( true, new EnumerableIntFormatter( this.DefaultRepository ) );
        }

        private void EnumerableIntFormatterMethod( bool logFirst, Formatter<IEnumerable<int>> formatter )
        {
            int[] array = { 1, 2, 3 };

            if ( logFirst )
            {
                this.DefaultRepository.Get<int[]>();
                this.DefaultRepository.Get<IEnumerable<int>>();
                this.DefaultRepository.Get<IEnumerable>();
            }

            this.DefaultRepository.Register( formatter );

            Assert.Equal( "[1,2,3]", this.FormatDefault<int[]>( array ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<IEnumerable<int>>( array ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<IEnumerable>( array ) );
        }

        [InlineData( true )]
        [InlineData( false )]
        [Theory]
        public void EnumerableTFormatter( bool logFirst )
        {
            int[] array = { 1, 2, 3 };

            if ( logFirst )
            {
                this.DefaultRepository.Get<int[]>();
                this.DefaultRepository.Get<IEnumerable<int>>();
                this.DefaultRepository.Get<IEnumerable>();
            }

            this.DefaultRepository.Register( typeof( IEnumerable<> ), typeof( EnumerableFormatter<> ) );

            Assert.Equal( "[1,2,3]", this.FormatDefault<int[]>( array ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<IEnumerable<int>>( array ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<IEnumerable>( array ) );
        }

        [InlineData( true )]
        [InlineData( false )]
        [Theory]
        public void CollectionTFormatter( bool logFirst )
        {
            var collection = new ObservableCollection<int> { 1, 2, 3 };

            if ( logFirst )
            {
                this.DefaultRepository.Get<ObservableCollection<int>>();
                this.DefaultRepository.Get<Collection<int>>();
                this.DefaultRepository.Get<IEnumerable<int>>();
            }

            this.DefaultRepository.Register( typeof( Collection<> ), typeof( EnumerableFormatter<> ) );

            Assert.Equal( "[1,2,3]", this.FormatDefault<ObservableCollection<int>>( collection ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<Collection<int>>( collection ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<IEnumerable<int>>( collection ) );
        }

        [InlineData( true )]
        [InlineData( false )]
        [Theory]
        public void ArrayFormatter( bool logFirst )
        {
            int[] array = { 1, 2, 3 };

            if ( logFirst )
            {
                this.DefaultRepository.Get<int[]>();
                this.DefaultRepository.Get<IEnumerable<int>>();
                this.DefaultRepository.Get<IEnumerable>();
            }

            this.DefaultRepository.Register( typeof( Array ), typeof( EnumerableFormatter<> ) );

            Assert.Equal( "[1,2,3]", this.FormatDefault<int[]>( array ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<IEnumerable<int>>( array ) );
            Assert.Equal( "[1,2,3]", this.FormatDefault<IEnumerable>( array ) );
        }

        [InlineData( true )]
        [InlineData( false )]
        [Theory]
        public void DictionaryFormatter( bool logFirst )
        {
            var dictionary = new Dictionary<int, string> { { 1, "uno" }, { 2, "dos" }, { 3, "tres" } };

            if ( logFirst )
            {
                this.DefaultRepository.Get<Dictionary<int, string>>();
                this.DefaultRepository.Get<IDictionary<int, string>>();
                this.DefaultRepository.Get<IDictionary>();
            }

            this.DefaultRepository.Register( typeof( IDictionary<,> ), typeof( DictionaryFormatter<,> ) );

            Assert.Equal( "{1:uno,2:dos,3:tres}", this.FormatDefault<Dictionary<int, string>>( dictionary ) );
            Assert.Equal( "{1:uno,2:dos,3:tres}", this.FormatDefault<IDictionary<int, string>>( dictionary ) );
            Assert.Equal( "{1:uno,2:dos,3:tres}", this.FormatDefault<IDictionary>( dictionary ) );
        }

        [InlineData( true )]
        [InlineData( false )]
        [Theory]
        public void NullableFormatter( bool logFirst )
        {
            if ( logFirst )
            {
                this.DefaultRepository.Get<int?>();
                this.DefaultRepository.Get<int>();
            }

            this.DefaultRepository.Register( typeof( int? ), typeof( NullableFormatter<int> ) );

            Assert.Equal( "<null>", this.FormatDefault<int?>( null ) );
            Assert.Equal( "2", this.FormatDefault<int>( 2 ) );
        }

        [InlineData( true )]
        [InlineData( false )]
        [Theory]
        public void GenericNullableFormatter( bool logFirst )
        {
            if ( logFirst )
            {
                this.DefaultRepository.Get<int?>();
                this.DefaultRepository.Get<int>();
            }

            this.DefaultRepository.Register( typeof( Nullable<> ), typeof( NullableFormatter<> ) );

            Assert.Equal( "<null>", this.FormatDefault<int?>( null ) );
            Assert.Equal( "2", this.FormatDefault<int>( 2 ) );
        }

        [InlineData( true )]
        [InlineData( false )]
        [Theory]
        public void NonNullableFormatter( bool logFirst )
        {
            if ( logFirst )
            {
                this.DefaultRepository.Get<int?>();
                this.DefaultRepository.Get<int>();
            }

            this.DefaultRepository.Register( typeof( int ), typeof( NonNullableFormatter<int> ) );

            Assert.Equal( "null", this.FormatDefault<int?>( null ) );
            Assert.Equal( "[2]", this.FormatDefault<int>( 2 ) );
        }

        [Fact]
        public void FormatterExceptions()
        {
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( int[] ), typeof( EnumerableFormatter<> ) ) );
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( int ), typeof( EnumerableFormatter<int> ) ) );
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( IEnumerable<> ), typeof( EnumerableFormatter<int> ) ) );
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( IEnumerable<int> ), typeof( EnumerableFormatter<> ) ) );
            AssertEx.Throws<MissingMethodException>( () => this.DefaultRepository.Register( typeof( int ), typeof( int ) ) );
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( IEnumerable<> ), typeof( IEnumerable<> ) ) );
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( Dictionary<,> ), typeof( EnumerableFormatter<> ) ) );

            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( IEnumerable<> ), new EnumerableFormatter<int>( this.DefaultRepository ) ) );
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( int ), new EnumerableFormatter<int>( this.DefaultRepository ) ) );

            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( int ), typeof( NullableFormatter<int> ) ) );
            AssertEx.Throws<ArgumentException>( () => this.DefaultRepository.Register( typeof( int? ), typeof( NonNullableFormatter<int> ) ) );
        }

        [Fact]
        public void NoVariance()
        {
            this.DefaultRepository.Register( new EnumerableFormatter<object>( this.DefaultRepository ) );

            string[] array = { "foo", "bar", "baz" };

            Assert.Equal( "{string[]}", this.FormatDefault<IEnumerable<string>>( array ) );
            Assert.Equal( "[foo,bar,baz]", this.FormatDefault<IEnumerable<object>>( array ) );
        }
    }

    internal class EnumerableFormatter<T> : Formatter<IEnumerable<T>>
    {
        public EnumerableFormatter( IFormatterRepository repository ) : base( repository )
        {
        }

        public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
        {
            stringBuilder.Append( '[' );
            stringBuilder.Append( string.Join( ",", value ) );
            stringBuilder.Append( ']' );
        }
    }

    internal class EnumerableIntFormatter : EnumerableFormatter<int>
    {
        public EnumerableIntFormatter( IFormatterRepository repository ) : base( repository )
        {
        }
    }

    internal class DictionaryFormatter<TKey, TValue> : Formatter<IDictionary<TKey, TValue>>
    {
        public DictionaryFormatter( IFormatterRepository repository ) : base( repository )
        {
        }

        public override void Write( UnsafeStringBuilder stringBuilder, IDictionary<TKey, TValue> value )
        {
            stringBuilder.Append(
                "{" + string.Join( ",", value.Select( kvp => string.Format( "{0}:{1}", kvp.Key, kvp.Value ) ) ) + "}" );
        }
    }

    internal class NonNullableFormatter<T> : Formatter<T>
        where T : struct
    {
        public NonNullableFormatter( IFormatterRepository repository ) : base( repository )
        {
        }

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
        public NullableFormatter( IFormatterRepository repository ) : base( repository )
        {
        }

        public override void Write( UnsafeStringBuilder stringBuilder, T? value )
        {
            stringBuilder.Append( '<' );
            stringBuilder.Append( value == null ? "null" : value.ToString() );
            stringBuilder.Append( '>' );
        }
    }
}