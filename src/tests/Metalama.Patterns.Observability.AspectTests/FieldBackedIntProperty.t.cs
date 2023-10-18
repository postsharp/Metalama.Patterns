using System.ComponentModel;
using Metalama.Patterns.Observability.Metadata;
namespace Metalama.Patterns.Observability.AspectTests.FieldBackedIntProperty;
[Observable]
public class FieldBackedIntProperty : INotifyPropertyChanged
{
  private int _x1;
  private int _x
  {
    get
    {
      return this._x1;
    }
    set
    {
      if (this._x1 != value)
      {
        this._x1 = value;
        this.OnPropertyChanged("X");
        this.OnPropertyChanged("Y");
      }
    }
  }
  public int X => this._x;
  public int Y => this.X;
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