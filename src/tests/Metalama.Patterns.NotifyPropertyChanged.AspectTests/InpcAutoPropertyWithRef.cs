// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.InpcAutoPropertyWithRef;

public class Foo : INotifyPropertyChanged
{
    private int _a;

    public int A
    {
        get => this._a;
        set
        {
            if ( value != this._a )
            {
                this._a = value;
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( nameof( this.A ) ) );
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
public class InpcAutoPropertyWithRef
{
    public Foo X { get; set; }

    public int Y => this.X.A;
}