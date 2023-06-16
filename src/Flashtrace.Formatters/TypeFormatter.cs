// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PostSharp.Reflection;

namespace Flashtrace.Formatters;


/// <summary>
/// A <see cref="Formatter{T}"/> for <see cref="Type"/> values.
/// </summary>
public sealed class TypeFormatter : Formatter<Type>
{
    private readonly bool includeNamespace;

    private static readonly Dictionary<Type, string> specialTypeNames = new Dictionary<Type, string>()
                                                                        {
                                                                            {typeof(string), "string"},
                                                                            {typeof(int), "int"},
                                                                            {typeof(bool), "bool"},
                                                                            {typeof(uint), "uint"},
                                                                            {typeof(long), "long"},
                                                                            {typeof(ulong), "ulong"},
                                                                            {typeof(short), "short"},
                                                                            {typeof(ushort), "ushort"},
                                                                            {typeof(char), "char"},
                                                                            {typeof(byte), "byte"},
                                                                            {typeof(sbyte), "sbyte"},
                                                                            {typeof(decimal), "decimal"},
                                                                            {typeof(object), "object"},
                                                                            {typeof(float), "float"},
                                                                            {typeof(double), "double"}
                                                                        };

    /// <summary>
    /// The default instance of <see cref="TypeFormatter"/>, which includes namespaces.
    /// </summary>
    [SuppressMessage( "Microsoft.Security", "CA2104" )]
    public static readonly TypeFormatter Instance = new TypeFormatter();

    /// <summary>
    /// Initializes a new <see cref="TypeFormatter"/> and specifies whether it should include namespaces.
    /// </summary>
    /// <param name="includeNamespace"></param>
    public TypeFormatter( bool includeNamespace = true )
    {
        this.includeNamespace = includeNamespace;
    }

    /// <inheritdoc />
    public override void Write( UnsafeStringBuilder stringBuilder, Type value )
    {
        this.WriteCore( stringBuilder, value );
    }

    private void WriteCore( UnsafeStringBuilder stringBuilder, Type type, bool removeArity = false, Type[] genericArguments = null )
    {
        try
        {
            if ( type == null )
            {
                stringBuilder.Append( 'n', 'u', 'l', 'l' );
                return;
            }

            if ( type.IsGenericParameter )
            {
                stringBuilder.Append( type.Name );
                return;
            }

            if ( type.DeclaringType != null )
            {
                // For generic inner type instance, pass the fully specified types on (for A<int>.B<int> argument array contains {int, int}).
                genericArguments ??= (type.IsGenericType() && !type.IsGenericTypeDefinition()) ? type.GetGenericArguments() : null;
                this.WriteCore( stringBuilder, type.DeclaringType, genericArguments: genericArguments );
                stringBuilder.Append( '.' );
            }

            if ( type.IsGenericType() && type.GetGenericTypeDefinition() == typeof(Nullable<>) )
            {
                this.Write( stringBuilder, type.GetGenericArguments()[0] );
                stringBuilder.Append( '?' );
            }
            else if ( type.IsArray )
            {
                this.Write( stringBuilder, type.GetElementType() );
                stringBuilder.Append( '[' );
                for ( int i = 1; i < type.GetArrayRank(); i++ )
                {
                    stringBuilder.Append( ',' );
                }

                stringBuilder.Append( ']' );
            }
            else if ( type.IsGenericType() && (!type.IsGenericTypeDefinition() || genericArguments != null) )
            {
                // If generic arguments are set, formatting was started on type instance and the passed array should be used as it contains actual arguments.
                // Otherwise formatting runs on type definition, where we use the local array.
                genericArguments ??= type.GetGenericArguments();
                Type[] genericParameters = type.GetGenericArguments();
                bool appendComma = false;

                stringBuilder.Append( RemoveArity( this.GetTypeName( type ), true ) );
                stringBuilder.Append( '<' );

                int startIndex = type.DeclaringType?.GetGenericArguments()?.Length ?? 0;
                for ( int i = startIndex; i < genericParameters.Length; i++ )
                {
                    if ( appendComma )
                    {
                        stringBuilder.Append( ',' );
                    }
                    else
                    {
                        appendComma = true;
                    }

                    this.Write( stringBuilder, genericArguments[i] );
                }

                stringBuilder.Append( '>' );
            }
            else if ( type.IsGenericTypeDefinition() )
            {
                stringBuilder.Append( RemoveArity( this.GetTypeName( type ), true ) );
                if ( !removeArity )
                {
                    stringBuilder.Append( '<' );

                    // Only include number of parameters declared on this type (array contains parameters of declaring types).
                    int startIndex = (type.DeclaringType?.GetGenericArguments()?.Length ?? 0) + 1;
                    int length = type.GetGenericArguments().Length;
                    for ( int i = startIndex; i < length; i++ )
                    {
                        stringBuilder.Append( ',' );
                    }

                    stringBuilder.Append( '>' );
                }
            }
            else if ( type.IsByRef )
            {
                this.WriteCore( stringBuilder, type.GetElementType(), removeArity );
                stringBuilder.Append( "&" );
            }
            else if ( type.IsPointer )
            {
                this.WriteCore( stringBuilder, type.GetElementType(), removeArity );
                stringBuilder.Append( "*" );
            }
            else if ( !type.HasElementType )
            {
                stringBuilder.Append( RemoveArity( this.GetTypeName( type ), removeArity ) );
            }
            else
            {
                stringBuilder.Append( type.ToString() );
            }
        }
        catch ( Exception )
        {
            // Decent handling of exception in case we have a bug.
            stringBuilder.Append( "!!" );
            stringBuilder.Append( type.ToString() );
        }
    }

    private string GetTypeName( Type type )
    {
        if ( specialTypeNames.TryGetValue( type, out string typeName ) )
        {
            return typeName;
        }

        if ( type.DeclaringType != null )
        {
            return type.Name;
        }

        return this.IsTrivialNamespace( type ) ? type.Name : type.FullName;
    }

    private bool IsTrivialNamespace( Type type )
    {
        if ( this.includeNamespace )
        {
            switch ( type.Namespace )
            {
                case "System":
                case "System.Text":
                case "System.Collections":
                case "System.Collections.Generic":
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            return true;
        }
    }

    private static string RemoveArity( string typeName, bool condition )
    {
        if ( condition )
        {
            int indexOfQuote = typeName.LastIndexOf( '`' );
            if ( indexOfQuote > 0 )
            {
                return typeName.Substring( 0, indexOfQuote );
            }
        }

        return typeName;
    }
}