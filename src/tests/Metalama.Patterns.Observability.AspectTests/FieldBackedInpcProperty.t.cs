[Observable]
public class FieldBackedInpcProperty : INotifyPropertyChanged
{
  private A _x1 = default !;
  private A _x
  {
    get
    {
      return this._x1;
    }
    set
    {
      if (!object.ReferenceEquals(value, this._x1))
      {
        var oldValue = this._x1;
        if (oldValue != null)
        {
          oldValue.PropertyChanged -= this._on_xPropertyChangedHandler;
        }
        this._x1 = value;
        this.OnPropertyChanged("P1");
        this.OnPropertyChanged("P2");
        this.SubscribeTo_x(value);
      }
    }
  }
  public A P1 => this._x;
  public int P2 => this._x.A1;
  private PropertyChangedEventHandler? _on_xPropertyChangedHandler;
  [OnChildPropertyChangedMethod]
  protected virtual void OnChildPropertyChanged(string parentPropertyPath, string propertyName)
  {
  }
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  [OnUnmonitoredObservablePropertyChangedMethod]
  protected virtual void OnUnmonitoredObservablePropertyChanged(string propertyPath, INotifyPropertyChanged? oldValue, INotifyPropertyChanged? newValue)
  {
  }
  private void SubscribeTo_x(A value)
  {
    if (value != null)
    {
      this._on_xPropertyChangedHandler ??= OnChildPropertyChanged_1;
      value.PropertyChanged += this._on_xPropertyChangedHandler;
    }
    void OnChildPropertyChanged_1(object? sender, PropertyChangedEventArgs e)
    {
      {
        var propertyName = e.PropertyName;
        if (propertyName == "A1")
        {
          this.OnPropertyChanged("P2");
          return;
        }
      }
    }
  }
  public event PropertyChangedEventHandler? PropertyChanged;
}