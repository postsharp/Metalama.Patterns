﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation.Natural;

[CompileTime]
internal sealed class Elements : CommonElements
{
    public Elements( INamedType target ) : base( target )
    {
        this.OnChildPropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof(OnChildPropertyChangedMethodAttribute) );
        this.OnUnmonitoredObservablePropertyChangedMethodAttribute = (INamedType) TypeFactory.GetType( typeof(OnUnmonitoredObservablePropertyChangedMethodAttribute) );
    }

    public INamedType OnChildPropertyChangedMethodAttribute { get; }

    public INamedType OnUnmonitoredObservablePropertyChangedMethodAttribute { get; }
}