// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using PostSharp.Patterns.Diagnostics;

namespace PostSharp.Patterns.Formatters
{
    /// <summary>
    /// Base for kind marker types for <see cref="FormatterRepository{TRole}"/>.
    /// </summary>
    /// <remarks>
    /// Types derived from this type are not meant to have members or instances,
    /// they're used only as markers.
    /// </remarks>
    public abstract class FormattingRole
    {
        /// <summary>
        /// Initializes a new <see cref="FormattingRole"/>.
        /// </summary>
        protected FormattingRole()
        {
        }

        /// <summary>
        /// Gets the name of the <see cref="FormattingRole"/>.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the logging role name (see <see cref="LoggingRoles"/>).
        /// </summary>
        public abstract string LoggingRole { get; }
    }
}
