namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;
[NotifyPropertyChanged]
public class NonInpcAutoProperty : global::System.ComponentModel.INotifyPropertyChanged
{
    private int _x;
    public int X
    {
        get
        {
            return this._x;
        }
        set
        {
            if ( (this._x != value) )
            {
                this._x = value;
                this.OnPropertyChanged( "X" );
            }
        }
    }
    [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnChildPropertyChangedMethodAttribute( new global::System.String[] { } )]
    protected virtual void OnChildPropertyChanged( global::System.String parentPropertyPath, global::System.String propertyName )
    {
    }
    protected virtual void OnPropertyChanged( global::System.String propertyName )
    {
        this.PropertyChanged?.Invoke( this, new global::System.ComponentModel.PropertyChangedEventArgs( propertyName ) );
    }
    [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute( new global::System.String[] { } )]
    protected virtual void OnUnmonitoredObservablePropertyChanged( global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue )
    {
    }
    public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}