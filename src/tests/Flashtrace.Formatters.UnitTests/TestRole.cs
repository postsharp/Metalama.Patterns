using PostSharp.Patterns.Formatters;

namespace PostSharp.Patterns.Common.Tests.Formatters
{
    sealed class TestRole : FormattingRole
    {
        public TestRole() 
        {

        }

        public override string Name
        {
            get { return "TestRole"; }
        }

        public override string LoggingRole
        {
            get { return this.Name; }
        }
    }
}