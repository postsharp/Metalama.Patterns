﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.Observability.AspectTests;

public class ExistingInpcImplWithValidOpcMethodNamedNotifyOfPropertyChange : INotifyPropertyChanged
{
    private int _ex1;

    public int EX1
    {
        get => this._ex1;
        set
        {
            if ( value != this._ex1 )
            {
                this._ex1 = value;
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof(this.EX1) ) );
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void NotifyOfPropertyChange( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
}

// <target>
[Observable]
public partial class InheritsExistingInpcImplWithValidOpcMethodNamedNotifyOfPropertyChange : ExistingInpcImplWithValidOpcMethodNamedNotifyOfPropertyChange { }