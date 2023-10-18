using System.ComponentModel;
using Metalama.Patterns.Observability.Metadata;
namespace Metalama.Patterns.Observability.AspectTests;
[Observable]
public class NoProperties : INotifyPropertyChanged
{
  [OnChildPropertyChangedMethod(new string[] { })]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  [OnUnmonitoredObservablePropertyChangedMethod(new string[] { })]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}