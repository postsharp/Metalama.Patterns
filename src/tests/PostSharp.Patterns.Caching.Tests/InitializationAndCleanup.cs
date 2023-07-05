using PostSharp.Aspects;
using System;
using System.Collections;
using System.Diagnostics;

namespace PostSharp.Patterns.Caching.Tests
{
    public class InitializationAndCleanup
    {
        [ModuleInitializer( 0 )]
        public static void AssemblyInitialize()
        {
            Trace.Listeners.Add( new TextWriterTraceListener( Console.Out ) );
        }
    }
}
