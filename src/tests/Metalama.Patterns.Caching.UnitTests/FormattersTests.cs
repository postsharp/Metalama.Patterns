// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Implementation;
using Xunit;
using IFormattable = Flashtrace.Formatters.IFormattable;

namespace Metalama.Patterns.Caching.Tests
{
    public sealed class FormattersTests
    {
        [Fact]
        public void TestSameClass()
        {
            var formatters = new CachingFormatterRepository();
            formatters.Register( new DogFormatter( formatters ) );

            AssertKey( formatters, "FormattedDog", new Dog() );
        }

        private static void AssertKey( FormatterRepository formatters, string expectedKey, object o )
        {
            var cacheKeyBuilder = new CacheKeyBuilder( formatters );
            var key = cacheKeyBuilder.BuildDependencyKey( o );

            Assert.Equal( expectedKey, key );
        }

        [Fact]
        public void TestSameClassOverwritingFormatter()
        {
            var formatters = new CachingFormatterRepository();
            formatters.Register( new AnimalFormatter( formatters ) );
            formatters.Register( new DogFormatter( formatters ) );

            AssertKey( formatters, "FormattedDog", new Dog() );
        }

        [Fact]
        public void TestDerivedClass()
        {
            var formatters = new CachingFormatterRepository();
            formatters.Register( new DogFormatter( formatters ) );
            AssertKey( formatters, "FormattedDog", new Chihuahua() );
        }

        [Fact]
        public void TestInterface()
        {
            var formatters = new CachingFormatterRepository();
            formatters.Register( new AnimalFormatter( formatters ) );

            AssertKey( formatters, "FormattedAnimal", new Cat() );
        }

        [Fact]
        public void TestManuallyFormatted()
        {
            var formatters = new CachingFormatterRepository();

            AssertKey( formatters, "ManuallyFormatted:Caching", new ManuallyFormatted() );
        }

        private interface IAnimal { }

        private sealed class Cat : IAnimal { }

        private class Dog : IAnimal { }

        private sealed class Chihuahua : Dog { }

        private sealed class AnimalFormatter : Formatter<IAnimal>
        {
            public AnimalFormatter( IFormatterRepository repository ) : base( repository ) { }

            public override void Write( UnsafeStringBuilder stringBuilder, IAnimal? value )
            {
                stringBuilder.Append( "FormattedAnimal" );
            }
        }

        private sealed class DogFormatter : Formatter<Dog>
        {
            public DogFormatter( IFormatterRepository repository ) : base( repository ) { }

            public override void Write( UnsafeStringBuilder stringBuilder, Dog? value )
            {
                stringBuilder.Append( "FormattedDog" );
            }
        }

        private sealed class ManuallyFormatted : IFormattable
        {
            void IFormattable.Format( UnsafeStringBuilder stringBuilder, IFormatterRepository formatterRepository )
            {
                stringBuilder.Append( "ManuallyFormatted:" + formatterRepository.Role.Name );
            }
        }
    }
}