using System.ComponentModel;
using Metalama.Patterns.Observability.AspectTests.Include;
namespace Metalama.Patterns.Observability.AspectTests.InpcAutoPropertyWithInitializerNoRefs;
[Observable]
public class InpcAutoPropertyWithInitializerNoRefs : INotifyPropertyChanged
{
  private SimpleInpcByHand _x = new(42);
  public SimpleInpcByHand X
  {
    get
    {
      return this._x;
    }
    set
    {
      if (!object.ReferenceEquals(value, this._x))
      {
        var oldValue = this._x;
        this._x = value;
        this.OnObservablePropertyChanged("X", (INotifyPropertyChanged? )oldValue, value);
        this.OnPropertyChanged("X");
      }
    }
  }
  [InvokedFor("X")]
  protected virtual void OnObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}