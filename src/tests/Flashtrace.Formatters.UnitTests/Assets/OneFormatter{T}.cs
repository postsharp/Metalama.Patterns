﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters.UnitTests;

internal class OneFormatter<T> : Formatter<T>
{
    public OneFormatter( IFormatterRepository repository ) : base( repository ) { }

    public override void Write( UnsafeStringBuilder stringBuilder, T? value )
    {
        stringBuilder.Append( 1 );
    }
}