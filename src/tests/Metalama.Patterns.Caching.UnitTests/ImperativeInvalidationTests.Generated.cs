// THIS FILE IS T4-GENERATED.
// To edit, go to CacheInvalidation.Generated.tt.
// To transform, run this: "C:\Program Files (x86)\Common Files\Microsoft Shared\TextTemplating\14.0\TextTransform.exe" CacheInvalidation.Generated.tt
// The transformation is not going to be automatic once we are in a shared project.


using System.Threading.Tasks;
using Xunit;
using Metalama.Patterns.Caching.TestHelpers;

namespace Metalama.Patterns.Caching.Tests
{
	public sealed partial class ImperativeInvalidationTests
    {
		
		#region TestSimpleImperativeInvalidationWith1Parameters

        private const string testSimpleImperativeInvalidationWith1ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith1Parameters);

        class TestSimpleImperativeInvalidationWith1ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith1ParametersProfileName)]
            public override CachedValueClass GetValue( int param1 )
            {
                return base.GetValue( param1 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith1Parameters()
        {
            TestSimpleImperativeInvalidationWith1ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith1ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith1ParametersProfileName,
                                                () => cachingClass.GetValue( 1 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith1ParametersAsync

        private const string testSimpleImperativeInvalidationWith1ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith1ParametersAsync);

        class TestSimpleImperativeInvalidationWith1ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith1ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await base.GetValueAsync( param1 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith1ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith1ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith1ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith1ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith1Parameters

        private const string testSimpleImperativeRecachingWith1ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith1Parameters);

        class TestSimpleImperativeRecachingWith1ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith1ParametersProfileName )]
            public override CachedValueClass GetValue( int param1 )
            {
                return base.GetValue( param1 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith1Parameters()
        {
            TestSimpleImperativeRecachingWith1ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith1ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith1ParametersProfileName,
                                                () => cachingClass.GetValue( 1 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith1ParametersAsync

        private const string testSimpleImperativeRecachingWith1ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith1ParametersAsync);

        class TestSimpleImperativeRecachingWith1ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith1ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1 )
            {
                return await base.GetValueAsync( param1 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith1ParametersAsync()
        {
            TestSimpleImperativeRecachingWith1ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith1ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith1ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith2Parameters

        private const string testSimpleImperativeInvalidationWith2ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith2Parameters);

        class TestSimpleImperativeInvalidationWith2ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith2ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2 )
            {
                return base.GetValue( param1, param2 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith2Parameters()
        {
            TestSimpleImperativeInvalidationWith2ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith2ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith2ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith2ParametersAsync

        private const string testSimpleImperativeInvalidationWith2ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith2ParametersAsync);

        class TestSimpleImperativeInvalidationWith2ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith2ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2 )
            {
                return await base.GetValueAsync( param1, param2 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith2ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith2ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith2ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith2ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith2Parameters

        private const string testSimpleImperativeRecachingWith2ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith2Parameters);

        class TestSimpleImperativeRecachingWith2ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith2ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2 )
            {
                return base.GetValue( param1, param2 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith2Parameters()
        {
            TestSimpleImperativeRecachingWith2ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith2ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith2ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith2ParametersAsync

        private const string testSimpleImperativeRecachingWith2ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith2ParametersAsync);

        class TestSimpleImperativeRecachingWith2ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith2ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2 )
            {
                return await base.GetValueAsync( param1, param2 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith2ParametersAsync()
        {
            TestSimpleImperativeRecachingWith2ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith2ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith2ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith3Parameters

        private const string testSimpleImperativeInvalidationWith3ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith3Parameters);

        class TestSimpleImperativeInvalidationWith3ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith3ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3 )
            {
                return base.GetValue( param1, param2, param3 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith3Parameters()
        {
            TestSimpleImperativeInvalidationWith3ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith3ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith3ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith3ParametersAsync

        private const string testSimpleImperativeInvalidationWith3ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith3ParametersAsync);

        class TestSimpleImperativeInvalidationWith3ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith3ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3 )
            {
                return await base.GetValueAsync( param1, param2, param3 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith3ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith3ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith3ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith3ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith3Parameters

        private const string testSimpleImperativeRecachingWith3ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith3Parameters);

        class TestSimpleImperativeRecachingWith3ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith3ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3 )
            {
                return base.GetValue( param1, param2, param3 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith3Parameters()
        {
            TestSimpleImperativeRecachingWith3ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith3ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith3ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith3ParametersAsync

        private const string testSimpleImperativeRecachingWith3ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith3ParametersAsync);

        class TestSimpleImperativeRecachingWith3ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith3ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3 )
            {
                return await base.GetValueAsync( param1, param2, param3 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith3ParametersAsync()
        {
            TestSimpleImperativeRecachingWith3ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith3ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith3ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith4Parameters

        private const string testSimpleImperativeInvalidationWith4ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith4Parameters);

        class TestSimpleImperativeInvalidationWith4ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith4ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4 )
            {
                return base.GetValue( param1, param2, param3, param4 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith4Parameters()
        {
            TestSimpleImperativeInvalidationWith4ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith4ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith4ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3, 4 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith4ParametersAsync

        private const string testSimpleImperativeInvalidationWith4ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith4ParametersAsync);

        class TestSimpleImperativeInvalidationWith4ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith4ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith4ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith4ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith4ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith4ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3, 4 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith4Parameters

        private const string testSimpleImperativeRecachingWith4ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith4Parameters);

        class TestSimpleImperativeRecachingWith4ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith4ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4 )
            {
                return base.GetValue( param1, param2, param3, param4 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith4Parameters()
        {
            TestSimpleImperativeRecachingWith4ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith4ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith4ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3, 4 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith4ParametersAsync

        private const string testSimpleImperativeRecachingWith4ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith4ParametersAsync);

        class TestSimpleImperativeRecachingWith4ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith4ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith4ParametersAsync()
        {
            TestSimpleImperativeRecachingWith4ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith4ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith4ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3, 4 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith5Parameters

        private const string testSimpleImperativeInvalidationWith5ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith5Parameters);

        class TestSimpleImperativeInvalidationWith5ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith5ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5 )
            {
                return base.GetValue( param1, param2, param3, param4, param5 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith5Parameters()
        {
            TestSimpleImperativeInvalidationWith5ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith5ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith5ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3, 4, 5 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith5ParametersAsync

        private const string testSimpleImperativeInvalidationWith5ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith5ParametersAsync);

        class TestSimpleImperativeInvalidationWith5ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith5ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith5ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith5ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith5ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith5ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith5Parameters

        private const string testSimpleImperativeRecachingWith5ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith5Parameters);

        class TestSimpleImperativeRecachingWith5ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith5ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5 )
            {
                return base.GetValue( param1, param2, param3, param4, param5 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith5Parameters()
        {
            TestSimpleImperativeRecachingWith5ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith5ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith5ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3, 4, 5 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith5ParametersAsync

        private const string testSimpleImperativeRecachingWith5ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith5ParametersAsync);

        class TestSimpleImperativeRecachingWith5ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith5ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith5ParametersAsync()
        {
            TestSimpleImperativeRecachingWith5ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith5ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith5ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith6Parameters

        private const string testSimpleImperativeInvalidationWith6ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith6Parameters);

        class TestSimpleImperativeInvalidationWith6ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith6ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith6Parameters()
        {
            TestSimpleImperativeInvalidationWith6ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith6ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith6ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3, 4, 5, 6 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith6ParametersAsync

        private const string testSimpleImperativeInvalidationWith6ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith6ParametersAsync);

        class TestSimpleImperativeInvalidationWith6ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith6ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith6ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith6ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith6ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith6ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith6Parameters

        private const string testSimpleImperativeRecachingWith6ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith6Parameters);

        class TestSimpleImperativeRecachingWith6ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith6ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith6Parameters()
        {
            TestSimpleImperativeRecachingWith6ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith6ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith6ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3, 4, 5, 6 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith6ParametersAsync

        private const string testSimpleImperativeRecachingWith6ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith6ParametersAsync);

        class TestSimpleImperativeRecachingWith6ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith6ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith6ParametersAsync()
        {
            TestSimpleImperativeRecachingWith6ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith6ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith6ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith7Parameters

        private const string testSimpleImperativeInvalidationWith7ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith7Parameters);

        class TestSimpleImperativeInvalidationWith7ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith7ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith7Parameters()
        {
            TestSimpleImperativeInvalidationWith7ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith7ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith7ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith7ParametersAsync

        private const string testSimpleImperativeInvalidationWith7ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith7ParametersAsync);

        class TestSimpleImperativeInvalidationWith7ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith7ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith7ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith7ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith7ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith7ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith7Parameters

        private const string testSimpleImperativeRecachingWith7ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith7Parameters);

        class TestSimpleImperativeRecachingWith7ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith7ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith7Parameters()
        {
            TestSimpleImperativeRecachingWith7ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith7ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith7ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith7ParametersAsync

        private const string testSimpleImperativeRecachingWith7ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith7ParametersAsync);

        class TestSimpleImperativeRecachingWith7ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith7ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith7ParametersAsync()
        {
            TestSimpleImperativeRecachingWith7ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith7ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith7ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith8Parameters

        private const string testSimpleImperativeInvalidationWith8ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith8Parameters);

        class TestSimpleImperativeInvalidationWith8ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith8ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7, param8 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith8Parameters()
        {
            TestSimpleImperativeInvalidationWith8ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith8ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith8ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith8ParametersAsync

        private const string testSimpleImperativeInvalidationWith8ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith8ParametersAsync);

        class TestSimpleImperativeInvalidationWith8ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith8ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7, param8 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith8ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith8ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith8ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith8ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith8Parameters

        private const string testSimpleImperativeRecachingWith8ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith8Parameters);

        class TestSimpleImperativeRecachingWith8ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith8ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7, param8 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith8Parameters()
        {
            TestSimpleImperativeRecachingWith8ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith8ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith8ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith8ParametersAsync

        private const string testSimpleImperativeRecachingWith8ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith8ParametersAsync);

        class TestSimpleImperativeRecachingWith8ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith8ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7, param8 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith8ParametersAsync()
        {
            TestSimpleImperativeRecachingWith8ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith8ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith8ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7, 8 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith9Parameters

        private const string testSimpleImperativeInvalidationWith9ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith9Parameters);

        class TestSimpleImperativeInvalidationWith9ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith9ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7, param8, param9 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith9Parameters()
        {
            TestSimpleImperativeInvalidationWith9ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith9ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith9ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith9ParametersAsync

        private const string testSimpleImperativeInvalidationWith9ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith9ParametersAsync);

        class TestSimpleImperativeInvalidationWith9ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith9ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7, param8, param9 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith9ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith9ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith9ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith9ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith9Parameters

        private const string testSimpleImperativeRecachingWith9ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith9Parameters);

        class TestSimpleImperativeRecachingWith9ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith9ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7, param8, param9 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith9Parameters()
        {
            TestSimpleImperativeRecachingWith9ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith9ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith9ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith9ParametersAsync

        private const string testSimpleImperativeRecachingWith9ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith9ParametersAsync);

        class TestSimpleImperativeRecachingWith9ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith9ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7, param8, param9 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith9ParametersAsync()
        {
            TestSimpleImperativeRecachingWith9ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith9ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith9ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7, 8, 9 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
		#region TestSimpleImperativeInvalidationWith10Parameters

        private const string testSimpleImperativeInvalidationWith10ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith10Parameters);

        class TestSimpleImperativeInvalidationWith10ParametersCachingClass : CachingClass
        {
            [Cache(ProfileName = testSimpleImperativeInvalidationWith10ParametersProfileName)]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9, int param10 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7, param8, param9, param10 );
            }
        }

        [Fact]
        public void TestSimpleImperativeInvalidationWith10Parameters()
        {
            TestSimpleImperativeInvalidationWith10ParametersCachingClass cachingClass = new TestSimpleImperativeInvalidationWith10ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeInvalidationWith10ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                () => CachingServices.Default.Invalidation.Invalidate( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeInvalidationWith10ParametersAsync

        private const string testSimpleImperativeInvalidationWith10ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeInvalidationWith10ParametersAsync);

        class TestSimpleImperativeInvalidationWith10ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeInvalidationWith10ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9, int param10 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7, param8, param9, param10 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeInvalidationWith10ParametersAsync()
        {
            TestSimpleImperativeInvalidationWith10ParametersAsyncCachingClass cachingClass =
                new TestSimpleImperativeInvalidationWith10ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeInvalidationWith10ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                           () => CachingServices.Default.Invalidation.InvalidateAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                           cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith10Parameters

        private const string testSimpleImperativeRecachingWith10ParametersProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith10Parameters);

        class TestSimpleImperativeRecachingWith10ParametersCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith10ParametersProfileName )]
            public override CachedValueClass GetValue( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9, int param10 )
            {
                return base.GetValue( param1, param2, param3, param4, param5, param6, param7, param8, param9, param10 );
            }
        }

        [Fact]
        public void TestSimpleImperativeRecachingWith10Parameters()
        {
            TestSimpleImperativeRecachingWith10ParametersCachingClass cachingClass = new TestSimpleImperativeRecachingWith10ParametersCachingClass();

            DoTestSimpleImperativeInvalidation( testSimpleImperativeRecachingWith10ParametersProfileName,
                                                () => cachingClass.GetValue( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                () => CachingServices.Default.Invalidation.Recache( cachingClass.GetValue, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                cachingClass.Reset );
        }

        #endregion

		#region TestSimpleImperativeRecachingWith10ParametersAsync

        private const string testSimpleImperativeRecachingWith10ParametersAsyncProfileName =
            _profileNamePrefix + nameof(TestSimpleImperativeRecachingWith10ParametersAsync);

        class TestSimpleImperativeRecachingWith10ParametersAsyncCachingClass : CachingClass
        {
            [Cache( ProfileName = testSimpleImperativeRecachingWith10ParametersAsyncProfileName )]
            public override async Task<CachedValueClass> GetValueAsync( int param1, int param2, int param3, int param4, int param5, int param6, int param7, int param8, int param9, int param10 )
            {
                return await base.GetValueAsync( param1, param2, param3, param4, param5, param6, param7, param8, param9, param10 );
            }
        }

        [Fact]
        public async Task TestSimpleImperativeRecachingWith10ParametersAsync()
        {
            TestSimpleImperativeRecachingWith10ParametersAsyncCachingClass cachingClass = new TestSimpleImperativeRecachingWith10ParametersAsyncCachingClass();

            await DoTestSimpleImperativeInvalidationAsync( testSimpleImperativeRecachingWith10ParametersAsyncProfileName,
                                                           () => cachingClass.GetValueAsync( 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                           () => CachingServices.Default.Invalidation.RecacheAsync( cachingClass.GetValueAsync, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ),
                                                           cachingClass.Reset );
        }

        #endregion

		
	}
}

