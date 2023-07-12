﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#if NETCOREAPP3_0_OR_GREATER
using Metalama.Patterns.Caching.TestHelpers;
using System.Text;
using Xunit.Abstractions;

namespace Metalama.Patterns.Caching.Tests;

public abstract class AsyncEnumTestsBase : IDisposable
{
    protected ITestOutputHelper TestOutputHelper { get; }

    protected StringBuilder StringBuilder { get; }

    protected TestClass Instance { get; }

    public AsyncEnumTestsBase( ITestOutputHelper testOutputHelper )
    {
        this.TestOutputHelper = testOutputHelper;
        this.StringBuilder = new();
        this.Instance = new TestClass( this.Log );

        TestProfileConfigurationFactory.InitializeTestWithCachingBackend( nameof( AsyncEnumerableTests ) );
    }

    public void Dispose()
    {
        TestProfileConfigurationFactory.DisposeTest();
        this.TestOutputHelper.WriteLine( this.StringBuilder.ToString() );
    }

    protected void Log( string message )
    {
        if ( this.StringBuilder.Length > 0 )
        {
            this.StringBuilder.Append( "." );
        }
        this.StringBuilder.Append( message );
    }

    protected async Task Iterate( IAsyncEnumerator<int> iterator )
    {
        this.Log( "I1" );
        while ( await iterator.MoveNextAsync() )
        {
            this.Log( $"I2[{iterator.Current}]" );
        }
        this.Log( "I3" );
    }

    public sealed class TestClass
    {
        private readonly Action<string> _log;

        public TestClass( Action<string> log )
        {
            this._log = log;
        }

        [Cache( IgnoreThisParameter = true )]
        public async IAsyncEnumerable<int> CachedEnumerable()
        {
            this._log( "E1" );
            await Task.Delay( 1 );
            this._log( "E2" );
            yield return 42;
            this._log( "E3" );
            yield return 99;
            this._log( "E4" );
        }

        [Cache( IgnoreThisParameter = true )]
        public async IAsyncEnumerator<int> CachedEnumerator()
        {
            this._log( "E1" );
            await Task.Delay( 1 );
            this._log( "E2" );
            yield return 42;
            this._log( "E3" );
            yield return 99;
            this._log( "E4" );
        }
    }
}

#endif