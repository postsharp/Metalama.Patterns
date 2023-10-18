using System.ComponentModel;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;
[NotifyPropertyChanged]
public sealed class SealedNoProperties : INotifyPropertyChanged
{
  [OnChildPropertyChangedMethod(new string[] { })]
  private void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  private void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}