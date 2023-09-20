﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;

public class ExistingInpcImplWithoutNPCMethod : INotifyPropertyChanged
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
                this.PropertyChanged?.Invoke( this, new( nameof( this.EX1 ) ) );
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

// <target>
[NotifyPropertyChanged]
public partial class InheritsExistingInpcImplWithoutNPCMethod : ExistingInpcImplWithoutNPCMethod
{
}