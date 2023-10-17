using System.ComponentModel;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
namespace Metalama.Patterns.NotifyPropertyChanged.AspectTests;
[NotifyPropertyChanged]
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