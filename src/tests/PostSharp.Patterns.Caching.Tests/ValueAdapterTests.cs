using PostSharp.Patterns.Caching.TestHelpers;
using PostSharp.Patterns.Common.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PostSharp.Patterns.Caching.Tests
{
    public class ValueAdapterTests : IDisposable
    {
        public ValueAdapterTests()
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( "Caching.Tests.ValueAdapterTests" );
        }

        public void Dispose()
        {
            TestProfileConfigurationFactory.DisposeTest();
        }

        [Fact]
        public void TestStream()
        {
            Stream s1 = this.MethodReturningStream();
            byte[] buffer1 = new byte[512];
            s1.Read( buffer1, 0, buffer1.Length );

            Assert.IsType<MemoryStream>( s1 );

            Stream s2 = this.MethodReturningStream();
            byte[] buffer2 = new byte[512];
            s2.Read(buffer2, 0, buffer2.Length);

            CompareArrays( buffer1, buffer2 );
            
        }
        [Fact]
        public async Task TestStreamAsync()
        {
            Stream s1 = await this.MethodReturningStreamAsync();
            byte[] buffer1 = new byte[512];
            s1.Read( buffer1, 0, buffer1.Length );

            Assert.IsType<MemoryStream>( s1 );

            Stream s2 = await this.MethodReturningStreamAsync();
            byte[] buffer2 = new byte[512];
            s2.Read(buffer2, 0, buffer2.Length);

            CompareArrays( buffer1, buffer2 );

        }

        [Fact]
        public void TestEnumerable()
        {
            int[] a1 = this.MethodReturningEnumerable().ToArray();
            int[] a2 = this.MethodReturningEnumerable().ToArray();

            CompareArrays(a1, a2);
        }

        [Fact]
        public void TestEnumerator()
        {
            IEnumerator<int> e1 = this.MethodReturningEnumerator();
            e1.MoveNext();
            IEnumerator<int> e2 = this.MethodReturningEnumerator();
            e2.MoveNext();
            Assert.Equal( e1.Current, e2.Current );
        }

        private static void CompareArrays<T>( T[] buffer1, T[] buffer2 )
        {
            Assert.Equal( buffer1.Length, buffer2.Length );
            for ( int i = 0; i < buffer1.Length; i++ )
            {
                Assert.Equal( buffer1[i], buffer2[i] );
            }
        }

        [Cache]
        private Stream MethodReturningStream()
        {
            return File.OpenRead( this.GetType().Assembly.Location );
        }

        [Cache]
        private async Task<Stream> MethodReturningStreamAsync()
        {
            await Task.Yield();
            return File.OpenRead( this.GetType().Assembly.Location );
        }

        [Cache]
        private IEnumerable<int> MethodReturningEnumerable()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        [Cache]
        private IEnumerator<int> MethodReturningEnumerator()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }
    }
}
