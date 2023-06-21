// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.
using System;
using System.Diagnostics.CodeAnalysis;

namespace PostSharp.Patterns.Diagnostics.Custom
{
    /// <summary>
    /// Determines the behavior of the <see cref="LogEventData.VisitProperties{TVisitorState}(ILoggingPropertyVisitor{TVisitorState}, ref TVisitorState, in LoggingPropertyVisitorOptions)"/>
    /// method.
    /// </summary>
    [SuppressMessage( "Microsoft.Performance", "CA1815", Justification = "Equal is not a use case" )]
    public readonly struct LoggingPropertyVisitorOptions 
    {
        private readonly Flags flags;

        private LoggingPropertyVisitorOptions( Flags flags ) 
        {
            this.flags = flags;
        }

        /// <summary>
        /// Initializes a new <see cref="LoggingPropertyVisitorOptions"/>.
        /// </summary>
        /// <param name="onlyInherited">Determines if only inherited properties (those with the
        /// <see cref="LoggingPropertyOptions.IsInherited" qualifyHint="true"/> flag set to <c>true</c>) must be visited.</param>
        /// <param name="includeInherited">Determines if inherited properties must be included. This flag is taken into account by the visitors implemented on the logging contexts
        /// only. It is ignored by other visiting methods.</param>
        /// <param name="onlyRendered">Determines if only rendered properties must be visited.</param>
        public LoggingPropertyVisitorOptions( bool onlyInherited = false, bool includeInherited = false, bool onlyRendered = false )
        {
            this.flags = Flags.Default;

            if ( onlyInherited )
            {
                this.flags |= Flags.OnlyInherited;
            }

            if ( includeInherited )
            {
                this.flags |= Flags.IncludeInherited;
            }

            if ( onlyRendered )
            {
                this.flags |= Flags.OnlyRendered;
            }

        }

        /// <summary>
        /// Determines if only inherited properties (those with the <see cref="LoggingPropertyOptions.IsInherited" qualifyHint="true"/> flag set to <c>true</c>)
        /// must be visited.
        /// </summary>
        public bool OnlyInherited => (this.flags & Flags.OnlyInherited) != 0;


        /// <summary>
        /// Determines if inherited properties must be included. This flag is taken into account by the visitors implemented on the logging contexts
        /// only. It is ignored by other visiting methods.
        /// </summary>
        public bool IncludeInherited => (this.flags & Flags.IncludeInherited) != 0;


        /// <summary>
        /// Determines if only rendered properties must be visited.
        /// </summary>
        public bool OnlyRendered => (this.flags & Flags.OnlyRendered) != 0;

        /// <summary>
        /// Returns a copy of the current <see cref="LoggingPropertyVisitorOptions"/> but with a specific value of the <see cref="OnlyInherited"/> property.
        /// </summary>
        /// <param name="value">The new value of the <see cref="OnlyInherited"/> property.</param>
        /// <returns></returns>
        public LoggingPropertyVisitorOptions WithOnlyInherited(bool value = true )
        {
            return this.WithFlag(Flags.OnlyInherited, value );
        }

        /// <summary>
        /// Returns a copy of the current <see cref="LoggingPropertyVisitorOptions"/> but with a specific value of the <see cref="OnlyRendered"/> property.
        /// </summary>
        /// <param name="value">The new value of the <see cref="OnlyRendered"/> property.</param>
        /// <returns></returns>
        public LoggingPropertyVisitorOptions WithOnlyRendered( bool value = true )
        {
            return this.WithFlag( Flags.OnlyRendered, value );
        }

        /// <summary>
        /// Returns a copy of the current <see cref="LoggingPropertyVisitorOptions"/> but with a specific value of the <see cref="IncludeInherited"/> property.
        /// </summary>
        /// <param name="value">The new value of the <see cref="IncludeInherited"/> property.</param>
        /// <returns></returns>
        public LoggingPropertyVisitorOptions WithIncludeInherited( bool value = true )
        {
            return this.WithFlag( Flags.IncludeInherited, value );
        }


        private LoggingPropertyVisitorOptions WithFlag( Flags flag, bool value ) => new LoggingPropertyVisitorOptions(( this.flags & ~flag) | (value ? flag : Flags.Default));

        [Flags]
        private enum Flags
        {
            Default,
            OnlyInherited = 1,
            OnlyRendered = 2,
            IncludeInherited = 4
        }
    }

}


