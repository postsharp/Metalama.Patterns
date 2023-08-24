// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#pragma warning disable

Console.WriteLine( "Hello, World!" );

[NotifyPropertyChanged]
public class A
{
    public int A1 { get; set; }
}

public class A_Desired : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    // Hidden
    private int _a1;

    public int A1
    {
        get => _a1;
        set
        {
            if (  _a1 != value )
            {
                _a1 = value;
                OnPropertyChanged();
            }
        }
    }
}