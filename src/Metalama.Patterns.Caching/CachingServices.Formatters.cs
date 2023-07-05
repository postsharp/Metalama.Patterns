// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using JetBrains.Annotations;
using Metalama.Patterns.Caching.Formatters;

namespace Metalama.Patterns.Caching;

public partial class CachingServices
{
    /// <summary>
    /// Allows to get and register formatters used to generate caching keys.
    /// </summary>
    [PublicAPI] // TODO: [Porting] Does CachingServices.Formatters need to be public?
    public sealed class Formatters : FormatterRepository
    {
        public static Formatters Instance { get; } = new( CachingFormattingRole.Instance );

        private Formatters( FormattingRole role )
            : base( role )
        {
            this.Register( typeof(IEnumerable<>), typeof(CollectionFormatter<>) );
        }
    }
}