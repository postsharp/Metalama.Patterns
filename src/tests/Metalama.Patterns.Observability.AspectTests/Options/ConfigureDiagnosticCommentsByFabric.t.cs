[Observable]
public class ConfigureDiagnosticCommentsByFabric : INotifyPropertyChanged
{
    protected virtual void OnPropertyChanged( string propertyName )
    {
        // Template: OnPropertyChanged
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}