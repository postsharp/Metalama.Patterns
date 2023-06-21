// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace.Custom
{
    internal class LogEventPropertiesMetadata
        : LogEventMetadata<LoggingPropertiesExpressionModel>
    {
        internal static readonly LogEventPropertiesMetadata Instance = new();

        public LogEventPropertiesMetadata() : base( "Properties" ) { }

        public override bool HasInheritedProperty( object data )
        {
            IReadOnlyList<LoggingProperty> properties = (IReadOnlyList<LoggingProperty>) data;

            if ( data == null )
            {
                return false;
            }

            foreach ( var property in properties )
            {
                if ( property.IsInherited )
                {
                    return true;
                }
            }

            return false;
        }

        public override void VisitProperties<TVisitorState>(
            object data,
            ILoggingPropertyVisitor<TVisitorState> visitor,
            ref TVisitorState visitorState,
            in LoggingPropertyVisitorOptions visitorOptions = default )
        {
            IReadOnlyList<LoggingProperty> properties = (IReadOnlyList<LoggingProperty>) data;

            if ( data == null )
            {
                return;
            }

            foreach ( var property in properties )
            {
                if ( visitorOptions.OnlyInherited && !property.IsInherited )
                {
                    continue;
                }

                visitor.Visit( property.Name, property.Value, property.Options, ref visitorState );
            }
        }

        public override LoggingPropertiesExpressionModel GetExpressionModel( object data ) => new( (IReadOnlyList<LoggingProperty>) data );
    }
}