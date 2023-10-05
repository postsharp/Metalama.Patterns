﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using System.Collections.Concurrent;

namespace Metalama.Patterns.NotifyPropertyChanged.Implementation;

[CompileTime]
internal sealed class InpcInstrumentationKindLookup
{
    private readonly ConcurrentDictionary<IType, InpcInstrumentationKind> _inpcInstrumentationKindLookup = new();
    private readonly Func<IType, InpcInstrumentationKind> _getCore;
    private readonly Elements _elements;

    public InpcInstrumentationKindLookup( Elements elements )
    {
        this._getCore = this.GetCore;
        this._elements = elements;
    }

    public InpcInstrumentationKind Get( IType type )
    {
        return this._inpcInstrumentationKindLookup.GetOrAdd( type, this._getCore );
    }

    private InpcInstrumentationKind GetCore( IType type )
    {
        switch ( type )
        {
            case INamedType namedType:
                if ( namedType.SpecialType != SpecialType.None )
                {
                    // None of the special types implement INPC.
                    return InpcInstrumentationKind.None;
                }
                else if ( namedType.Equals( this._elements.INotifyPropertyChanged ) )
                {
                    return InpcInstrumentationKind.Implicit;
                }
                else if ( namedType.Is( this._elements.INotifyPropertyChanged ) )
                {
                    if ( namedType.TryFindImplementationForInterfaceMember( this._elements.PropertyChangedEventOfINotifyPropertyChanged, out var member ) )
                    {
                        return member.IsExplicitInterfaceImplementation ? InpcInstrumentationKind.Explicit : InpcInstrumentationKind.Implicit;
                    }

                    throw new InvalidOperationException( "Could not find implementation of interface member." );
                }
                else if ( !namedType.BelongsToCurrentProject )
                {
                    return InpcInstrumentationKind.None;
                }
                else
                {
                    if ( this._elements.Target.Compilation.IsPartial && !this._elements.Target.Compilation.Types.Contains( type ) )
                    {
                        return InpcInstrumentationKind.Unknown;
                    }

                    return namedType.Definition.Enhancements().HasAspect<NotifyPropertyChangedAttribute>()
                        ? InpcInstrumentationKind.Implicit // For now, the aspect always introduces implicit implementation.
                        : InpcInstrumentationKind.None;
                }

            case ITypeParameter typeParameter:
                var hasImplicit = false;

                foreach ( var t in typeParameter.TypeConstraints )
                {
                    var k = this.Get( t );

                    switch ( k )
                    {
                        case InpcInstrumentationKind.Implicit:
                            return InpcInstrumentationKind.Implicit;

                        case InpcInstrumentationKind.Explicit:
                            hasImplicit = true;

                            break;
                    }
                }

                return hasImplicit ? InpcInstrumentationKind.Implicit : InpcInstrumentationKind.None;

            default:
                return InpcInstrumentationKind.None;
        }
    }
}