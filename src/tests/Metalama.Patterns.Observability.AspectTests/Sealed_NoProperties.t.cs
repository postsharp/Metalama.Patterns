using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests;
[Observable]
public sealed class SealedNoProperties : INotifyPropertyChanged
{
    private void OnPropertyChanged( string propertyName )
    {
        this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
    }
    public event PropertyChangedEventHandler? PropertyChanged;
}