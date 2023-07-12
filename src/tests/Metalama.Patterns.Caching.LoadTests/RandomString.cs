// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;

namespace Metalama.Patterns.Caching.LoadTests
{
    // These strings would be less random when using multithreaded
    public static class RandomString
    {
        // Random is not thread-safe
        [ThreadStatic]
        private static Random random;

        public static char[] AllowedChars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#&@{}:;".ToCharArray();

        //"abc".ToCharArray();

        public static string New( int minLength, int maxLength )
        {
            if ( random == null )
            {
                random = new Random();
            }

            var length = random.Next( minLength, maxLength + 1 );

            return New( length );
        }

        public static string New( int length )
        {
            if ( random == null )
            {
                random = new Random();
            }

            var chars = new char[length];

            for ( var i = 0; i < length; i++ )
            {
                chars[i] = AllowedChars[random.Next( 0, AllowedChars.Length )];
            }

            return new string( chars );
        }
    }
}