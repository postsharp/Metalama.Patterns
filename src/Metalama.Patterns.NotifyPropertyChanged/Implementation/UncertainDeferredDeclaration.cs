// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A declaration that may be defined later, where the potential for future declaration can be "maybe" (that is,
/// when <see cref="WillBeDefined"/> is <see langword="null"/>).
/// </summary>
/// <typeparam name="T"></typeparam>
[CompileTime]
internal class UncertainDeferredDeclaration<T>
    where T : class, IDeclaration
{
    private readonly bool _mustBeSetBeforeGet;
    private T? _declaration;

    // ReSharper disable once MemberCanBeProtected.Global
    /// <summary>
    /// Initializes a new instance of the <see cref="UncertainDeferredDeclaration{T}"/> class.
    /// </summary>
    /// <param name="willBeDefined">Indicates the future final state of <see cref="Declaration"/>. The default is <see langword="null"/>, indicating "maybe".</param>
    /// <param name="mustBeSetBeforeGet"> If <see langword="true"/>, the <see cref="Declaration"/> getter will throw if the
    /// setter has not yet been called (regardless of whether the value was <see langword="null"/> or not). The default is <see langword="false"/>.
    /// Note that the <see cref="Declaration"/> getter also checks that the current value is compatible with <see cref="WillBeDefined"/>.</param>
    public UncertainDeferredDeclaration( bool? willBeDefined = null, bool mustBeSetBeforeGet = false )
    {
        this._mustBeSetBeforeGet = mustBeSetBeforeGet;
        this.WillBeDefined = willBeDefined;
    }

    // ReSharper disable once MemberCanBeProtected.Global
    /// <summary>
    /// Gets a value indicating the future final state of <see cref="Declaration"/>. <see langword="null"/> 
    /// indicates that the declaration may or may not be defined.
    /// </summary>
    public bool? WillBeDefined { get; }    

    /// <summary>
    /// Gets a value indicating whether the <see cref="Declaration"/> setter has been successfully invoked.
    /// </summary>
    public bool DeclarationIsSet { get; private set; }

    /// <summary>
    /// Gets or sets the declaration. The declaration defaults to <see langword="null"/>, and can be
    /// set at most once. Code that expects to execute before the value will be set can make preliminary 
    /// decisions according to <see cref="WillBeDefined"/>. Only get the value of <see cref="Declaration"/>
    /// from code is known to execute after the value should have been set.
    /// </summary>
    public T? Declaration
    {
        get
        {
            if ( this._mustBeSetBeforeGet && !this.DeclarationIsSet )
            {
                throw new InvalidOperationException( nameof( this.Declaration ) + " must be set before it can be read." );
            }

            switch ( this.WillBeDefined )
            {
                case true:
                    if ( this._declaration == null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Declaration),
                            "The deferred declaration promised to provide a non-null value but a non-null value has not been set." );
                    }

                    break;

                case false:
                    if ( this._declaration != null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Declaration),
                            "The deferred declaration promised not to provide a non-null value but a non-null value has been set." );
                    }

                    break;
            }

            return this._declaration;
        }

        set
        {
            if ( this.DeclarationIsSet && value != this._declaration )
            {
                throw new InvalidOperationException( "The declaration has already been set, the value cannot be changed." );
            }

            switch ( this.WillBeDefined )
            {
                case true:
                    if ( value == null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Declaration),
                            "The deferred declaration has promised to provide a non-null value, the value cannot be null." );
                    }

                    break;

                case false:
                    if ( this._declaration != null )
                    {
                        throw new ArgumentNullException(
                            nameof(this.Declaration),
                            "The deferred declaration has promised not to provide a non-null value, the value must be null." );
                    }

                    break;
            }

            this._declaration = value;
            this.DeclarationIsSet = true;
        }
    }
}