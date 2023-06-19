// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Formatters;


namespace PostSharp.Patterns.Common.Tests.Formatters
{
    public class EnumFormatterTests
    {
        [Fact]
        public void FlagsEnumEqualsToStringTest()
        {
            FlagsEnum[] values = {0, FlagsEnum.A, FlagsEnum.B, FlagsEnum.A | FlagsEnum.B, (FlagsEnum) int.MaxValue};

            foreach ( FlagsEnum value in values )
            {
                Assert.Equal( value.ToString(), EnumFormatter<FlagsEnum>.GetString( value ) );
            }
        }

        [Flags]
        enum FlagsEnum
        {
            A = 1,
            B = 2
        }

        [Fact]
        public void SimpleEnumEqualsToStringTest()
        {
            SimpleEnum[] values = {0, SimpleEnum.A, SimpleEnum.B, SimpleEnum.A | SimpleEnum.B, (SimpleEnum) ulong.MaxValue};

            foreach ( SimpleEnum value in values )
            {
                Assert.Equal( value.ToString(), EnumFormatter<SimpleEnum>.GetString( value ) );
            }
        }

        enum SimpleEnum : ulong
        {
            A = 1,
            B = 2
        }
    }
}