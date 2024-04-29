using System.ComponentModel;
using Metalama.Patterns.Observability.Metadata;
namespace Metalama.Patterns.Observability.AspectTests;
[Observable]
public sealed class SealedNoProperties : INotifyPropertyChanged
{
  [OnChildPropertyChangedMethod]
  private void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  private void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}