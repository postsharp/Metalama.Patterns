[NotifyPropertyChanged]
public class InpcAutoPropertyNoRefs : global::System.ComponentModel.INotifyPropertyChanged
{
    private SimpleInpcByHand _x = default!;
    public SimpleInpcByHand X
    {
        get
        {
            return this._x;
        }
        set
        {
            if ( !global::System.Object.ReferenceEquals( value, this._x ) )
            {
                var oldValue = this._x;
                this._x = value;
                this.OnUnmonitoredObservablePropertyChanged( "X", (global::System.ComponentModel.INotifyPropertyChanged?) oldValue, value );
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
    [global::Metalama.Patterns.NotifyPropertyChanged.Metadata.OnUnmonitoredObservablePropertyChangedMethodAttribute( new global::System.String[] { "X" } )]
    protected virtual void OnUnmonitoredObservablePropertyChanged( global::System.String propertyPath, global::System.ComponentModel.INotifyPropertyChanged? oldValue, global::System.ComponentModel.INotifyPropertyChanged? newValue )
    {
    }
    public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}