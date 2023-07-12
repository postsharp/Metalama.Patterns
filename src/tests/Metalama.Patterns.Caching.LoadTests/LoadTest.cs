// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.LoadTests
{
    public struct Interval
    {
        public int Min;

        public int Max;

        public Interval( int min, int max )
        {
            this.Min = min;
            this.Max = max;
        }
    }

    public class LoadTest : IDisposable
    {
        private class TestClient : IDisposable
        {
            [Serializable]
            private class Payload
            {
                public string Value { get; set; }

                public string[] Dependencies { get; set; }
            }

            private readonly LoadTest loadTest;
            private readonly CachingBackend backend;
            private Thread thread;
            private CancellationTokenSource cancellationTokenSource;
            private readonly Random random = new();
            private const int randomnessMultiplier = 1000;

            private BigInteger setItemCount = BigInteger.Zero;
            private BigInteger cacheMissCount = BigInteger.Zero;
            private BigInteger cacheHitCount = BigInteger.Zero;
            private BigInteger invalidateDependencyCount = BigInteger.Zero;
            private BigInteger removeItemCount = BigInteger.Zero;
            private BigInteger invalidatedDependenciesCount = BigInteger.Zero;
            private readonly object invalidatedDependenciesCountLock = new();
            private BigInteger evictedRemovedItemsCount = BigInteger.Zero;
            private BigInteger expiredRemovedItemsCount = BigInteger.Zero;
            private BigInteger invalidatedRemovedItemsCount = BigInteger.Zero;
            private BigInteger removedItemsCount = BigInteger.Zero;
            private BigInteger otherRemovedItemsCount = BigInteger.Zero;
            private BigInteger unknownRemovedItemsCount = BigInteger.Zero;
            private BigInteger sharedDependencies = BigInteger.Zero;
            private readonly object removedItemsCountLock = new();
            private readonly StringCounter errorsCounters = new();

            public TestClient( LoadTest loadTest, Func<CachingBackend> cachingBackendFactory )
            {
                this.loadTest = loadTest;
                this.backend = cachingBackendFactory.Invoke();

                this.backend.DependencyInvalidated += ( sender, args ) =>
                {
                    lock ( this.invalidatedDependenciesCountLock )
                    {
                        this.invalidatedDependenciesCount++;
                    }
                };

                this.backend.ItemRemoved += ( sender, args ) =>
                {
                    lock ( this.removedItemsCountLock )
                    {
                        switch ( args.RemovedReason )
                        {
                            case CacheItemRemovedReason.Evicted:
                                this.evictedRemovedItemsCount++;

                                break;

                            case CacheItemRemovedReason.Expired:
                                this.expiredRemovedItemsCount++;

                                break;

                            case CacheItemRemovedReason.Invalidated:
                                this.invalidatedRemovedItemsCount++;

                                break;

                            case CacheItemRemovedReason.Removed:
                                this.removedItemsCount++;

                                break;

                            case CacheItemRemovedReason.Other:
                                this.otherRemovedItemsCount++;

                                break;

                            default:
                                this.unknownRemovedItemsCount++;

                                break;
                        }
                    }
                };
            }

            public void Start()
            {
                if ( this.thread != null )
                {
                    throw new InvalidOperationException( "The client is already running." );
                }

                this.cancellationTokenSource = new CancellationTokenSource();
                this.thread = new Thread( this.Run );
                this.thread.Start();
            }

            private void Run()
            {
                while ( !this.cancellationTokenSource.IsCancellationRequested )
                {
                    var nextAction = this.GetNextRandomNumber( 4 );

                    switch ( nextAction )
                    {
                        case 0:
                            this.SetItem();

                            break;

                        case 1:
                            this.GetItem();

                            break;

                        case 2:
                            this.RemoveItem();

                            break;

                        case 3:
                            this.InvalidateDependency();

                            break;

                        default:
                            throw new Exception( "Unknown action " + nextAction );
                    }
                }
            }

            private int GetNextRandomNumber( int maxValue )
            {
                return this.GetNextRandomNumber( 0, maxValue );
            }

            private int GetNextRandomNumber( Interval interval )
            {
                return this.GetNextRandomNumber( interval.Min, interval.Max + 1 );
            }

            private int GetNextRandomNumber( int minValue, int maxValue )
            {
                var range = maxValue - minValue;

                return this.random.Next( 0, range * randomnessMultiplier ) % range + minValue;
            }

            private string GetRandomValueKey()
            {
                var index = this.GetNextRandomNumber( this.loadTest.valueKeys.Length );

                return this.loadTest.valueKeys[index];
            }

            private string GetRandomDependencyKey()
            {
                var index = this.GetNextRandomNumber( this.loadTest.dependencyKeys.Length );

                return this.loadTest.dependencyKeys[index];
            }

            private void SetItem( string sharedDependency = null )
            {
                var key = this.GetRandomValueKey();

                var payload = new Payload();

                payload.Value = new string( 'a', this.GetNextRandomNumber( this.loadTest.configuration.ValueLength ) );

                var dependenciesCount = this.GetNextRandomNumber( this.loadTest.configuration.DependenciesPerValueCount );

                if ( sharedDependency != null )
                {
                    dependenciesCount++;
                }

                CacheItem item;

                var configuration = new CacheItemConfiguration()
                {
                    SlidingExpiration =
                        TimeSpan.FromSeconds( this.GetNextRandomNumber( this.loadTest.configuration.ValueKeyExpiry ) )
                };

                string dependencyToShare = null;

                if ( dependenciesCount == 0 )
                {
                    item = new CacheItem( payload, configuration: configuration );
                }
                else
                {
                    string[] dependencies = new string[dependenciesCount];

                    for ( var i = sharedDependency == null ? 0 : 1; i < dependencies.Length; i++ )
                    {
                        dependencies[i] = this.GetRandomDependencyKey();
                    }

                    if ( sharedDependency == null )
                    {
                        dependencyToShare = dependencies[0];
                    }
                    else
                    {
                        dependencies[0] = sharedDependency;
                    }

                    payload.Dependencies = dependencies;

                    item = new CacheItem( payload, ImmutableList.Create( dependencies ), configuration );
                }

                this.backend.SetItem( key, item );
                this.setItemCount++;

                if ( dependencyToShare != null )
                {
                    this.sharedDependencies++;

                    var valuesSharingDependency = this.GetNextRandomNumber( this.loadTest.configuration.ValuesPerSharedDependency );

                    for ( var i = 0; i < valuesSharingDependency; i++ )
                    {
                        this.SetItem( dependencyToShare );
                    }
                }
            }

            private void GetItem()
            {
                var key = this.GetRandomValueKey();
                var value = this.backend.GetItem( key );

                if ( value == null )
                {
                    this.cacheMissCount++;

                    return;
                }

                this.cacheHitCount++;

                var payload = value.Value as Payload;

                if ( payload == null )
                {
                    this.errorsCounters.Increment(
                        "Corrupted payload type.",
                        value.Value == null
                            ? "payload is null"
                            : value.Value.GetType().FullName + ", " + value.Value.ToString() );

                    return;
                }

                var retrievedDependenciesList = value.Dependencies ?? ImmutableList<string>.Empty;

                if ( payload.Dependencies == null && retrievedDependenciesList.Count == 0 )
                {
                    // No dependencies as expected
                    return;
                }

                if ( payload.Dependencies != null && payload.Dependencies.Length == 0 )
                {
                    this.errorsCounters.Increment( "Corrupted payload - dependencies missing." );
                }

                if ( payload.Dependencies == null && retrievedDependenciesList.Count > 0 )
                {
                    this.errorsCounters.Increment(
                        "Corrupted dependencies - dependencies not expected but retrieved.",
                        $"Retrieved: {string.Join( " ", retrievedDependenciesList )}" );

                    return;
                }

                if ( payload.Dependencies != null && payload.Dependencies.Length != retrievedDependenciesList.Count )
                {
                    this.errorsCounters.Increment(
                        "Corrupted dependencies - different number of expected and retrieved dependencies.",
                        $"Expected: {string.Join( " ", retrievedDependenciesList )} Retrieved: {string.Join( " ", payload.Dependencies )}" );

                    return;
                }

                HashSet<string> retrievedDependencies = new( retrievedDependenciesList );

                foreach ( var expectedDependency in payload.Dependencies )
                {
                    if ( !retrievedDependencies.Contains( expectedDependency ) )
                    {
                        this.errorsCounters.Increment(
                            "Expected dependency not present.",
                            $"Expected: {string.Join( " ", retrievedDependenciesList )} Retrieved: {string.Join( " ", payload.Dependencies )}" );

                        return;
                    }
                }
            }

            private void RemoveItem()
            {
                var key = this.GetRandomValueKey();
                this.backend.RemoveItem( key );
                this.removeItemCount++;
            }

            private void InvalidateDependency()
            {
                var dependency = this.GetRandomDependencyKey();
                this.backend.InvalidateDependency( dependency );
                this.invalidateDependencyCount++;
            }

            public void Stop()
            {
                this.cancellationTokenSource.Cancel();
                this.thread.Join();
                this.thread = null;
            }

            public void Report()
            {
                Console.WriteLine( "Operations:" );
                Console.WriteLine( $"setItemCount: {this.setItemCount}" );
                Console.WriteLine( $"sharedDependenciesCount: {this.sharedDependencies}" );
                Console.WriteLine( $"cacheMissCount: {this.cacheMissCount}" );
                Console.WriteLine( $"cacheHitCount: {this.cacheHitCount}" );
                Console.WriteLine( $"invalidateDependencyCount: {this.invalidateDependencyCount}" );
                Console.WriteLine( $"removeItemCount: {this.removeItemCount}" );
                Console.WriteLine( "Events:" );
                Console.WriteLine( $"invalidatedDependenciesCount: {this.invalidatedDependenciesCount}" );
                Console.WriteLine( $"evictedRemovedItemsCount: {this.evictedRemovedItemsCount}" );
                Console.WriteLine( $"expiredRemovedItemsCount: {this.expiredRemovedItemsCount}" );
                Console.WriteLine( $"invalidatedRemovedItemsCount: {this.invalidatedRemovedItemsCount}" );
                Console.WriteLine( $"removedItemsCount: {this.removedItemsCount}" );
                Console.WriteLine( $"otherRemovedItemsCount: {this.otherRemovedItemsCount}" );
                Console.WriteLine( $"unknownRemovedItemsCount: {this.unknownRemovedItemsCount}" );
                Console.WriteLine( "Errors:" );

                if ( this.errorsCounters.Counters.Count > 0 )
                {
                    foreach ( KeyValuePair<string, BigInteger> error in this.errorsCounters.Counters )
                    {
                        Console.WriteLine( $"> {error.Key}: {error.Value}" );

                        List<string> details;

                        if ( this.errorsCounters.Details.TryGetValue( error.Key, out details ) )
                        {
                            foreach ( var detail in details )
                            {
                                Console.WriteLine( detail );
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine( "No error." );
                }
            }

            public void Dispose()
            {
                this.backend.Dispose();
            }
        }

        private readonly LoadTestConfiguration configuration;

        private string[] valueKeys;

        private string[] dependencyKeys;

        private TestClient[] clients;

        public LoadTest( LoadTestConfiguration configuration )
        {
            this.configuration = configuration;
        }

        public void Initialize( Func<CachingBackend> backendFactory )
        {
            this.valueKeys = GenerateKeys( this.configuration.ValueKeysCount, this.configuration.ValueKeyLenght );
            this.dependencyKeys = GenerateKeys( this.configuration.DependencyKeysCount, this.configuration.DependencyKeyLenght );
            this.clients = new TestClient[this.configuration.ClientsCount];

            for ( var i = 0; i < this.configuration.ClientsCount; i++ )
            {
                this.clients[i] = new TestClient( this, backendFactory );
            }
        }

        private static string[] GenerateKeys( int count, Interval length )
        {
            string[] keys = new string[count];

            for ( var i = 0; i < count; i++ )
            {
                keys[i] = RandomString.New( length.Min, length.Max );
            }

            return keys;
        }

        public void Start()
        {
            Task[] startTasks = new Task[this.configuration.ClientsCount];

            for ( var i = 0; i < this.configuration.ClientsCount; i++ )
            {
                var client = this.clients[i];
                startTasks[i] = Task.Run( () => client.Start() );
            }

            Task.WaitAll( startTasks );
        }

        public void Stop()
        {
            Task[] stopTasks = new Task[this.configuration.ClientsCount];

            for ( var i = 0; i < this.configuration.ClientsCount; i++ )
            {
                var client = this.clients[i];
                stopTasks[i] = Task.Run( () => client.Stop() );
            }

            Task.WaitAll( stopTasks );
        }

        public void Report()
        {
            for ( var i = 0; i < this.clients.Length; i++ )
            {
                Console.WriteLine( $"Client {i + 1}" );
                this.clients[i].Report();
                Console.WriteLine();
            }
        }

        public void Dispose()
        {
            Task[] disposeTasks = new Task[this.configuration.ClientsCount];

            for ( var i = 0; i < this.configuration.ClientsCount; i++ )
            {
                var client = this.clients[i];
                disposeTasks[i] = Task.Run( () => client.Dispose() );
            }

            Task.WaitAll( disposeTasks );
        }
    }
}