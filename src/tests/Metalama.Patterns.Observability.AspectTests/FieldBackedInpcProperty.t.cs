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
          oldValue.PropertyChanged -= this._handle_xPropertyChanged;
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
  private PropertyChangedEventHandler? _handle_xPropertyChanged;
  protected virtual void OnPropertyChanged(string propertyName)
  {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
  private void SubscribeTo_x(A value)
  {
    if (value != null)
    {
      this._handle_xPropertyChanged ??= HandlePropertyChanged;
      value.PropertyChanged += this._handle_xPropertyChanged;
    }
    void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
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