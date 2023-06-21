// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using Flashtrace.Formatters;
using System.Diagnostics.CodeAnalysis;

namespace Flashtrace.Custom
{

    /// <summary>
    /// Specifies the behavior of logging properties (exposed by <see cref="LogEventData"/>), such as
    /// <see cref="IsRendered"/>, <see cref="IsInherited"/> or <see cref="IsBaggage"/>.
    /// </summary>
    [SuppressMessage( "Microsoft.Performance", "CA1815", Justification = "Equal is not a use case" )]
    public readonly struct LoggingPropertyOptions
    {
        private readonly Flags flags;

        private LoggingPropertyOptions(Flags flags, IFormatter formatter)
        {
            this.flags = flags;
            this.Formatter = formatter;
        }

        /// <summary>
        /// Initializes a new <see cref="LoggingPropertyOptions"/>.
        /// </summary>
        /// <param name="isRendered">Determines whether the property will be included in the log message. The default value is <c>false</c>, then
        /// the property is only available as an additional property, if this is supported by the backend.</param>
        /// <param name="isInherited"> Determines whether the property is inherited from the parent activity to children activities and messages. The default value is <c>true</c>.
        ///  When this property is set to <c>false</c>, <see cref="IsBaggage"/> is automatically set to <c>false</c>.
        /// </param>
        /// <param name="isBaggage">
        ///  Determines whether the property is cross-process. The default value is <c>false</c>. When this property is set to <c>true</c>, <see cref="IsInherited"/> is automatically
        /// set to <c>true</c>.
        /// </param>
        /// <param name="isIgnored">Determines whether this property must be ignored by the <see cref="LogEventMetadata.VisitProperties{TVisitorState}(object, ILoggingPropertyVisitor{TVisitorState}, ref TVisitorState, in LoggingPropertyVisitorOptions)"/>
        /// method.</param>
        /// <param name="formatter">The formatter to be used to render the property value.</param>
        public LoggingPropertyOptions( bool isRendered = false, bool isInherited = false, bool isBaggage = false, bool isIgnored = false,
            IFormatter formatter = null )
        {
            this.flags = Flags.None;

            if ( isRendered )
            {
                this.flags |= Flags.IsRendered;
            }

            if ( isInherited )
            {
                this.flags |= Flags.IsInherited;
            }

            if ( isBaggage )
            {
                this.flags |= Flags.IsBaggage | Flags.IsInherited;
            }

            if ( isIgnored )
            {
                this.flags |= Flags.IsIgnored;
            }

            this.Formatter = formatter;

        }

        /// <summary>
        /// Gets the formatter to be used to render the property value.
        /// </summary>
        public IFormatter Formatter { get; }

        /// <summary>
        /// Returns a copy of the current <seealso cref="LoggingPropertyOptions"/> but with a different value of the <see cref="Formatter"/> property.
        /// </summary>
        /// <param name="formatter">The formatter to be used to render the property value.</param>
        /// <returns></returns>
        public LoggingPropertyOptions WithFormatter( IFormatter formatter )
        {
            return new LoggingPropertyOptions( this.flags, formatter);
        }


        /// <summary>
        /// Determines whether this property must be ignored by the <see cref="LogEventMetadata.VisitProperties{TVisitorState}(object, ILoggingPropertyVisitor{TVisitorState}, ref TVisitorState, in LoggingPropertyVisitorOptions)"/>
        /// method. This value is typically only returned by <see cref="LogEventMetadata.GetPropertyOptions(string)"/> to say that a property of the raw CLR object
        /// must not be exposed as a logging property.
        /// </summary>
        public bool IsIgnored => (this.flags & Flags.IsIgnored) != 0;


        /// <summary>
        /// Returns a copy of the current <seealso cref="LoggingPropertyOptions"/> but with a different value of the <see cref="IsIgnored"/> property.
        /// </summary>
        /// <param name="value">New value of the <see cref="IsIgnored"/> property.</param>
        /// <returns></returns>
        public LoggingPropertyOptions WithIsIgnored(bool value)
        {
            Flags otherFlags = this.flags & ~Flags.IsIgnored;
            Flags thisFlag = value ? Flags.IsIgnored : Flags.None;
            return new LoggingPropertyOptions( otherFlags | thisFlag, this.Formatter);
        }


        /// <summary>
        /// Determines whether the property will be included in the log message. The default value is <c>false</c>, then
        /// the property is only available as an additional property, if this is supported by the backend.
        /// </summary>
        public bool IsRendered => (this.flags & Flags.IsRendered) != 0;

        /// <summary>
        /// Returns a copy of the current <seealso cref="LoggingPropertyOptions"/> but with a different value of the <see cref="IsRendered"/> property.
        /// </summary>
        /// <param name="value">New value of the <see cref="IsRendered"/> property.</param>
        /// <returns></returns>

        public LoggingPropertyOptions WithIsRendered( bool value )
        {
            Flags otherFlags = this.flags & ~Flags.IsRendered;
            Flags thisFlag = value ? Flags.IsRendered : Flags.None;
            return new LoggingPropertyOptions( otherFlags | thisFlag, this.Formatter );
        }

        /// <summary>
        /// Determines whether the property is inherited from the parent activity to children activities and messages. The default value is <c>true</c>.
        ///  When this property is set to <c>false</c>, <see cref="IsBaggage"/> is automatically set to <c>false</c>.
        /// </summary>
        public bool IsInherited => (this.flags & Flags.IsInherited) != 0;

        /// <summary>
        /// Returns a copy of the current <seealso cref="LoggingPropertyOptions"/> but with a different value of the <see cref="IsInherited"/> property.
        /// </summary>
        /// <param name="value">New value of the <see cref="IsInherited"/> property.</param>
        /// <returns></returns>
        public LoggingPropertyOptions WithIsInherited( bool value )
        {
            Flags otherFlags = this.flags & ~Flags.IsInherited;
            Flags thisFlag = value ? Flags.IsInherited : Flags.None;
            return new LoggingPropertyOptions( otherFlags | thisFlag, this.Formatter );
        }

        /// <summary>
        /// Determines whether the property is cross-process. The default value is <c>false</c>. When this property is set to <c>true</c>, <see cref="IsInherited"/> is automatically
        /// set to <c>true</c>.
        /// </summary>
        public bool IsBaggage => (this.flags & Flags.IsBaggage) != 0;

        /// <summary>
        /// Returns a copy of the current <seealso cref="LoggingPropertyOptions"/> but with a different value of the <see cref="IsBaggage"/> property.
        /// </summary>
        /// <param name="value">New value of the <see cref="IsBaggage"/> property.</param>
        /// <returns></returns>
        public LoggingPropertyOptions WithIsBaggage( bool value )
        {
            Flags otherFlags = this.flags & ~(Flags.IsBaggage);
            Flags thisFlag = value ? (Flags.IsBaggage|Flags.IsInherited) : Flags.None;
            return new LoggingPropertyOptions( otherFlags | thisFlag, this.Formatter );
        }

        [Flags]
        private enum Flags
        {
            None,
            IsRendered = 1,
            IsInherited = 2,
            IsBaggage = 4,
            IsIgnored = 8
        }

    }

}


