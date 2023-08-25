// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;

#pragma warning disable

Console.WriteLine( "Hello, World!" );

[NotifyPropertyChanged]
public class A
{
    public int A1 { get; set; }

    public B A2 { get; set; }

    public int A3 => A2.B1;
}

// For now user must explicitly (or via fabric) apply [NPC]. Automatic application applies
// only to inheritance, not to dependency types. Should we consider automatic application?
[NotifyPropertyChanged]
public class B
{
    public int B1 { get; set; }
}

public class A_Desired : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    protected virtual void OnPropertyChanged<T>( T? oldValue, T? newValue, [CallerMemberName] string? propertyName = null )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs<T>( oldValue, newValue, propertyName ) );
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

    private B_Desired _a2;

    public B_Desired A2
    {
        get => _a2;
        set
        {
            if ( !ReferenceEquals( _a2, value ) )
            {
                var oldValue = _a2;

                // Only if there are child dependencies:
                if ( oldValue != null )
                {
                    oldValue.PropertyChanged -= _onA2PropertyChangedHandler;
                }
                // ---

                _a2 = value;
                OnPropertyChanged( oldValue, value );

                // Only if there are child dependencies:
                if ( value != null )
                {
                    _onA2PropertyChangedHandler ??= this.OnA2PropertyChanged;
                    value.PropertyChanged += _onA2PropertyChangedHandler;
                }
                // ---
            }
        }
    }

    public int A3 => A2.B1;

    #region Only if there are child dependencies:

    private PropertyChangedEventHandler? _onA2PropertyChangedHandler;

    private void OnA2PropertyChanged( object sender, PropertyChangedEventArgs e )
    {
        switch ( e.PropertyName )
        {
            case nameof( B.B1 ):
                OnPropertyChanged( nameof( this.A3 ) );
                break;
        }
    }

    #endregion
}

public class B_Desired : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // If B was sealed, this would be private.
    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
    
    // If B was sealed, we would know that this method will not be used, so don't emit it.
    protected virtual void OnPropertyChanged<T>( T? oldValue, T? newValue, [CallerMemberName] string? propertyName = null )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs<T>( oldValue, newValue, propertyName ) );
    }

    // Hidden
    private int _b1;

    public int B1
    {
        get => _b1;
        set
        {
            if ( _b1 != value )
            {
                _b1 = value;
                OnPropertyChanged();
            }
        }
    }
}