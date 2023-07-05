// @MinToolsVersion(15.0)
// @ExpectedMessage(LA0164)

namespace PostSharp.Patterns.Caching.BuildTests.LanguageFeatures
{
    namespace RefReturnSimple
    {
        public class Program
        {
            static int Main()
            {
                int a = 42;
                ref int x = ref TestClass.SimpleMethod(a);

                return 0;
            }
        }

        public class TestClass
        {
            public static int field;
            [Cache]
            public static ref int SimpleMethod(int a)
            {
                return ref field;
            }
        }
    }
}
