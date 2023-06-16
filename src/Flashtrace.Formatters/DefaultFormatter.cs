// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Text;
using PostSharp.Reflection;
using PostSharp.Patterns.Formatters;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Collections.Concurrent;

namespace PostSharp.Patterns.Formatters
{
    // Must be a separate class because we want to share among different generic types.
    internal static class DefaultFormatterHelper
    {
        private static readonly ConcurrentDictionary<Type, bool> hasCustomToStringMethod
            = new ConcurrentDictionary<Type, bool>();

        public static bool HasCustomToStringMethod(Type type)
        {
            return hasCustomToStringMethod.GetOrAdd( type, t =>
                t.GetMethod( "ToString", Type.EmptyTypes, BindingFlags.Public | BindingFlags.Instance )?.DeclaringType != typeof( object )
            );
        }
    }
   
    /// <summary>
    /// The default formatter that formats objects by calling <see cref="object.ToString"/>.
    /// </summary>
    public sealed class DefaultFormatter<TRole,TValue> : Formatter<TValue> where TRole : FormattingRole, new()
    {
        private static readonly bool isValueType = typeof(TValue).IsValueType();

        private static readonly bool hasCustomToStringMethod = DefaultFormatterHelper.HasCustomToStringMethod(typeof(TValue));

        private static readonly Type valueType = typeof(TValue);

        
        private readonly FormattingOptions options;
        static readonly DefaultFormatter<TRole, TValue> unquotedFormatter = new DefaultFormatter<TRole, TValue>( FormattingOptions.Unquoted );

        /// <summary>
        /// Gets the default instance of the <see cref="DefaultFormatter{TRole, TValue}"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000")]
        public static DefaultFormatter<TRole, TValue> Instance { get; } = new DefaultFormatter<TRole, TValue>();

        /// <summary>
        /// Initializes a new <see cref="DefaultFormatter{TRole, TValue}"/>.
        /// </summary>
        public DefaultFormatter() : this(FormattingOptions.Default)
        {
        }

        private DefaultFormatter( FormattingOptions options )
        {
            this.options = options;


        }

        /// <inheritdoc/>
        public override FormatterAttributes Attributes => FormatterAttributes.Default;

        /// <inheritdoc />
        public override IOptionAwareFormatter WithOptions( FormattingOptions options )
        {
            if ( options.RequiresUnquotedStrings )
            {
                return unquotedFormatter;
            }
            else
            {
                return this;
            }
        }


        /// <inheritdoc />
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void Write( UnsafeStringBuilder stringBuilder, TValue value )
        {
            bool useToString = hasCustomToStringMethod;
            Type thisValueType = valueType;

            if ( isValueType )
            {
                IFormatter<TValue> formatter = FormatterRepository<TRole>.Get<TValue>().WithOptions( this.options );

                if ( (formatter.Attributes & FormatterAttributes.Default) == 0 )
                {
                    formatter.Write( stringBuilder, value );
                    return;
                }
            }
            else
            {
                // ReSharper disable HeapView.PossibleBoxingAllocation

                if ( value == null )
                {
                    stringBuilder.Append( 'n', 'u', 'l', 'l' );
                    return;
                }

            
                IFormatter formatter = FormatterRepository<TRole>.Get( value.GetType() ).WithOptions( this.options );

                if ( (formatter.Attributes & FormatterAttributes.Default) == 0 )
                {
                    formatter.Write( stringBuilder, value );
                    return;
                }

                thisValueType = value.GetType();
                if ( thisValueType != typeof( TValue ) )
                {
                    // We get here because the caller called the static Get<T> and we have a value of the derived type,
                    // not the exact type. 
                    useToString = DefaultFormatterHelper.HasCustomToStringMethod( thisValueType );
                 
                }

                // ReSharper restore HeapView.PossibleBoxingAllocation

            }

            if ( useToString )
            {
                string text;
                try
                {
                    text = value.ToString();
                }
                catch ( Exception e )
                {
                    text = e.GetType().ToString();
                }

                bool braces = !value.GetType().IsPrimitive();

                if ( braces )
                {
                    stringBuilder.Append( '{' );
                }

                // ToString can return null.
                if ( text == null )
                {
                    stringBuilder.Append( 'n', 'u', 'l', 'l' );
                }
                else
                {
                    stringBuilder.Append( text );
                }

                if ( braces )
                {
                    stringBuilder.Append( '}' );
                }
            }
            else
            {
                stringBuilder.Append( '{' );
                FormatterRepository<TRole>.Get<Type>(  ).Write( stringBuilder, thisValueType );
                stringBuilder.Append( '}' );
            }
        }
    }
}