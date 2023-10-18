// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.UnitTests.Assets.Command;

public class ManualInpcIntegrationTestClass : CommandTestBase, INotifyPropertyChanged
{
    [Command]
    public ICommand FooCommand { get; }

    private void ExecuteFoo( int v )
    {
        LogCall( $"{v}" );
    }

    private bool _canExecuteFoo;

    public bool CanExecuteFoo
    {
        get => this._canExecuteFoo;
        set
        {
            if ( value != this._canExecuteFoo )
            {
                this._canExecuteFoo = value;
                this.OnPropertyChanged( nameof( this.CanExecuteFoo ) );
            }
        }
    }

    private void OnPropertyChanged( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}