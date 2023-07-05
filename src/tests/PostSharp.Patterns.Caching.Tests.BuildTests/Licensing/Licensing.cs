// @SkipOS(Unix)
// @Property(MaxFreeEnhancedLocInProject=4)
// @DisableUnattendedCheck
// @Optimize(False)

// @License(Essentials)
// @RequiredSymbol(Tags="premium")

using System;

namespace PostSharp.Patterns.Caching.BuildTests
{
    namespace Licensing
    {
        public class Program
        {
            public static void Main()
            {
            }
        }

        public class EnhancedClass
        {
            [Cache]
            public int Method1()
            {
                int a = 20
                        + 40;
                return a;
            }
        }
    }
}