// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Flashtrace.Formatters
{
    /// <summary>
    /// A formatter for <see cref="MethodBase"/> values.
    /// </summary>
    public sealed class MethodFormatter : Formatter<MethodBase>
    {
#if COMPILED_REGEX
        private const RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant;
#else
         private const RegexOptions regexOptions = RegexOptions.CultureInvariant;
#endif

        // (See #26919) We do not create a static Regex instance because it may result in the incorrect AppContext switch values when invoked via module initializer.
        // TODO: revert back to the static Regex instance when #27182 is fixed.
        private const string anonymousMethodRegexPattern = "^(<(?<parent>[^>]+)>b__[0-9A-Fa-f]+|_Lambda)";
        //private static readonly Regex anonymousMethodRegex = new Regex("^(<(?<parent>[^>]+)>b__[0-9A-Fa-f]+|_Lambda)", regexOptions);
        //private static readonly Regex localFunctionRegex = new Regex("<[^>]+>g__[0-9A-Fa-f]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly ConcurrentDictionary<MethodBase,Match> matchCache = new ConcurrentDictionary<MethodBase, Match>();

        /// <summary>
        /// The singleton instance of <see cref="MethodFormatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104")]
        public static readonly MethodFormatter Instance = new MethodFormatter();

        private MethodFormatter()
        {

        }


        /// <inheritdoc />
        public override void Write( UnsafeStringBuilder stringBuilder, MethodBase value )
        {
            if ( value == null )
            {
                stringBuilder.Append( 'n', 'u', 'l', 'l' );
                return;
            }
            
            try
            {
                string methodName = value is ConstructorInfo ? ( value.IsStatic ? "StaticConstructor" : "new") : value.Name;
                char c = methodName[0];
                if ( c == '<' || c == '_' )
                {
                    Match anonymousMethodMatch;

                    if ( !this.matchCache.TryGetValue( value, out anonymousMethodMatch ) )
                    {
                        anonymousMethodMatch = Regex.Match( methodName, anonymousMethodRegexPattern, regexOptions );
                        this.matchCache[value] = anonymousMethodMatch;
                    }

                    if ( anonymousMethodMatch.Success )
                    {
                        if ( value.DeclaringType != null )
                        {
                            if ( value.DeclaringType.Name[0] == '<' )
                            {
                                // Don't write the name of the closure class, if any.
                                TypeFormatter.Instance.Write( stringBuilder, value.DeclaringType.DeclaringType );
                            }
                            else
                            {
                                TypeFormatter.Instance.Write( stringBuilder, value.DeclaringType );
                            }

                            stringBuilder.Append( '.' );
                        }

                        Group parentGroup = anonymousMethodMatch.Groups["parent"];
                        if ( parentGroup != null )
                        {
                            stringBuilder.Append( parentGroup.Value );
                            stringBuilder.Append( ".Anonymous" );
                        }
                        else
                        {
                            stringBuilder.Append( "Anonymous" );
                        }


                        return;
                    }
                }

                TypeFormatter.Instance.Write( stringBuilder, value.DeclaringType );
                stringBuilder.Append( '.' );

                stringBuilder.Append( methodName );
                if ( value.IsGenericMethod )
                {
                    Type[] genericArguments = value.GetGenericArguments();
                    stringBuilder.Append( '<' );
                    for ( int i = 0; i < genericArguments.Length; i++ )
                    {
                        if ( i > 0 )
                        {
                            stringBuilder.Append( ',' );
                        }

                        TypeFormatter.Instance.Write( stringBuilder, genericArguments[i] );
                    }

                    stringBuilder.Append( '>' );

                }

                stringBuilder.Append( '(' );
                ParameterInfo[] parameters = value.GetParameters();
                for ( int i = 0; i < parameters.Length; i++ )
                {
                    if ( i > 0 )
                    {
                        stringBuilder.Append( ',' );
                    }

                    TypeFormatter.Instance.Write( stringBuilder, parameters[i].ParameterType );
                }

                stringBuilder.Append( ')' );
            }
            catch ( Exception )
            {
                stringBuilder.Append( value.ToString() );
            }
        }
    }
}
