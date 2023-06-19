// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Globalization;
using System.Text;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Formatters;


namespace PostSharp.Patterns.Common.Tests.Formatters
{
    public class IntegerFormatterTests
    {
        [Fact]
        public void ByteTest()
        {
            byte[] values = {0, 1, 13, 42, 127, 128, 255};

            ByteFormatter formatter = ByteFormatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder(1024);

            foreach ( byte value in values )
            {
                formatter.Write( stringBuilder, value );

                Assert.Equal( value.ToString( CultureInfo.InvariantCulture ), stringBuilder.ToString() );

                stringBuilder.Clear();
            }
        }

        [Fact]
        public void SByteTest()
        {
            sbyte[] values = {0, 1, 13, 42, 127, -1, -13, -127, -128};

            SByteFormatter formatter = SByteFormatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            foreach ( sbyte value in values )
            {
                formatter.Write( stringBuilder, value );

                Assert.Equal( value.ToString( CultureInfo.InvariantCulture ), stringBuilder.ToString() );

                stringBuilder.Clear();
            }
        }

        [Fact]
        public void UInt16Test()
        {
            ushort[] values = { 0, 1, 13, 42, 127, 128, 255, 256, 10000, 32767, 32768, 65534, 65535 };

            UInt16Formatter formatter = UInt16Formatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            foreach (ushort value in values)
            {
                formatter.Write(stringBuilder, value);

                Assert.Equal(value.ToString(CultureInfo.InvariantCulture), stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }

        [Fact]
        public void Int16Test()
        {
            short[] values = { 0, 1, 13, 42, 127, 128, 255, 256, 10000, 32767, -1, -13, -127, -128, -10000, -32767, -32768 };

            Int16Formatter formatter = Int16Formatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            foreach (short value in values)
            {
                formatter.Write(stringBuilder, value);

                Assert.Equal(value.ToString(CultureInfo.InvariantCulture), stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }

        [Fact]
        public void UInt32Test()
        {
            uint[] values =
            {
                0, 1, 13, 42, 127, 128, 255, 256, 10000, 32767, 32768, 65534, 65535, 65536, 142857, 1000000, 33550336, 2147483647, 2147483648,
                4294967294, 4294967295
            };

            UInt32Formatter formatter = UInt32Formatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            foreach (uint value in values)
            {
                formatter.Write(stringBuilder, value);

                Assert.Equal(value.ToString(CultureInfo.InvariantCulture), stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }

        [Fact]
        public void Int32Test()
        {
            int[] values =
            {
                0, 1, 13, 42, 127, 128, 255, 256, 10000, 32767, 32768, 65534, 65535, 65536, 142857, 1000000, 33550336, 2147483647, -1, -13, -127, -128, -10000,
                -32767, -32768, -65536, -2147483647, -2147483648
            };

            Int32Formatter formatter = Int32Formatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            foreach (int value in values)
            {
                formatter.Write(stringBuilder, value);

                Assert.Equal(value.ToString(CultureInfo.InvariantCulture), stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }

        /*
        [Fact]
        public void Int32AllTest()
        {
            Int32Formatter formatter = Int32Formatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            for (int value = 0; value < 1000000000; value++)
            {
                formatter.Write(stringBuilder, value);

                AssertEx.Equal(value.ToString(CultureInfo.InvariantCulture), stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }
        */

        [Fact]
        public void UInt64Test()
        {
            ulong[] values =
            {
                0, 1, 13, 42, 127, 128, 255, 256, 10000, 32767, 32768, 65534, 65535, 65536, 142857, 1000000, 33550336, 2147483647, 2147483648,
                4294967294, 4294967295, 8589869056, 137438691328, 2305843008139952128, 9223372036854775807, 9223372036854775808, 18446744073709551615
            };

            UInt64Formatter formatter = UInt64Formatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            foreach (ulong value in values)
            {
                formatter.Write(stringBuilder, value);

                Assert.Equal(value.ToString(CultureInfo.InvariantCulture), stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }

        [Fact]
        public void Int64Test()
        {
            long[] values =
            {
                0, 1, 13, 42, 127, 128, 255, 256, 10000, 32767, 32768, 65534, 65535, 65536, 142857, 1000000, 33550336, 2147483647, 2147483648,
                4294967294, 4294967295, 8589869056, 137438691328, 2305843008139952128, 9223372036854775807, -1, -13, -127, -128, -10000, -32767, -32768, -65536,
                -2147483647, -2147483648, -4294967294, -2305843008139952128, -9223372036854775807, -9223372036854775808
            };

            Int64Formatter formatter = Int64Formatter.Instance;
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder();

            foreach (long value in values)
            {
                formatter.Write(stringBuilder, value);

                Assert.Equal(value.ToString(CultureInfo.InvariantCulture), stringBuilder.ToString());

                stringBuilder.Clear();
            }
        }
    }
}