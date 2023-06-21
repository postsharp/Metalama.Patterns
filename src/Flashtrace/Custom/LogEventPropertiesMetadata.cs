// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections.Generic;

namespace PostSharp.Patterns.Diagnostics.Custom
{
    internal class LogEventPropertiesMetadata
        : LogEventMetadata<LoggingPropertiesExpressionModel>
    {
        internal static readonly LogEventPropertiesMetadata Instance = new LogEventPropertiesMetadata();

        public LogEventPropertiesMetadata() : base( "Properties" )
        {
        }

        public override bool HasInheritedProperty( object data )
        {
            IReadOnlyList<LoggingProperty> properties = (IReadOnlyList<LoggingProperty>) data;
            if ( data == null )
            {
                return false;
            }

            foreach ( LoggingProperty property in properties )
            {
                if ( property.IsInherited )
                    return true;
            }

            return false;
        }

        public override void VisitProperties<TVisitorState>( object data, ILoggingPropertyVisitor<TVisitorState> visitor, ref TVisitorState visitorState, in LoggingPropertyVisitorOptions visitorOptions = default )
        {
            IReadOnlyList<LoggingProperty> properties = (IReadOnlyList<LoggingProperty>) data;
            if ( data == null )
            {
                return;
            }

            foreach ( LoggingProperty property in properties )
            {
                if ( visitorOptions.OnlyInherited && !property.IsInherited )
                    continue;

                visitor.Visit( property.Name, property.Value, property.Options, ref visitorState );
            }
        }

        public override LoggingPropertiesExpressionModel GetExpressionModel( object data ) => new LoggingPropertiesExpressionModel( (IReadOnlyList<LoggingProperty>) data );


    }

}


