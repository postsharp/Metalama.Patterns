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

            int length = random.Next( minLength, maxLength + 1 );

            return New( length );
        }

        public static string New( int length )
        {
            if (random == null)
            {
                random = new Random();
            }

            char[] chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = AllowedChars[random.Next(0, AllowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
