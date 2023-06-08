using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Contracts;

namespace PostSharp.Patterns.Contracts.Tests
{
    public class PrecisionTests : RangeContractTestsBase
    {
        [Fact]
        public void CheckDoubleTolerance()
        {
            Assert.Equal( DoubleTolerance.ToString( CultureInfo.InvariantCulture ), FloatingPointHelper.DoubleTolerance.ToString( CultureInfo.InvariantCulture ) );
        }

        [Fact]
        public void CheckDecimalTolerance()
        {
            Assert.Equal( DecimalTolerance, FloatingPointHelper.DecimalTolerance );
        }
    }
}
