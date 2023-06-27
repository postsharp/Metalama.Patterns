// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections.Generic;
using PostSharp.Patterns.Formatters;

namespace PostSharp.Patterns.Caching.Formatters
{
    internal class CollectionFormatter<TKind, T> : Formatter<IEnumerable<T>>
        where TKind : FormattingRole, new()
    {
        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
        {
            if ( value == null )
            {
                stringBuilder.Append( 'n', 'u', 'l', 'l' );
                return;
            }

            IFormatter<T> formatter = FormatterRepository<TKind>.Get<T>();

            bool first = true;

            foreach ( T item in value )
            {
                if ( first )
                {
                    stringBuilder.Append( '[', ' ' );
                }
                else
                {
                    stringBuilder.Append( ',', ' ' );
                }

                first = false;

                formatter.Write( stringBuilder, item );
            }

            if ( first )
            {
                stringBuilder.Append( '[', ']' );
            }
            else
            {
                stringBuilder.Append( ' ', ']' );
            }
        }
    }
}
