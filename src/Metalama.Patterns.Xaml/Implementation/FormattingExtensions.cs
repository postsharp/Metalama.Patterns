// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.Text;

namespace Metalama.Patterns.Xaml.Implementation;

[CompileTime]
internal static class FormattingExtensions
{
    public static string PrettyList( this IEnumerable<string> words, string conjunction )
    {
        var iter = words.GetEnumerator();

        var a = iter.MoveNext() ? iter.Current : null;

        if ( a == null )
        {
            return string.Empty;
        }

        var b = iter.MoveNext() ? iter.Current : null;

        if ( b == null )
        {
            return a;
        }

        var sb = new StringBuilder();

        while ( iter.MoveNext() )
        {
            sb.Append( a ).Append( ',' ).Append( ' ' );

            a = b;
            b = iter.Current;
        }

        sb.Append( a ).Append( conjunction ).Append( b );

        return sb.ToString();
    }
}