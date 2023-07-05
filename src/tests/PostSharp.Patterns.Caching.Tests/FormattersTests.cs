using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PostSharp.Patterns.Formatters;
using PostSharp.Patterns.Caching;
using PostSharp.Patterns.Caching.Implementation;
using IFormattable = PostSharp.Patterns.Formatters.IFormattable;
using PostSharp.Patterns.Common.Tests.Helpers;

namespace PostSharp.Patterns.Caching.Tests
{
    public class FormattersTests
    {

        [Fact]
        public void TestSameClass()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register( new DogFormatter() );

            this.AssertKey("FormattedDog", new Dog());
        }

        private void AssertKey( string expectedKey, object o )
        {
            CacheKeyBuilder cacheKeyBuilder = new CacheKeyBuilder();
            string key = cacheKeyBuilder.BuildDependencyKey(o);

            Assert.Equal(expectedKey, key);
        }


        [Fact]
        public void TestSameClassOverwritingFormatter()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register(new AnimalFormatter());
            CachingServices.Formatters.Register(new DogFormatter());

            this.AssertKey("FormattedDog", new Dog());

        }

        [Fact]
        public void TestDerivedClass()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register(new DogFormatter());
            this.AssertKey("FormattedDog", new Chihuahua());

        }


        [Fact]
        public void TestInterface()
        {
            CachingServices.Formatters.Reset();
            CachingServices.Formatters.Register(new AnimalFormatter());

            this.AssertKey("FormattedAnimal", new Cat());

        }


        [Fact]
        public void TestManuallyFormatted()
        {
            CachingServices.Formatters.Reset();

            this.AssertKey("ManuallyFormatted:Caching", new ManuallyFormatted());

        }


        interface IAnimal
        {

        }

        class Cat : IAnimal
        {

        }

        class Dog : IAnimal
        {

        }

        class Chihuahua : Dog
        {

        }

        class AnimalFormatter : Formatter<IAnimal>
        {
            public override void Write( UnsafeStringBuilder stringBuilder, IAnimal value )
            {
                stringBuilder.Append("FormattedAnimal");
            }
        }

        class DogFormatter : Formatter<Dog>
        {
            public override void Write( UnsafeStringBuilder stringBuilder, Dog value )
            {
                stringBuilder.Append("FormattedDog");
            }
        }

        class ManuallyFormatted : IFormattable
        {
            void IFormattable.Format( UnsafeStringBuilder stringBuilder, FormattingRole role )
            {
                stringBuilder.Append("ManuallyFormatted:" + role.Name);
            }
        }


    }
}
