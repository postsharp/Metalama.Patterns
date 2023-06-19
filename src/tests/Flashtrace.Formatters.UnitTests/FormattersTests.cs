// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Xunit;
using PostSharp.Patterns.Formatters;
using PostSharp.Patterns.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;
using IFormattable = PostSharp.Patterns.Formatters.IFormattable;

namespace PostSharp.Patterns.Common.Tests.Formatters
{
    public class FormattersTests
    {
        private ITestOutputHelper logger;

        public FormattersTests( ITestOutputHelper logger )
        {
            this.logger = logger;
        }

        private string Format<T>( T value )
        {
            var result = Format<TestRole, T>( value );
            this.logger.WriteLine( "'" + value?.ToString() + "' => '" + result + "'" );
            return result;
        }

        private static string Format<TKind, T>(T value) where TKind : FormattingRole, new()
        {
            UnsafeStringBuilder stringBuilder = new UnsafeStringBuilder(1024);
            FormatterRepository<TKind>.Get<T>().Write(stringBuilder, value);
            return stringBuilder.ToString();
        }

        struct TestStruct
        {
            public override string ToString()
            {
                return "ToString";
            }
        }

        class TestStructFormatter : Formatter<TestStruct>
        {
            public override void Write( UnsafeStringBuilder stringBuilder, TestStruct value )
            {
                stringBuilder.Append( "formatter" );
            }
        }

        [Fact]
        public void StronglyTypeUnregisteredType()
        {
            _ = FormatterRepository<TestRole>.Get<TestStruct>();
        }

        [Fact]
        public void NullableUsesFormatter()
        {
            FormatterRepository<TestRole>.Register( new TestStructFormatter() );

            Assert.Equal( "formatter", this.Format<TestStruct?>( new TestStruct() ) );
        }

        [Fact]
        public void NullableNull()
        {
            FormatterRepository<TestRole>.Register( new TestStructFormatter() );

            Assert.Equal( "null", this.Format<TestStruct?>( null ) );
        }

        [Fact]
        public void Formattable()
        {
            Assert.Equal("Formattable", this.Format(new FormattableObject()));
        }

        [Fact]
        public void NullToString()
        {
            Assert.Equal( "{null}", this.Format(new NullToStringClass()));
        }


        [Fact]
        public void StronglyTypedAnonymous()
        {
            var anonymous = new { A = "a", B = 0 };
            Assert.True( anonymous.GetType().IsAnonymous() );


            Assert.Equal( "{ A = \"a\", B = 0 }", this.Format( anonymous ) );
        }

        [Fact]
        public void WeaklyTypedAnonymous()
        {
            object anonymous = new { A = "a", B = 0 };
            Assert.True( anonymous.GetType().IsAnonymous() );


            Assert.Equal( "{ A = \"a\", B = 0 }", this.Format( anonymous ) );
        }

        [Fact]
        public void Types()
        {
            Assert.Equal( "null", this.Format<Type>(null));
            Assert.Equal( "int[]", this.Format(typeof(int[])));
            Assert.Equal( "List<int>", this.Format(typeof(List<int>)));
        }

        [Fact]
        public void MethodInfo()
        {
            Assert.Equal( "null", this.Format<MethodInfo>(null));
            
            Assert.Equal( "PostSharp.Patterns.Common.Tests.Formatters.FormattersTests.SomeType.Method1()", this.Format( typeof(SomeType).GetMethod( nameof(SomeType.Method1), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );
            
            Assert.Equal( "PostSharp.Patterns.Common.Tests.Formatters.FormattersTests.SomeType.Method2(int)", this.Format( typeof(SomeType).GetMethod( nameof(SomeType.Method2), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );

            Assert.Equal( "PostSharp.Patterns.Common.Tests.Formatters.FormattersTests.SomeType.Method3(int&)", this.Format( typeof(SomeType).GetMethod( nameof(SomeType.Method3), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );
            
            Assert.Equal( "PostSharp.Patterns.Common.Tests.Formatters.FormattersTests.SomeType.Method4<T>(List<T>)", this.Format( typeof(SomeType).GetMethod( nameof(SomeType.Method4), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) ) );
        }

        [Fact]
        public void ConstructorInfo()
        {
            Assert.Equal( "null", this.Format<ConstructorInfo>(null));
            Assert.Equal( "PostSharp.Patterns.Common.Tests.Formatters.FormattersTests.SomeType.new()", this.Format( typeof(SomeType).GetConstructors( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Single( c => c.GetParameters().Length == 0 ) ) );
            Assert.Equal( "PostSharp.Patterns.Common.Tests.Formatters.FormattersTests.SomeType.new(int)", this.Format( typeof(SomeType).GetConstructors( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Single( c => c.GetParameters().Length == 1 ) ) );
            Assert.Equal( "PostSharp.Patterns.Common.Tests.Formatters.FormattersTests.SomeType.StaticConstructor()", this.Format( typeof(SomeType).GetConstructors( BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic ).Single( c => c.GetParameters().Length == 0 ) ) );
        }

        class SomeType
        {
#pragma warning disable CS0414
            private static int staticField;
            private int instanceField;
#pragma warning restore CS0414

            static SomeType()
            {
                staticField = 1;
            }
            
            public SomeType()
            {
                this.instanceField = 1;
            }

            public SomeType( int a )
            {
                this.instanceField = a;
            }
            public void Method1() { }

            public int Method2( int a ) { return a; }
            
            public void Method3( out int a ) { a = 0; }

            public void Method4<T>( List<T> l ) { }

        }

        class Marker1 : FormattingRole
        {
            public Marker1() 
            {
                
            }

            public override string Name
            {
                get { return "Marker1"; }
            }

            public override string LoggingRole
            {
                get { return "Marker1"; }
            }
        }

        class Marker2 : FormattingRole
        {
            public Marker2()
            {

            }

            public override string Name
            {
                get { return "Marker2"; }
            }

            public override string LoggingRole
            {
                get { return "Marker2"; }
            }
        }

        [Fact]
        public void DifferentMarkersAreDifferent()
        {
            FormatterRepository<Marker1>.Register( new TestStructFormatter() );

            Assert.Equal( "formatter", Format<Marker1, TestStruct>( new TestStruct() ) );
            Assert.Equal( "{ToString}", Format<Marker2, TestStruct>( new TestStruct() ) );
        }

        private class FormattableObject : IFormattable
        {
            public void Format( UnsafeStringBuilder stringBuilder, FormattingRole role )
            {
                stringBuilder.Append( "Formattable" );
            }
        }

        private class NullToStringClass
        {
            public override string ToString()
            {
                return null;
            }
        }
    }
}