using Flashtrace.Formatters;
using PostSharp.Patterns.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// The formatted used to for anonymous types by default.
    /// </summary>
    /// <typeparam name="TKind">The formatting role.</typeparam>
    public sealed class AnonymousTypeFormatter<TKind> : IFormatter
          where TKind : FormattingRole, new()
    {
        private readonly Func<object, UnknownObjectAccessor> accessorFactory;

        /// <summary>
        /// Initializes a new <see cref="AnonymousTypeFormatter{TKind}"/> for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">A type.</param>
        public AnonymousTypeFormatter( Type type )
        {
            if ( type == null )
            {
                throw new ArgumentNullException( nameof(type) );
            }

            this.accessorFactory = UnknownObjectAccessor.GetFactory( type );
        }

        /// <inheritdoc/>
        public FormatterAttributes Attributes => FormatterAttributes.Normal;

        /// <inheritdoc/>
        public void Write( UnsafeStringBuilder stringBuilder, object value )
        {
            UnknownObjectAccessor accessor = this.accessorFactory( value );

            stringBuilder.Append( '{', ' ' );

            int i = 0;
            foreach ( KeyValuePair<string, object> property in accessor )
            {
                if ( i > 0 )
                {
                    stringBuilder.Append( ',', ' ' );
                }

                if ( property.Value != null )
                {
                    stringBuilder.Append( property.Key );
                    stringBuilder.Append( ' ', '=', ' ' );

                    FormatterRepository<TKind>.Get( property.Value.GetType() ).Write( stringBuilder, property.Value );
                }

                i++;

            }

            stringBuilder.Append( ' ', '}' );
        }
    }
}
