// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Metalama.Patterns.Tests.Helpers;

public class AssertEx
{
    // NOTE: Xunit doesn't support assert messages by design (except for True and False methods).

    public static void NotNull( object expected, string message )
    {
        Xunit.Assert.NotNull( expected );
    }

    public static void Null( object expectedNull, string message )
    {
        Xunit.Assert.Null( expectedNull );
    }

    public static void NotEqual( object notExpected, object actual, string message )
    {
        Xunit.Assert.NotEqual( notExpected, actual );
    }

    public static void Equal( object expected, object actual, string message )
    {
        Xunit.Assert.Equal( expected, actual );
    }

    public static void Equal( long expected, long actual, string message )
    {
        Xunit.Assert.Equal( expected, actual );
    }

    public static void Equal( int expected, int actual, string message )
    {
        Xunit.Assert.Equal( expected, actual );
    }

    public static void NotSame( object notExpected, object actual, string message )
    {
        Xunit.Assert.NotSame( notExpected, actual );
    }

    public static void Inconclusive()
    {
        // TODO: implement a replacement for Inconclusive, see https://github.com/AArnott/Xunit.SkippableFact
        // Xunit doesn't support inconclusive tests
        Xunit.Assert.True( true, "Inconclusive" );
    }

    public static void Inconclusive( string message )
    {
        // TODO: implement a replacement for Inconclusive, see https://github.com/AArnott/Xunit.SkippableFact
        // Xunit doesn't support inconclusive tests
        Xunit.Assert.True( true, message );
    }

    public static void Fail( string message )
    {
        Xunit.Assert.True( false, message );
    }

    public static void Fail()
    {
        Xunit.Assert.True( false );
    }

    public static void EqualSet<T>( IEnumerable<T> expected, IEnumerable<T> actual )
    {
        Xunit.Assert.Equal( new HashSet<T>( expected ), new HashSet<T>( actual ) );
    }

    public static void ContainsAll<T>( IEnumerable<T> expected, ICollection<T> actual )
    {
        foreach ( T item in expected )
        {
            Xunit.Assert.Contains( item, actual );
        }
    }

    // TODO Inline this.
    public static void Throws<T>(Action task) where T : Exception
    {
        Xunit.Assert.Throws<T>( task );
    }
    
    // TODO Inline this.
    public static Task ThrowsAsync<T>(Func<Task> task) where T : Exception
    {
        return Xunit.Assert.ThrowsAsync<T>( task );
    }

    public static void Throws<T>(string expectedMessage, Action task) where T : Exception
    {
        try
        {
            task();
        }
        catch (Exception ex)
        {
            AssertExceptionType<T>(ex);
            AssertExceptionMessage(ex, expectedMessage);
            return;
        }

        Xunit.Assert.True( false, string.Format( "Expected exception of type {0} but no exception was thrown.", typeof( T ) ) );
    }

    public static async Task ThrowsAsync<T>(string expectedMessage, Func<Task> task) where T : Exception
    {
        try
        {
            await task();
        }
        catch (Exception ex)
        {
            AssertExceptionType<T>(ex);
            AssertExceptionMessage(ex, expectedMessage);
            return;
        }

        Xunit.Assert.True( false, string.Format( "Expected exception of type {0} but no exception was thrown.", typeof( T ) ) );
    }

    private static void AssertExceptionType<T>(Exception ex)
    {
        Xunit.Assert.True( ex.GetType().Equals( typeof( T ) ), "Expected exception type failed." );
    }

    private static void AssertExceptionMessage(Exception ex, string expectedMessage)
    {
        Xunit.Assert.True(
            string.Equals( expectedMessage.ToLowerInvariant(), ex.Message.ToLowerInvariant(), StringComparison.Ordinal ),
            "Expected exception message failed." );
    }

    public static void ContainsMultiple(string expectedText, int expectedCount, string actualText)
    {
        int nextStart = 0;
        int actualCount = 0;

        while(true)
        {
            int index = actualText.IndexOf( expectedText, nextStart );

            if (index != -1)
            {
                nextStart = index + 1;
                actualCount++;
            }
            else
            {
                break;
            }
        }

        Xunit.Assert.True(
            actualCount == expectedCount,
            $"Expected string \"{expectedText}\" to be present {expectedCount} times but it was present {actualCount} times." );
    }
}
