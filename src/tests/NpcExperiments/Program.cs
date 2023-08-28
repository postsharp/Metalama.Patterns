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

    public B A2 { get; set; }

    public int A3 => A2.B1;

    public int A4 => A2.B2.C1;
}


[NotifyPropertyChanged]
public class C
{
    public int C1 { get; set; }
}

[NotifyPropertyChanged]
public class B
{
    public int B1 { get; set; }

    public C B2 { get; set; }
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

    private B_Desired _a2;

    public B_Desired A2
    {
        get => _a2;
        set
        {
            if ( !ReferenceEquals( _a2, value ) )
            {
                var oldValue = _a2;

                // A3 depends on A2.B1, A4 depends on A2.B2, so we need to track changes to A2:
                if ( oldValue != null )
                {
                    oldValue.PropertyChanged -= _onA2PropertyChangedHandler;
                }

                _a2 = value;
                OnPropertyChanged();

                if ( _a2 != null )
                {
                    _onA2PropertyChangedHandler ??= this.OnA2PropertyChanged;
                    _a2.PropertyChanged += _onA2PropertyChangedHandler;
                }

                UpdateA2Children();
            }
        }
    }

    private void UpdateA2Children()
    {
        UpdateA2B2();
        // ...
    }

    // Keep track of the last value of A2.B2, so we can unsubscribe from it.
    C_Desired _lastA2B2;

    private void UpdateA2B2()
    {
        // A4 depends on A2.B2.C1, so we need to track changes to A2.B2
        var newA2B2 = _a2?.B2;
        
        if ( !ReferenceEquals( newA2B2, _lastA2B2 ) )
        {
            if ( _lastA2B2 != null )
            {
                _lastA2B2.PropertyChanged -= _onA2B2PropertyChangedHandler;
            }

            if ( newA2B2 != null )
            {
                _onA2B2PropertyChangedHandler ??= this.OnA2B2PropertyChanged;
                newA2B2.PropertyChanged += _onA2B2PropertyChangedHandler;
            }

            _lastA2B2 = newA2B2;

            OnPropertyChanged( nameof( this.A4 ) );
        }
    }

    public int A3 => A2.B1;

    public int A4 => A2.B2.C1;

    private PropertyChangedEventHandler? _onA2PropertyChangedHandler;

    private void OnA2PropertyChanged( object sender, PropertyChangedEventArgs e )
    {
        switch ( e.PropertyName )
        {
            case nameof( B.B1 ):
                OnPropertyChanged( nameof( this.A3 ) );
                break;

            case nameof( B.B2 ):
                // A2.B2 ref has changed
                UpdateA2B2();                
                break;
        }
    }

    private PropertyChangedEventHandler? _onA2B2PropertyChangedHandler;

    private void OnA2B2PropertyChanged( object sender, PropertyChangedEventArgs e )
    {
        switch ( e.PropertyName )
        {
            case nameof( C.C1 ):
                OnPropertyChanged( nameof( this.A4 ) );
                break;
        }
    }
}

public class B_Desired : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // If B was sealed, this would be private.
    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
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

    private C_Desired _b2;

    public C_Desired B2
    {
        get => _b2;
        set
        {
            if ( !ReferenceEquals( _b2, value ) )
            {
                // No property of B depends on a child property of C instance B2, so we don't need to register with B2.
                _b2 = value;
                OnPropertyChanged();
            }
        }
    }
}

public class C_Desired : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
    {
        PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }

    int _c1;

    public int C1
    {
        get => _c1;
        set
        {
            if ( _c1 != value )
            {
                _c1 = value;
                OnPropertyChanged();
            }
        }
    }
}