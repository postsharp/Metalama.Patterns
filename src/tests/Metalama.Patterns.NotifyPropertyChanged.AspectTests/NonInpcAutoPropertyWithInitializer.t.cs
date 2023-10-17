using System.ComponentModel;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests.NonInpcAutoPropertyWithInitializer;
[NotifyPropertyChanged]
public class NonInpcAutoPropertyWithInitializer : INotifyPropertyChanged
{
  private int _x = 42;
  public int X
  {
    get
    {
      return this._x;
    }
    set
    {
      if (this._x != value)
      {
        this._x = value;
        this.OnPropertyChanged("X");
      }
    }
  }
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