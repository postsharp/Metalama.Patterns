using System.ComponentModel;
namespace Metalama.Patterns.Observability.AspectTests;
[Observable]
public sealed class SealedNonInpcAutoProperty : INotifyPropertyChanged
{
  private int _x;
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
  private void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}