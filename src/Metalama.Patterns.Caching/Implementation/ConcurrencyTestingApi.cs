// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Ported from PostSharp.Patterns.Common/Threading

namespace Metalama.Patterns.Caching.Implementation
{
    internal static class ConcurrencyTestingApi
    {
        public static ConcurrencyTestingApiImpl Implementation;
            
        [Conditional( "DEBUG" )]
        public static void TraceEvent( string message )
        {
            if (Implementation != null)
                Implementation.TraceEvent(message);
        }
    }

    internal abstract class ConcurrencyTestingApiImpl
    {
        public abstract void TraceEvent( string message );
    }
}
