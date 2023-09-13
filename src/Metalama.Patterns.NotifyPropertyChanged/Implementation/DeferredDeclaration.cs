// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A declaration that will or will not be defined later. There is no maybe.
/// </summary>
/// <typeparam name="T"></typeparam>
[CompileTime]
internal class DeferredDeclaration<T> : AmbiguousDeferredDeclaration<T>
    where T : IDeclaration
{
    public DeferredDeclaration( bool willBeDefined )
        : base( willBeDefined )
    { 
    }

    public new bool WillBeDefined => base.WillBeDefined == true;

    /// <summary>
    /// Gets or sets the declaration. Template code will always see the final value,
    /// which will not be <see langword="null"/>.
    /// </summary>
    public new T? Declaration
    {
        get => base.Declaration!;
        set => base.Declaration = value;
    }

    public static implicit operator T?( DeferredDeclaration<T> d ) => d.Declaration;
}