﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

/// <summary>
/// A declaration that will certainly be defined later.
/// </summary>
/// <typeparam name="T"></typeparam>
[CompileTime]
internal sealed class CertainDeferredDeclaration<T> : DeferredDeclaration<T>, IReadOnlyCertainDeferredDeclaration<T>
    where T : class, IDeclaration
{
    public CertainDeferredDeclaration()
        : base( true ) { }

    // ReSharper disable once UnusedMember.Global
    [Obsolete( "The value will always be true for " + nameof(CertainDeferredDeclaration<T>) + ", avoid unnecessary conditions." )]
    public new bool WillBeDefined => true;

    /// <summary>
    /// Gets or sets the declaration. Code that expects to execute before the value will be set can assume
    /// that the value will be set. Only get the actual value of <see cref="Declaration"/> from code that is known 
    /// to execute after the value should have been set.
    /// </summary>
    public new T Declaration
    {
        get => base.Declaration!;
        set => base.Declaration = value;
    }
}