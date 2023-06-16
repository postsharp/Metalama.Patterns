// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.Collections.Concurrent;
using System.Text;

namespace PostSharp.Patterns.Formatters
{
    internal sealed class FormatterConverter<TargetType, SourceType> : FormatterConverter<TargetType>
    {
        
        private readonly IFormatter wrapped;

        public FormatterConverter( IFormatter wrapped ) : base(wrapped)
        {
            this.wrapped = wrapped;
        }

        
    }

    internal class FormatterConverter<TargetType> : Formatter<TargetType>
    {
        private readonly IFormatter wrapped;
        private static readonly ConcurrentDictionary<IFormatter, FormatterConverter<TargetType>> cache = new ConcurrentDictionary<IFormatter, FormatterConverter<TargetType>>();

        public FormatterConverter( IFormatter wrapped )
        {
            this.wrapped = wrapped;
        }

        public static IFormatter<TargetType> Convert( IFormatter formatter ) => 
            formatter == null ? null : formatter as IFormatter<TargetType> ?? cache.GetOrAdd( formatter, f => new FormatterConverter<TargetType>( f ) );

        public override void Write( UnsafeStringBuilder stringBuilder, TargetType value )
        {
            this.wrapped.Write( stringBuilder, value );
        }

        public override FormatterAttributes Attributes => this.wrapped.Attributes | FormatterAttributes.Converter;
    }
}
