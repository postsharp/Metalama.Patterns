// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Flashtrace.Custom
{
    internal static class AugmentedLogEventData
    {
        private static readonly ConcurrentDictionary<(Type, Type), object> delegateCache = new();

        public static LogEventData Augment<T>( in LogEventData augmented, string propertyName, T propertyValue, in LoggingPropertyOptions options )
        {
            var augmentedType = augmented.Metadata?.ExpressionModelType ?? typeof(object);

            CreateMetaDataDelegate<T> factory = (CreateMetaDataDelegate<T>) delegateCache.GetOrAdd(
                (typeof(T), augmentedType),
                t => CreateFactory<T>( t.Item2 ) );

            var instance = factory( augmented, propertyName, propertyValue, options );

            return LogEventData.Create( instance, instance );
        }

        private static CreateMetaDataDelegate<TValue> CreateFactory<TValue>( Type augmentedType )
        {
            var augmentedParameter = Expression.Parameter( typeof(LogEventData).MakeByRefType() );
            var propertyNameParameter = Expression.Parameter( typeof(string) );
            var propertyValueParameter = Expression.Parameter( typeof(TValue) );
            var optionsParameter = Expression.Parameter( typeof(LoggingPropertyOptions).MakeByRefType() );

            var genericType = typeof(MetaData<,>).MakeGenericType( typeof(TValue), augmentedType );

            Expression newExpression = Expression.New(
                genericType.GetConstructors( BindingFlags.Instance | BindingFlags.Public )[0],
                augmentedParameter,
                propertyNameParameter,
                propertyValueParameter,
                optionsParameter );

            Expression<CreateMetaDataDelegate<TValue>> lambda = Expression.Lambda<CreateMetaDataDelegate<TValue>>(
                newExpression,
                augmentedParameter,
                propertyNameParameter,
                propertyValueParameter,
                optionsParameter );

            return lambda.Compile();
        }

        private delegate LogEventMetadata CreateMetaDataDelegate<TValue>(
            in LogEventData augmented,
            string propertyName,
            TValue propertyValue,
            in LoggingPropertyOptions propertyOptions );

        // It's beneficial to have the data and metadata in the same object because we need an instance of both anyway.
        private class MetaData<TValue, TAugmented> : LogEventMetadata<TAugmented>
        {
            public LogEventData Augmented;

            public string PropertyName { get; }

            public TValue PropertyValue { get; }

            public LoggingPropertyOptions PropertyOptions { get; }

            public MetaData( in LogEventData augmented, string propertyName, TValue propertyValue, in LoggingPropertyOptions propertyOptions ) :
                base( augmented.Metadata?.Name )
            {
                this.Augmented = augmented;
                this.PropertyName = propertyName;
                this.PropertyValue = propertyValue;
                this.PropertyOptions = propertyOptions;
            }

            public override void VisitProperties<TVisitorState>(
                object data,
                ILoggingPropertyVisitor<TVisitorState> visitor,
                ref TVisitorState visitorState,
                in LoggingPropertyVisitorOptions visitorOptions = default )
            {
                if ( !visitorOptions.OnlyInherited || this.PropertyOptions.IsInherited )
                {
                    visitor.Visit( this.PropertyName, this.PropertyValue, this.PropertyOptions, ref visitorState );
                }

                this.Augmented.VisitProperties( visitor, ref visitorState, visitorOptions );
            }

            public override bool HasInheritedProperty( object data )
            {
                if ( !ReferenceEquals( data, this ) )
                {
                    throw new FlashtraceAssertionFailedException();
                }

                if ( this.PropertyOptions.IsInherited )
                {
                    return true;
                }

                if ( this.Augmented.Data != null )
                {
                    return this.Augmented.Metadata.HasInheritedProperty( this.Augmented.Data );
                }

                return false;
            }

            public override TAugmented GetExpressionModel( object data )
            {
                if ( !ReferenceEquals( data, this ) )
                {
                    throw new FlashtraceAssertionFailedException();
                }

                return this.Augmented.GetExpressionModel<TAugmented>();
            }
        }
    }
}