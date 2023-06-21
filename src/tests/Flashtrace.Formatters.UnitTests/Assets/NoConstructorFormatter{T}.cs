﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters.UnitTests;

internal class NoConstructorFormatter<T> : Formatter<IEnumerable<T>>
{
    private NoConstructorFormatter() : base( null! ) { }

    public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T>? value )
    {
        throw new NotSupportedException();
    }
}