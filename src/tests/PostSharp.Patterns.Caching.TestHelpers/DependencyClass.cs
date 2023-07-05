using System.Text;
using PostSharp.Patterns.Formatters;

namespace PostSharp.Patterns.Caching.TestHelpers
{
    public class DependencyClass : IFormattable
    {
        private readonly string key;

        public DependencyClass( string key )
        {
            this.key = key;
        }


        void IFormattable.Format( UnsafeStringBuilder stringBuilder, FormattingRole role )
        {
            stringBuilder.Append( this.key );
        }
    }
}
