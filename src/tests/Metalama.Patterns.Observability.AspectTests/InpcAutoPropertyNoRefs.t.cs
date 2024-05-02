[Observable]
public class InpcAutoPropertyNoRefs : INotifyPropertyChanged
{
  private SimpleInpcByHand _x = default !;
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
  [ObservedExpressions("X")]
  protected virtual void OnObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}