// @SkipOS(Unix)
// @Property(MaxFreeEnhancedLocInProject=1)
// @DisableUnattendedCheck
// @Optimize(False)

// @TestRun(Essentials_Exceeded) =License(Essentials) =ExpectedMessage(PS0243)
// @TestRun(Caching) =License(Caching) =ForbiddenSymbol(Tags="premium")

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

        public class EnhancedClassBig
        {
            [Cache]
            public int Method1()
            {
                int a = 20
                        + 40;
                return a;
            }
            
            public int Method2()
            {
                int a = 20
                        + 40;
                return a;
            }
        }
    }
}