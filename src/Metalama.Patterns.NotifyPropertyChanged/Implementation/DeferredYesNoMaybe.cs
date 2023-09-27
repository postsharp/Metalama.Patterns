// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A value that may be defined later, where the potential for <see cref="WillBeDefined"/> as specified at construction can be yes, no or maybe.
/// </summary>
/// <typeparam name="T"></typeparam>
[CompileTime]
internal class DeferredYesNoMaybe<T> : IReadOnlyDeferredYesNoMaybe<T>
    where T : class
{
    private readonly bool _mustBeSetBeforeGet;
    private T? _value;

    // ReSharper disable once MemberCanBeProtected.Global
    /// <summary>
    /// Initializes a new instance of the <see cref="DeferredYesNoMaybe{T}"/> class.
    /// </summary>
    /// <param name="willBeDefined">Indicates the future final state of <see cref="Value"/>. The default is <see langword="null"/>, indicating "maybe".</param>
    /// <param name="mustBeSetBeforeGet"> If <see langword="true"/>, the <see cref="Value"/> getter will throw if the
    /// setter has not yet been called (regardless of whether the value was <see langword="null"/> or not). The default is <see langword="false"/>.
    /// Note that the <see cref="Value"/> getter also checks that the current value is compatible with <see cref="WillBeDefined"/>.</param>
    public DeferredYesNoMaybe( bool? willBeDefined = null, bool mustBeSetBeforeGet = false )
    {
        this._mustBeSetBeforeGet = mustBeSetBeforeGet;
        this.WillBeDefined = willBeDefined;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    /// <summary>
    /// Gets a value indicating whether the future final state of <see cref="Value"/> will be defined (ie, not <see langword="null"/>).
    /// If <see cref="WillBeDefined"/> is <see langword="null"/>, this indicates that the value may or may not be defined.
    /// </summary>
    public bool? WillBeDefined { get; }    

    /// <summary>
    /// Gets a value indicating whether the <see cref="Value"/> setter has been successfully invoked. The
    /// set value may be <see langword="null"/> if allowed by <see cref="WillBeDefined"/>.
    /// </summary>
    public bool ValueIsSet { get; private set; }

    /// <summary>
    /// Gets or sets the deferred value. The value defaults to <see langword="null"/>, and can be
    /// set at most once. Code that expects to execute before the value will be set can make preliminary 
    /// decisions according to <see cref="WillBeDefined"/>. Only get the value of <see cref="Value"/>
    /// from code is known to execute after the value should have been set.
    /// </summary>
    public T? Value
    {
        get
        {
            if ( this._mustBeSetBeforeGet && !this.ValueIsSet )
            {
                throw new InvalidOperationException( nameof(this.Value) + " must be set before it can be read." );
            }

            switch ( this.WillBeDefined )
            {
                case true:
                    if ( this._value == null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Value),
                            "The deferred value promised to provide a non-null value but a non-null value has not been set." );
                    }

                    break;

                case false:
                    if ( this._value != null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Value),
                            "The deferred value promised not to provide a non-null value but a non-null value has been set." );
                    }

                    break;
            }

            return this._value;
        }

        set
        {
            if ( this.ValueIsSet && value != this._value )
            {
                throw new InvalidOperationException( "The value has already been set, the value cannot be changed." );
            }

            switch ( this.WillBeDefined )
            {
                case true:
                    if ( value == null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Value),
                            "The deferred value has promised to provide a non-null value, the value cannot be null." );
                    }

                    break;

                case false:
                    if ( this._value != null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Value),
                            "The deferred value has promised not to provide a non-null value, the value must be null." );
                    }

                    break;
            }

            this._value = value;
            this.ValueIsSet = true;
        }
    }
}