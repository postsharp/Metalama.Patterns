// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Formatters;

namespace Metalama.Patterns.Caching.Formatters
{
    internal class CollectionFormatter<T> : Formatter<IEnumerable<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionFormatter{T}"/> class using the specified <see cref="IFormatterRepository"/>
        /// to access formatters for other types.
        /// </summary>
        /// <param name="repository"></param>
        public CollectionFormatter( IFormatterRepository repository ) : base( repository )
        {
        }

        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
        {
            if ( value == null )
            {
                stringBuilder.Append( 'n', 'u', 'l', 'l' );
                return;
            }

            IFormatter<T> formatter = this.Repository.Get<T>();

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
