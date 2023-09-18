// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// @RemoveOutputCode
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;

public class ExistingInpcImplWithValidOPCMethodNamedNotifyOfPropertyChange : INotifyPropertyChanged

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

    protected virtual void NotifyOfPropertyChange( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new( propertyName ) );
    }
}

// <target>
[NotifyPropertyChanged]
public partial class InheritsExistingInpcImplWithValidOPCMethodNamedNotifyOfPropertyChange : ExistingInpcImplWithValidOPCMethodNamedNotifyOfPropertyChange
{
}