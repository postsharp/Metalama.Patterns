﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.UnitTests;

public sealed partial class LogEventDataTests
{
    private sealed class PropertiesWithAttributes
    {
        [LoggingPropertyOptions( IsIgnored = true )]
        public string Ignored { get; set; }

        [LoggingPropertyOptions( IsInherited = true )]
        public string Inherited { get; set; }
    }
}