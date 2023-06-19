// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

#if APP_DOMAIN

using System;
using System.Collections.Generic;
using System.Text;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Formatters;


namespace PostSharp.Patterns.Common.Tests.Formatters
{
    public class FormatterNoLoggingExceptionTests : AppDomainTestsBaseClass<FormatterNoLoggingExceptionTests>
    {
        private static string Format<T>( T value )
        {
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder(1024);
            FormatterRepository<TestRole>.Get<T>().Write( stringBuilder, value );
            return stringBuilder.ToString();
        }

        [Fact]
        public void ThrowingConstructor()
        {
            ExecuteTest( tc => tc.ThrowingConstructorMethod() );
        }

        private void ThrowingConstructorMethod()
        {
            FormatterRepository<TestRole>.Register( typeof(IEnumerable<>), typeof(ThrowingFormatter<>) );

            string result = Format<IEnumerable<int>>( new int[0] );

            Assert.True( ThrowingFormatter<int>.Ran );
            Assert.Equal( "{int[]}", result );
        }

        [Fact]
        public void PrivateConstructor()
        {
            ExecuteTest( tc => tc.PrivateConstructorMethod() );
        }

        private void PrivateConstructorMethod()
        {
            FormatterRepository<TestRole>.Register( typeof(IEnumerable<>), typeof(NoConstructorFormatter<>) );

            string result = Format<IEnumerable<int>>( new int[0] );

            Assert.Equal( "{int[]}", result );
        }

        [Fact]
        public void BadRegistration()
        {
            ExecuteTest( tc => tc.BadRegistrationMethod() );
        }

        private void BadRegistrationMethod()
        {
            FormatterRepository<TestRole>.Register( typeof(IComparable<>), typeof(ThrowingFormatter<>) );

            string result = Format<IComparable<int>>( 0 );

            Assert.True( ThrowingFormatter<int>.Ran );
            Assert.Equal( "0", result );
        }
    }

    internal class ThrowingFormatter<T> : Formatter<IEnumerable<T>>
    {
        public static bool Ran;

        public ThrowingFormatter()
        {
            Ran = true;
            throw new Exception();
        }

        public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
        {
            throw new NotSupportedException();
        }
    }

    internal class NoConstructorFormatter<T> : Formatter<IEnumerable<T>>
    {
        private NoConstructorFormatter()
        {
        }

        public override void Write( UnsafeStringBuilder stringBuilder, IEnumerable<T> value )
        {
            throw new NotSupportedException();
        }
    }
}

#endif