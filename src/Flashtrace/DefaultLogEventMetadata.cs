﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using System.Reflection;

namespace Flashtrace
{
    internal sealed class DefaultLogEventMetadata<T> : LogEventMetadata<T>
    {
        private readonly Dictionary<string, LoggingPropertyOptions> propertyOptions = new( StringComparer.OrdinalIgnoreCase );
        private readonly bool hasInheritedProperty;

        public static readonly DefaultLogEventMetadata<T> Instance = new();

        private DefaultLogEventMetadata() : base( typeof(T).Name )
        {
            if ( !typeof(T).IsAnonymous() )
            {
                foreach ( var property in UnknownObjectAccessor.GetProperties( typeof(T) ) )
                {
                    var attribute = property.GetCustomAttribute<LoggingPropertyOptionsAttribute>();

                    if ( attribute != null )
                    {
                        var options = attribute.ToOptions();
                        this.propertyOptions.Add( property.Name, options );

                        if ( options.IsInherited )
                        {
                            this.hasInheritedProperty = true;
                        }
                    }
                }
            }
        }

        public override bool HasInheritedProperty( object data )
        {
            // This implementation is a simplification because it would return true even if all
            // inherited properties are null. This should not matter because HasInheritedProperty is called to implement
            // an optimization.

            return this.hasInheritedProperty;
        }

        protected internal override LoggingPropertyOptions GetPropertyOptions( string name )
        {
            this.propertyOptions.TryGetValue( name, out var options );

            return options;
        }
    }
}