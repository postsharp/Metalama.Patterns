﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Diagnostics;

namespace Metalama.Patterns.Caching.Tests
{
    public class InitializationAndCleanup
    {
        // TODO: !!! [ModuleInitializer( 0 )]
        public static void AssemblyInitialize()
        {
            Trace.Listeners.Add( new TextWriterTraceListener( Console.Out ) );
        }
    }
}