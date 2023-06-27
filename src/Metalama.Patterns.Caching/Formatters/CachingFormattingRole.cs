// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Metalama.Patterns.Caching.Formatters
{
    // TODO: Move to a different namespace because this namespace has only one public type.
    
    /// <summary>
    /// The <see cref="FormattingRole"/> for <c>PostSharp.Patterns.Caching</c>.
    /// </summary>
    public sealed class CachingFormattingRole : FormattingRole
    {
        /// <summary>
        /// Initializes a new <see cref="CachingFormattingRole"/>.
        /// </summary>
        public CachingFormattingRole() 
        {
        }

        /// <inheritdoc />
        public override string Name => "Caching";

        /// <inheritdoc />
        public override string LoggingRole => LoggingRoles.Caching;
    }
}
