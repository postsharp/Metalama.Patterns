// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Reflection;

namespace Flashtrace.Custom
{
    internal sealed class DefaultLogEventMetadata<T> : LogEventMetadata<T>
    {
        private readonly Dictionary<string, LoggingPropertyOptions> propertyOptions 
            = new Dictionary<string, LoggingPropertyOptions>(StringComparer.OrdinalIgnoreCase);
        private readonly bool hasInheritedProperty;

        public static readonly DefaultLogEventMetadata<T> Instance = new DefaultLogEventMetadata<T>();

        private DefaultLogEventMetadata() : base(typeof(T).Name)
        {
            if ( !typeof( T ).IsAnonymous() )
            {
                foreach ( PropertyInfo property in UnknownObjectAccessor.GetProperties( typeof( T ) ) )
                {
                    LoggingPropertyOptionsAttribute attribute = property.GetCustomAttribute<LoggingPropertyOptionsAttribute>();
                    if ( attribute != null )
                    {
                        LoggingPropertyOptions options = attribute.ToOptions();
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

        internal protected override LoggingPropertyOptions GetPropertyOptions( string name )
        {
            this.propertyOptions.TryGetValue( name, out LoggingPropertyOptions options );
            return options;
        }


    }



}


