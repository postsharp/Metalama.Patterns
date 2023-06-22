using PostSharp.Patterns.Contracts;
using PostSharp.Patterns.Diagnostics;
using PostSharp.Patterns.Diagnostics.Custom;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PostSharp.Patterns.Common.Tests.Diagnostics
{
    public class LogEventDataTests
    {
        [Fact]
        public void TestAnonymous()
        {
            LogEventData data = LogEventData.Create( new { Name1 = "Value", Name2 = 2 } );

            IReadOnlyDictionary<string, object> properties = data.ToDictionary();

            Assert.Equal( 2, properties.Count );
            Assert.Equal( "Value", properties["Name1"] );
            Assert.Equal( 2, properties["Name2"] );

        }

        [Fact]
        public void TestLegacy()
        {
            LoggingProperty[] inputProperties = new[] { new LoggingProperty( "Name1", "Value" ), new LoggingProperty( "Name2", 2 ) };
            LogEventData data = new LogEventData( inputProperties );

            IReadOnlyDictionary<string, object> properties = data.ToDictionary();

            Assert.Equal( 2, properties.Count );
            Assert.Equal( "Value", properties["Name1"] );
            Assert.Equal( 2, properties["Name2"] );
        }

        [Fact]
        public void TestWithExpressionModel()
        {
            LogEventData data = LogEventData.Create( new object(), new TestMetadata() );

            Assert.NotNull( data.GetExpressionModel<TestExpressionModel>() );
        }

        [Fact]
        public void TestAugmented()
        {
            LogEventData data = LogEventData.Create( new { Name1 = "Value", Name2 = 2 } ).WithAdditionalProperty( "Name3", 1.2 );

            IReadOnlyDictionary<string, object> properties = data.ToDictionary();

            Assert.Equal( 3, properties.Count );
            Assert.Equal( "Value", properties["Name1"] );
            Assert.Equal( 2, properties["Name2"] );
            Assert.Equal( 1.2, properties["Name3"] );
        }

        [Fact]
        public void TestAugmentedEmpty()
        {
            LogEventData data = default( LogEventData ).WithAdditionalProperty( "Name3", 1.2 );

            IReadOnlyDictionary<string, object> properties = data.ToDictionary();

            Assert.Equal( 1, properties.Count );
            Assert.Equal( 1.2, properties["Name3"] );
        }

        [Fact]
        public void TestAugmentedExpressionModel()
        {
            object rawData = new object();
            LogEventData data = LogEventData.Create( rawData, new TestMetadata() ).WithAdditionalProperty( "Name3", 1.2 );

            TestExpressionModel expressionModel = data.GetExpressionModel<TestExpressionModel>();
            Assert.NotNull( expressionModel );
            Assert.Same( rawData, expressionModel.Data );
        }


        [Fact]
        public void TestAnnotatedProperties()
        {
            PropertiesWithAttributes o = new PropertiesWithAttributes();
            LogEventData eventData = LogEventData.Create( o );

            // Test the dynamic factory.
            Assert.Same( eventData.Metadata, LogEventData.Create( o, null ).Metadata );

            Assert.True( eventData.Metadata.GetPropertyOptions( "Inherited" ).IsInherited );
            Assert.True( eventData.Metadata.GetPropertyOptions( "Ignored" ).IsIgnored );

            IReadOnlyDictionary<string, object> properties = eventData.ToDictionary();

            Assert.Equal( 1, properties.Count );

        }

        class TestExpressionModel
        {
            public readonly object Data;

            public TestExpressionModel( object data )
            {
                this.Data = data;
            }
        }

        private class TestMetadata : LogEventMetadata<TestExpressionModel>
        {
            public TestMetadata() : base( "Test" )
            {
            }

            public override TestExpressionModel GetExpressionModel( object data )
            {
                return new TestExpressionModel( data );
            }

        }

        private class PropertiesWithAttributes
        {
            [LoggingPropertyOptions( IsIgnored = true )]
            public string Ignored { get; set; }


            [LoggingPropertyOptions( IsInherited = true )]
            public string Inherited { get; set; }

        }
    }
}
