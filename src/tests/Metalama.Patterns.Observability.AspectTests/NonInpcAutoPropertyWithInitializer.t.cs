using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests.NonInpcAutoPropertyWithInitializer;
[Observable]
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
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}