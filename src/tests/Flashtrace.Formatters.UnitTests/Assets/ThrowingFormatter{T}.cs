// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Formatters.UnitTests;

internal class ThrowingFormatter<T> : Formatter<IEnumerable<T>>
{
    // ReSharper disable once StaticMemberInGenericType
#pragma warning disable SA1401
    public static bool Ran;
#pragma warning restore SA1401

    public ThrowingFormatter( IFormatterRepository repository ) : base( repository )
    {
        Ran = true;

#pragma warning disable CA2201
        throw new Exception();
#pragma warning restore CA2201
    }

    public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T>? value ) => throw new NotSupportedException();
}