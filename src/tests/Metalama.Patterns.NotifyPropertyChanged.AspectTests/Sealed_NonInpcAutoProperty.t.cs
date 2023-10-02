namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;
[NotifyPropertyChanged]
public sealed class SealedNonInpcAutoProperty : global::System.ComponentModel.INotifyPropertyChanged
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
    private void OnChildPropertyChanged( global::System.String parentPropertyPath, global::System.String propertyName )
    {
    }
    private void OnPropertyChanged( global::System.String propertyName )
    {
        this.PropertyChanged?.Invoke( this, new global::System.ComponentModel.PropertyChangedEventArgs( propertyName ) );
    }
    public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}