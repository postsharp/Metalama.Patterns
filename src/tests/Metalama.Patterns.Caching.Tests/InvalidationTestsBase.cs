using System;
using Xunit;
using Metalama.Patterns.Caching.TestHelpers;
using Metalama.Patterns.Common.Tests.Helpers;

namespace Metalama.Patterns.Caching.Tests
{
    public abstract class InvalidationTestsBase
    {
        protected sealed class CallsCounters
        {
            private readonly int[] counters;

            public int this[ int i ] => this.counters[i];

            public CallsCounters(int size)
            {
                this.counters = new int[size];

                for (int i = 0; i < this.counters.Length; i++)
                {
                    this.counters[i] = 0;
                }
            }

            public void Increment( int i )
            {
                this.counters[i]++;
            }
        }

        protected static void DoInvalidateCacheAttributeTest(
            string profileName,
            Func<CachedValueClass>[] cachedMethods,
            Func<CachedValueClass>[] invalidatingMethods,
            string testDescription,
            bool firstShouldWork,
            bool othersShouldWork,
            bool onlyPairsShouldWork = false
        )
        {
            TestProfileConfigurationFactory.InitializeTestWithCachingBackend( profileName );
            TestProfileConfigurationFactory.CreateProfile( profileName );

            try
            {
                Assert.Equal( cachedMethods.Length, invalidatingMethods.Length );

                CachedValueClass[] cachedMethodValuesBeforeInvalidation = new CachedValueClass[cachedMethods.Length];

                for ( int testedPairIndex = 0; testedPairIndex < cachedMethods.Length; testedPairIndex++ )
                {
                    for ( int cachedMethodIndex = 0; cachedMethodIndex < cachedMethods.Length; cachedMethodIndex++ )
                    {
                        cachedMethodValuesBeforeInvalidation[cachedMethodIndex] =
                            cachedMethods[cachedMethodIndex].Invoke();
                    }

                    CachedValueClass valueDuringInvalidation =
                        invalidatingMethods[testedPairIndex].Invoke();

                    AssertEx.Equal(
                        cachedMethodValuesBeforeInvalidation[testedPairIndex], valueDuringInvalidation,
                        $"{testDescription}: The value during invalidation of the invalidated method #{testedPairIndex} is different than the cached one when invalidating by the method #{testedPairIndex}." );

                    for ( int cachedMethodIndex = 0; cachedMethodIndex < cachedMethods.Length; cachedMethodIndex++ )
                    {
                        CachedValueClass cachedMethodValueAfterInvalidation =
                            cachedMethods[cachedMethodIndex].Invoke();

                        bool shouldBeInvalidated;

                        if ( onlyPairsShouldWork )
                        {
                            shouldBeInvalidated = cachedMethodIndex == testedPairIndex
                                                  && ((firstShouldWork && testedPairIndex == 0)
                                                      || (othersShouldWork && testedPairIndex > 0));
                        }
                        else
                        {
                            shouldBeInvalidated = cachedMethodIndex <= testedPairIndex
                                                  && ((firstShouldWork && cachedMethodIndex == 0)
                                                      || (othersShouldWork && cachedMethodIndex > 0));
                        }

                        if ( shouldBeInvalidated )
                        {
                            AssertEx.NotEqual(
                                cachedMethodValuesBeforeInvalidation[cachedMethodIndex], cachedMethodValueAfterInvalidation,
                                $"{testDescription}: The value after invalidation of the invalidated method #{cachedMethodIndex} is the same as the cached one when invalidating by the method #{testedPairIndex}." );
                        }
                        else
                        {
                            AssertEx.Equal(
                                cachedMethodValuesBeforeInvalidation[cachedMethodIndex], cachedMethodValueAfterInvalidation,
                                $"{testDescription}: The value after invalidation of the not invalidated method #{cachedMethodIndex} is different than the cached one when invalidating by the method #{testedPairIndex}." );
                        }
                    }
                }
            }
            finally
            {
                TestProfileConfigurationFactory.DisposeTest();
            }
        }
    }
}
