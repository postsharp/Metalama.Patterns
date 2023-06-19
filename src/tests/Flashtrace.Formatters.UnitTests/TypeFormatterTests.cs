using System;
using System.Collections.Generic;
using System.Text;
using PostSharp.Patterns.Common.Tests.Helpers;
using Xunit;
using PostSharp.Patterns.Formatters;
using System.Collections;
using System.IO;


namespace PostSharp.Patterns.Common.Tests.Formatters
{
    public class TypeFormatterTests
    {
        private void TestFormatter( Type type, string expectedString )
        {
            
            UnsafeStringBuilder sb = new UnsafeStringBuilder();
            TypeFormatter.Instance.Write(sb, type);
            Assert.Equal(expectedString, sb.ToString());
        }

        [Fact]
        public void TestPrimitives()
        {
            this.TestFormatter(typeof(string), "string");
            this.TestFormatter(typeof(decimal), "decimal");
            
        }

        [Fact]
        public void TestArray()
        {
            this.TestFormatter(typeof(DateTime[]), "DateTime[]");
            this.TestFormatter(typeof(string[]), "string[]");
            this.TestFormatter(typeof(string[,]), "string[,]");
            this.TestFormatter(typeof(string[,,]), "string[,,]");
            this.TestFormatter(typeof(int?[]), "int?[]");
        }

        [Fact]
        public void TestNullable()
        {
            this.TestFormatter(typeof(int?), "int?");
            this.TestFormatter(typeof(DateTime?), "DateTime?");
        }

        [Fact]
        public void TestSystemNamespace()
        {
            this.TestFormatter(typeof(DateTime), "DateTime");
            this.TestFormatter(typeof(IEnumerable), "IEnumerable");
            this.TestFormatter(typeof(Stream), "System.IO.Stream");
        }

        [Fact]
        public void TestGenerics()
        {
            this.TestFormatter(typeof(IEquatable<>), "IEquatable<>");
            this.TestFormatter(typeof(IEquatable<DateTime>), "IEquatable<DateTime>");
            this.TestFormatter(typeof(IDictionary<,>), "IDictionary<,>");
            this.TestFormatter(typeof(IEquatable<Tuple<int>>), "IEquatable<Tuple<int>>");
            this.TestFormatter(typeof(Tuple<,>), "Tuple<,>");
        }

        [Fact]
        public void TestInnerTypes()
        {
            this.TestFormatter( typeof( TestType.InnerType ), "TestType.InnerType" );
            this.TestFormatter( typeof( IEnumerable<TestType.InnerType> ), "IEnumerable<TestType.InnerType>" );
            this.TestFormatter( typeof( TestType.InnerType.MoreInnerType ), "TestType.InnerType.MoreInnerType" );
            this.TestFormatter( typeof( IEnumerable<TestType.InnerType.MoreInnerType> ), "IEnumerable<TestType.InnerType.MoreInnerType>" );
            this.TestFormatter( typeof( TestType.InnerType.MoreInnerType.EvenMoreInnerType ), "TestType.InnerType.MoreInnerType.EvenMoreInnerType" );
            this.TestFormatter( typeof( IEnumerable<TestType.InnerType.MoreInnerType.EvenMoreInnerType> ), "IEnumerable<TestType.InnerType.MoreInnerType.EvenMoreInnerType>" );
        }

        [Fact]
        public void TestGenericInnerTypes()
        {
            this.TestFormatter( 
                typeof( TestType.InnerType<,> ), 
                "TestType.InnerType<,>" );

            this.TestFormatter( 
                typeof( TestType.InnerType<,>.MoreInnerType<,> ),
                "TestType.InnerType<,>.MoreInnerType<,>" );

            this.TestFormatter( 
                typeof( TestType.InnerType<,>.MoreInnerType<,>.EvenMoreInnerType<,> ),
                "TestType.InnerType<,>.MoreInnerType<,>.EvenMoreInnerType<,>" );

            this.TestFormatter( 
                typeof( TestType.InnerType<TestType1, TestType2> ),
                "TestType.InnerType<TestType1,TestType2>" );

            this.TestFormatter(
                typeof( IEnumerable<TestType.InnerType<TestType1, TestType2>> ),
                "IEnumerable<TestType.InnerType<TestType1,TestType2>>" );

            this.TestFormatter( 
                typeof( TestType.InnerType<TestType1, TestType2>.MoreInnerType<TestType3, TestType4> ),
                "TestType.InnerType<TestType1,TestType2>.MoreInnerType<TestType3,TestType4>" );

            this.TestFormatter( 
                typeof( IEnumerable<TestType.InnerType<TestType1, TestType2>.MoreInnerType<TestType3, TestType4>> ),
                "IEnumerable<TestType.InnerType<TestType1,TestType2>.MoreInnerType<TestType3,TestType4>>" );

            this.TestFormatter(                 
                typeof( TestType.InnerType<TestType1, TestType2>.MoreInnerType<TestType3, TestType4>.EvenMoreInnerType<TestType5, TestType6> ),
                "TestType.InnerType<TestType1,TestType2>.MoreInnerType<TestType3,TestType4>.EvenMoreInnerType<TestType5,TestType6>" );

            this.TestFormatter( 
                typeof( IEnumerable<TestType.InnerType<TestType1, TestType2>.MoreInnerType<TestType3, TestType4>.EvenMoreInnerType<TestType5, TestType6>> ),
                "IEnumerable<TestType.InnerType<TestType1,TestType2>.MoreInnerType<TestType3,TestType4>.EvenMoreInnerType<TestType5,TestType6>>" );
        }


    }
}

namespace System
{
    public class TestType
    {
        public class InnerType
        {
            public class MoreInnerType
            {
                public class EvenMoreInnerType
                {
                }
            }
        }

        public class InnerType<T1, T2>
        {
            public class MoreInnerType<U1, U2>
            {
                public class EvenMoreInnerType<V1, V2>
                {
                }
            }
        }
    }

    public class TestType1
    {
    }
    public class TestType2
    {
    }
    public class TestType3
    {
    }
    public class TestType4
    {
    }
    public class TestType5
    {
    }
    public class TestType6
    {
    }
}
