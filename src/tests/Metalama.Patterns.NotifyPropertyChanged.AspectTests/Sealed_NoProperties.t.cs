namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;
[NotifyPropertyChanged]
public sealed class SealedNoProperties : global::System.ComponentModel.INotifyPropertyChanged
{
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