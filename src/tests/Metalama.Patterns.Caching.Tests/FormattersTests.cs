// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Formatters;
using Metalama.Patterns.Caching;
using Metalama.Patterns.Caching.Implementation;
using IFormattable = Metalama.Patterns.Formatters.IFormattable;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests
{
    public class FormattersTests
    {
        [Fact]
        public void TestSameClass()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register( new DogFormatter() );

            this.AssertKey( "FormattedDog", new Dog() );
        }

        private void AssertKey( string expectedKey, object o )
        {
            var cacheKeyBuilder = new CacheKeyBuilder();
            var key = cacheKeyBuilder.BuildDependencyKey( o );

            Assert.Equal( expectedKey, key );
        }

        [Fact]
        public void TestSameClassOverwritingFormatter()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register( new AnimalFormatter() );
            CachingServices.Formatters.Register( new DogFormatter() );

            this.AssertKey( "FormattedDog", new Dog() );
        }

        [Fact]
        public void TestDerivedClass()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register( new DogFormatter() );
            this.AssertKey( "FormattedDog", new Chihuahua() );
        }

        [Fact]
        public void TestInterface()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register( new AnimalFormatter() );

            this.AssertKey( "FormattedAnimal", new Cat() );
        }

        [Fact]
        public void TestManuallyFormatted()
        {
            CachingServices.Formatters.Reset();

            this.AssertKey( "ManuallyFormatted:Caching", new ManuallyFormatted() );
        }

        private interface IAnimal { }

        private class Cat : IAnimal { }

        private class Dog : IAnimal { }

        private class Chihuahua : Dog { }

        private class AnimalFormatter : Formatter<IAnimal>
        {
            public override void Write( UnsafeStringBuilder stringBuilder, IAnimal value )
            {
                stringBuilder.Append( "FormattedAnimal" );
            }
        }

        private class DogFormatter : Formatter<Dog>
        {
            public override void Write( UnsafeStringBuilder stringBuilder, Dog value )
            {
                stringBuilder.Append( "FormattedDog" );
            }
        }

        private class ManuallyFormatted : IFormattable
        {
            void IFormattable.Format( UnsafeStringBuilder stringBuilder, FormattingRole role )
            {
                stringBuilder.Append( "ManuallyFormatted:" + role.Name );
            }
        }
    }
}