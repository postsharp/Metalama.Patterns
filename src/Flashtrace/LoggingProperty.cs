// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Flashtrace.Custom;
using System.Runtime.Serialization;

namespace Flashtrace
{
    /// <summary>
    /// Represents a property (a name, a value and a few options).
    /// </summary>
    public sealed class LoggingProperty
    {
        private readonly object value;
        private readonly Func<object> func;
        private LoggingPropertyOptions options;

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the property value. The property is never rendered when the value is <c>null</c>.
        /// If the <see cref="LoggingProperty"/> has been initialized with a <c>Func&lt;object&gt;</c>, this property
        /// will evaluate the delegate every time the property getter is invoked.
        /// </summary>
        public object Value
        {
            get
            {
                if ( this.func != null )
                {
                    return this.func();
                }
                else
                {
                    return this.value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the formatter used to render the <see cref="Value"/> as a string. By default, the default formatter
        /// for the property value type is used.
        /// </summary>
        public IFormatter Formatter { get; set; }

        /// <summary>
        /// Determines whether the property will be included in the log message. The default value is <c>false</c>.
        /// </summary>
        public bool IsRendered
        {
            get => this.options.IsRendered;
            set => this.options = this.options.WithIsRendered( value );
        }

        /// <summary>
        /// Determines whether the property is inherited from the parent activity to children activities and messages. The default value is <c>true</c>.
        ///  When this property is set to <c>false</c>, <see cref="IsBaggage"/> is automatically set to <c>false</c>.
        /// </summary>
        public bool IsInherited
        {
            get => this.options.IsInherited;
            set => this.options = this.options.WithIsInherited( value );
        }

        /// <summary>
        /// Determines whether the property is cross-process. The default value is <c>false</c>. When this property is set to <c>true</c>, <see cref="IsInherited"/> is automatically
        /// set to <c>true</c>.
        /// </summary>
        public bool IsBaggage
        {
            get => this.options.IsBaggage;
            set => this.options = this.options.WithIsBaggage( value );
        }

        internal LoggingPropertyOptions Options => this.options;

        /// <summary>
        /// Initializes a new <see cref="LoggingProperty"/> and assigns it to a constant value.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public LoggingProperty( [Required] string name, object value )
        {
            this.Name = name;
            this.value = value;
        }

        internal LoggingProperty( [Required] string name, object value, LoggingPropertyOptions options )
        {
            this.Name = name;
            this.value = value;
            this.options = options;
        }

        /// <summary>
        /// Initializes a new <see cref="LoggingProperty"/> and assigns it to a dynamic value.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="func">A function returning the property value. This function will be evaluated every time the <see cref="Value"/> getter is invoked.</param>
        public LoggingProperty( [Required] string name, Func<object> func )
        {
            this.Name = name;
            this.func = func;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{this.Name}={this.Value}";
    }
}