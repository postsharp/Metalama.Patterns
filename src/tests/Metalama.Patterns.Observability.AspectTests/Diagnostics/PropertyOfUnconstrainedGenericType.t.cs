using System.Collections.Generic;
using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests.Diagnostics;
[Observable]
public class PropertyOfUnconstrainedGenericType<T> : INotifyPropertyChanged
{
  private T _c1 = default !;
  public T C1
  {
    get
    {
      return this._c1;
    }
    set
    {
      if (!EqualityComparer<T>.Default.Equals(value, this._c1))
      {
        this._c1 = value;
        this.OnPropertyChanged("C1");
      }
    }
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}