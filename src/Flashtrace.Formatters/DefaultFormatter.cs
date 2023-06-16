// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Concurrent;
using System.Reflection;

namespace Flashtrace.Formatters;

// Must be a separate class because we want to share among different generic types.
internal static class DefaultFormatterHelper
{
    private static readonly ConcurrentDictionary<Type, bool> _hasCustomToStringMethod
        = new ConcurrentDictionary<Type, bool>();

    public static bool HasCustomToStringMethod( Type type )
    {
        return _hasCustomToStringMethod.GetOrAdd( type, t =>
            t.GetMethod( "ToString", BindingFlags.Public | BindingFlags.Instance )?.DeclaringType != typeof( object ) );
    }
}

/// <summary>
/// The default formatter that formats objects by calling <see cref="object.ToString"/>.
/// </summary>
public sealed class DefaultFormatter<TValue> : Formatter<TValue>
{
    private static readonly bool _isValueType = typeof(TValue).IsValueType;

    private static readonly bool _hasCustomToStringMethod = DefaultFormatterHelper.HasCustomToStringMethod(typeof(TValue));

    private static readonly Type _valueType = typeof(TValue);
    
    private readonly FormattingOptions _options;

    private readonly DefaultFormatter<TValue> _unquotedFormatter;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultFormatter{TValue}"/> class.
    /// </summary>
    public DefaultFormatter( IFormatterRepository repository ) : this( repository, FormattingOptions.Default )
    {
    }

    private DefaultFormatter( IFormatterRepository repository, FormattingOptions options )
        : base( repository )
    {
        this._options = options;
        this._unquotedFormatter = new DefaultFormatter<TValue>( repository, FormattingOptions.Unquoted );
    }

    /// <inheritdoc/>
    public override FormatterAttributes Attributes => FormatterAttributes.Default;

    /// <inheritdoc />
    public override IOptionAwareFormatter WithOptions( FormattingOptions options )
    {
        if ( options.RequiresUnquotedStrings )
        {
            return this._unquotedFormatter;
        }
        else
        {
            return this;
        }
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, TValue value )
    {
        bool useToString = _hasCustomToStringMethod;
        Type thisValueType = _valueType;

        if ( _isValueType )
        {
            var formatter = this.Repository.Get<TValue>().WithOptions( this._options );

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
        
            var formatter = this.Repository.Get( value.GetType() ).WithOptions( this._options );

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

            bool braces = !value.GetType().IsPrimitive;

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
            this.Repository.Get<Type>().Write( stringBuilder, thisValueType );
            stringBuilder.Append( '}' );
        }
    }
}