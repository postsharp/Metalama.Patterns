// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Formatters;
using Metalama.Patterns.Caching.Formatters;

namespace Metalama.Patterns.Caching;

/// <summary>
/// Allows to get and register formatters used to generate caching keys.
/// </summary>
internal sealed class CachingFormatterRepository : FormatterRepository
{
    internal CachingFormatterRepository() : base( CachingFormattingRole.Instance )
    {
        this.Register( typeof(IEnumerable<>), typeof(CollectionFormatter<>) );
    }
}