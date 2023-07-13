﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

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
            ((FormatterRepository.IUnitTesting) CachingServices.Formatters.Instance).Reset();
            CachingServices.Formatters.Instance.Register( new DogFormatter( CachingServices.Formatters.Instance ) );

            AssertKey( "FormattedDog", new Dog() );
        }

        private static void AssertKey( string expectedKey, object o )
        {
            var cacheKeyBuilder = new CacheKeyBuilder();
            var key = cacheKeyBuilder.BuildDependencyKey( o );

            Assert.Equal( expectedKey, key );
        }

        [Fact]
        public void TestSameClassOverwritingFormatter()
        {
            ((FormatterRepository.IUnitTesting) CachingServices.Formatters.Instance).Reset();
            CachingServices.Formatters.Instance.Register( new AnimalFormatter( CachingServices.Formatters.Instance ) );
            CachingServices.Formatters.Instance.Register( new DogFormatter( CachingServices.Formatters.Instance ) );

            AssertKey( "FormattedDog", new Dog() );
        }

        [Fact]
        public void TestDerivedClass()
        {
            ((FormatterRepository.IUnitTesting) CachingServices.Formatters.Instance).Reset();
            CachingServices.Formatters.Instance.Register( new DogFormatter( CachingServices.Formatters.Instance ) );
            AssertKey( "FormattedDog", new Chihuahua() );
        }

        [Fact]
        public void TestInterface()
        {
            ((FormatterRepository.IUnitTesting) CachingServices.Formatters.Instance).Reset();
            CachingServices.Formatters.Instance.Register( new AnimalFormatter( CachingServices.Formatters.Instance ) );

            AssertKey( "FormattedAnimal", new Cat() );
        }

        [Fact]
        public void TestManuallyFormatted()
        {
            ((FormatterRepository.IUnitTesting) CachingServices.Formatters.Instance).Reset();

            AssertKey( "ManuallyFormatted:Caching", new ManuallyFormatted() );
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