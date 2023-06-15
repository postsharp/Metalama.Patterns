// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

/* Ported from PostSharp.
 * 
 * Action: Skiping, already covered by unit tests in InterfaceTests.
 */

// @Skipped(Already covered by unit tests in InterfaceTests)

// #LangVersion(8.0)
// #ExcludeFramework(NET)
// #MinFrameworkVersion(NETCore,3.0)
// #SkipILVerify

using System;

namespace Metalama.Patterns.Contracts.AspectTests
{
    namespace DefaultInterfaceMethods
    {
        public class Program
        {
            public static void Main()
            {
                try
                {
                    IWithDefaults ii = new Impl();
                    Console.WriteLine("Returning " + ii.LogMe( 1 ));  
                }
                catch (ArgumentOutOfRangeException)
                {
                    return;
                }

                throw new Exception("There should have been an exception.");
            }
            public static int LogMe( [Negative] int number )
            {
                return number;
            }
        } 
        
        public class Impl : IWithDefaults
        {
            
        } 
        
        public interface IWithDefaults {
            public int LogMe( [Negative] int number )
            {
                return number;
            }
        }
    }
}