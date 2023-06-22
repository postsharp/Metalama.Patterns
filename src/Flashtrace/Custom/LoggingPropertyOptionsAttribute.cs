// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

namespace Flashtrace
{
    /// <summary>
    /// Specifies the options (<see cref="LoggingPropertyOptions"/>) of a logging property that is
    /// expresses as a public property of a CLR type.
    /// </summary>
    [AttributeUsage( AttributeTargets.Property )]
    public sealed class LoggingPropertyOptionsAttribute : Attribute
    {
        /// <summary>
        /// Specifies that this property is ignored, i.e. it should not be mapped to a logging property.
        /// </summary>
        public bool IsIgnored { get; set; }

        /// <summary>
        /// Specifies that this property must be included into the message.
        /// </summary>
        public bool IsRendered { get; set; }

        /// <summary>
        /// Specifies that this property is inherited by child contexts.
        /// </summary>
        public bool IsInherited { get; set; }

        /// <summary>
        /// Specifies that this property must be carried in cross-process requests.
        /// </summary>
        public bool IsBaggage { get; set; }

        /// <summary>
        /// Converts the current attribute into a <see cref="LoggingPropertyOptions"/>.
        /// </summary>
        /// <returns></returns>
        public LoggingPropertyOptions ToOptions()
        {
            return new LoggingPropertyOptions(
                isRendered: this.IsRendered,
                isInherited: this.IsInherited,
                isBaggage: this.IsBaggage,
                isIgnored: this.IsIgnored );
        }
    }
}