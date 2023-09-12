// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A declaration that may be defined later.
/// </summary>
/// <typeparam name="T"></typeparam>
[CompileTime]
internal class DeferredDeclaration<T>
    where T : IDeclaration
{
    private T? _declaration;

    public DeferredDeclaration( bool? willBeDefined )
    {        
        this.WillBeDefined = willBeDefined;
    }    

    /// <summary>
    /// Gets a value indicating the future final state of <see cref="Declaration"/>. <see langword="null"/> 
    /// indicates that the declaration may or may not be defined.
    /// </summary>
    public bool? WillBeDefined { get; }

    /// <summary>
    /// Gets or sets the declaration. Template code will always see the final value.
    /// Non-template code can make preliminary decisions according to <see cref="WillBeDefined"/>,
    /// and should only rely upon the actual value of <see cref="Declaration"/> if the code
    /// is known to execute after the value has been set.
    /// </summary>
    public T? Declaration
    {
        get
        {
            switch ( this.WillBeDefined )
            {
                case true:
                    if ( this._declaration == null )
                    {
                        throw new ArgumentNullException( nameof( this.Declaration ), "The deferred declaration promised to provide a value but a value has not been set." );
                    }
                    break;
                case false:
                    if ( this._declaration != null )
                    {
                        throw new ArgumentNullException( nameof( this.Declaration ), "The deferred declaration promised not to provide a value but a value has been set." );
                    }
                    break;
            }

            return this._declaration;
        }

        set
        {
            switch ( this.WillBeDefined )
            {
                case true:
                    if ( value == null )
                    {
                        throw new ArgumentNullException( nameof( this.Declaration ), "The deferred declaration has promised to provide a value, the value cannot be null." );
                    }
                    break;
                case false:
                    if ( this._declaration != null )
                    {
                        throw new ArgumentNullException( nameof( this.Declaration ), "The deferred declaration has promised not to provide a value, the value must be null." );
                    }
                    break;
            }

            this._declaration = value;
        }
    }

    public static implicit operator T?( DeferredDeclaration<T> d ) => d.Declaration;
}